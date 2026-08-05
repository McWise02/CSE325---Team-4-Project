using Microsoft.EntityFrameworkCore;
using RecipeAndMealTracker.Data;
using RecipeAndMealTracker.Models;

namespace RecipeAndMealTracker.Services;

/// <summary>
/// All pantry reads and writes live here so the Razor components stay presentational.
/// Every call opens its own short-lived context from the factory, which is what
/// interactive Blazor Server needs — a single injected DbContext would be shared
/// across overlapping renders.
/// </summary>
public class PantryService(IDbContextFactory<AppDbContext> contextFactory)
{
    public async Task<List<PantryItem>> GetItemsAsync(
        PantryQuery? query = null,
        CancellationToken cancellationToken = default)
    {
        query ??= new PantryQuery();

        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        var items = db.PantryItems.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            items = items.Where(i =>
                i.Name.Contains(term) ||
                (i.Notes != null && i.Notes.Contains(term)) ||
                (i.StorageLocation != null && i.StorageLocation.Contains(term)));
        }

        if (query.Category is { } category)
        {
            items = items.Where(i => i.Category == category);
        }

        var today = Today;
        var cutoff = today.AddDays(PantryItem.ExpiringSoonWindowDays);

        // Mirrors PantryItem.Status, expressed so SQL Server can do the filtering.
        items = query.Filter switch
        {
            StockFilter.InStock => items.Where(i => i.Quantity > 0 && i.Quantity > i.ReorderThreshold),
            StockFilter.LowStock => items.Where(i => i.Quantity > 0 && i.Quantity <= i.ReorderThreshold),
            StockFilter.OutOfStock => items.Where(i => i.Quantity <= 0),
            StockFilter.NeedsRestock => items.Where(i => i.Quantity <= i.ReorderThreshold),
            StockFilter.ExpiringSoon => items.Where(
                i => i.ExpiresOn != null && i.ExpiresOn >= today && i.ExpiresOn <= cutoff),
            _ => items
        };

        items = query.Sort switch
        {
            PantrySort.NameDescending => items.OrderByDescending(i => i.Name),
            PantrySort.QuantityAscending => items.OrderBy(i => i.Quantity).ThenBy(i => i.Name),
            PantrySort.QuantityDescending => items.OrderByDescending(i => i.Quantity).ThenBy(i => i.Name),
            PantrySort.Category => items.OrderBy(i => i.Category).ThenBy(i => i.Name),
            PantrySort.RecentlyUpdated => items.OrderByDescending(i => i.UpdatedAtUtc),
            _ => items.OrderBy(i => i.Name)
        };

        return await items.ToListAsync(cancellationToken);
    }

    public async Task<PantryItem?> GetItemAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await db.PantryItems
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<PantryStats> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        var today = Today;
        var cutoff = today.AddDays(PantryItem.ExpiringSoonWindowDays);

        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        var items = db.PantryItems.AsNoTracking();

        return new PantryStats(
            TotalItems: await items.CountAsync(cancellationToken),
            LowStockCount: await items.CountAsync(
                i => i.Quantity > 0 && i.Quantity <= i.ReorderThreshold, cancellationToken),
            OutOfStockCount: await items.CountAsync(
                i => i.Quantity <= 0, cancellationToken),
            ExpiringSoonCount: await items.CountAsync(
                i => i.ExpiresOn != null && i.ExpiresOn >= today && i.ExpiresOn <= cutoff,
                cancellationToken));
    }

    /// <summary>Items that need buying, worst first — the shopping-list view.</summary>
    public async Task<List<PantryItem>> GetRestockListAsync(
        int limit = 5,
        CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await db.PantryItems
            .AsNoTracking()
            .Where(i => i.Quantity <= i.ReorderThreshold)
            .OrderBy(i => i.Quantity)
            .ThenBy(i => i.Name)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task AddItemAsync(PantryItem item, CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);

        item.CreatedAtUtc = DateTime.UtcNow;
        item.UpdatedAtUtc = item.CreatedAtUtc;

        db.PantryItems.Add(item);
        await db.SaveChangesAsync(cancellationToken);
    }

    /// <returns><c>false</c> if the row no longer exists.</returns>
    public async Task<bool> UpdateItemAsync(PantryItem item, CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);

        var existing = await db.PantryItems.FirstOrDefaultAsync(i => i.Id == item.Id, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        existing.Name = item.Name;
        existing.Category = item.Category;
        existing.Quantity = item.Quantity;
        existing.Unit = item.Unit;
        existing.ReorderThreshold = item.ReorderThreshold;
        existing.StorageLocation = item.StorageLocation;
        existing.ExpiresOn = item.ExpiresOn;
        existing.Notes = item.Notes;
        existing.UpdatedAtUtc = DateTime.UtcNow;

        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <summary>
    /// Nudges stock up or down from the inventory table. Never lets a quantity go
    /// negative — "none left" is the floor.
    /// </summary>
    /// <returns>The saved item, or <c>null</c> if it no longer exists.</returns>
    public async Task<PantryItem?> AdjustQuantityAsync(
        int id,
        decimal delta,
        CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);

        var item = await db.PantryItems.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
        if (item is null)
        {
            return null;
        }

        var updated = decimal.Round(item.Quantity + delta, 3);
        item.Quantity = updated < 0 ? 0 : updated;
        item.UpdatedAtUtc = DateTime.UtcNow;

        await db.SaveChangesAsync(cancellationToken);
        return item;
    }

    /// <returns><c>false</c> if the row was already gone.</returns>
    public async Task<bool> DeleteItemAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);

        var item = await db.PantryItems.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
        if (item is null)
        {
            return false;
        }

        db.PantryItems.Remove(item);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow.Date);
}

/// <summary>The inventory page's search, filter and sort state.</summary>
public class PantryQuery
{
    public string? Search { get; set; }

    public PantryCategory? Category { get; set; }

    public StockFilter Filter { get; set; } = StockFilter.All;

    public PantrySort Sort { get; set; } = PantrySort.NameAscending;

    public bool HasFilters =>
        !string.IsNullOrWhiteSpace(Search) ||
        Category is not null ||
        Filter != StockFilter.All;

    public void Reset()
    {
        Search = null;
        Category = null;
        Filter = StockFilter.All;
        Sort = PantrySort.NameAscending;
    }
}

/// <summary>
/// The stock views the inventory page offers. <see cref="NeedsRestock"/> spans both
/// low and empty items, which is what the shopping list cares about.
/// </summary>
public enum StockFilter
{
    All,
    InStock,
    LowStock,
    OutOfStock,
    NeedsRestock,
    ExpiringSoon
}

public enum PantrySort
{
    NameAscending,
    NameDescending,
    QuantityAscending,
    QuantityDescending,
    Category,
    RecentlyUpdated
}

public record PantryStats(
    int TotalItems,
    int LowStockCount,
    int OutOfStockCount,
    int ExpiringSoonCount)
{
    public static PantryStats Empty { get; } = new(0, 0, 0, 0);

    public int NeedsAttentionCount => LowStockCount + OutOfStockCount;
}

/// <summary>Wording for the inventory page's filter and sort dropdowns.</summary>
public static class PantryQueryDisplay
{
    public static string Label(StockFilter filter) => filter switch
    {
        StockFilter.InStock => "In stock",
        StockFilter.LowStock => "Low stock",
        StockFilter.OutOfStock => "Out of stock",
        StockFilter.NeedsRestock => "Needs restocking",
        StockFilter.ExpiringSoon => "Expiring soon",
        _ => "All items"
    };

    public static string Label(PantrySort sort) => sort switch
    {
        PantrySort.NameDescending => "Name (Z–A)",
        PantrySort.QuantityAscending => "Quantity (low to high)",
        PantrySort.QuantityDescending => "Quantity (high to low)",
        PantrySort.Category => "Category",
        PantrySort.RecentlyUpdated => "Recently updated",
        _ => "Name (A–Z)"
    };
}
