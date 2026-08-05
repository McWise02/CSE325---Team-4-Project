using Microsoft.EntityFrameworkCore;
using RecipeAndMealTracker.Models;

namespace RecipeAndMealTracker.Data;

/// <summary>
/// Brings the database up to date at startup and drops in a starter pantry the first
/// time it runs, so a fresh clone has something to show. A database that cannot be
/// reached is logged and skipped rather than thrown — the site still needs to boot.
/// </summary>
public static class SeedDatabase
{
    public static async Task InitializeAsync(
        IServiceProvider services,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        var contextFactory = services.GetRequiredService<IDbContextFactory<AppDbContext>>();

        try
        {
            await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);

            if (!await db.Database.CanConnectAsync(cancellationToken))
            {
                logger.LogWarning(
                    "Could not connect to the database. Check the DefaultConnection value in your .env file and the Azure SQL firewall rules.");
                return;
            }

            logger.LogInformation("Connected to the database.");

            await db.Database.MigrateAsync(cancellationToken);

            if (await db.PantryItems.AnyAsync(cancellationToken))
            {
                return;
            }

            db.PantryItems.AddRange(BuildStarterPantry());
            await db.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Seeded the starter pantry inventory.");
        }
        catch (Exception ex)
        {
            // Startup should survive a database that is asleep, unreachable or still
            // being provisioned; the pages surface the problem on their own.
            logger.LogError(ex, "Database initialisation failed. The app will start without seeding.");
        }
    }

    /// <summary>
    /// A deliberately mixed shelf: some items well stocked, some at their reorder
    /// point, one empty and a couple nearing their best-before date, so every state
    /// the dashboard can report is visible straight away.
    /// </summary>
    private static IEnumerable<PantryItem> BuildStarterPantry()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);

        return
        [
            new PantryItem
            {
                Name = "All-purpose flour",
                Category = PantryCategory.Baking,
                Quantity = 2.5m,
                Unit = StockUnit.Kilogram,
                ReorderThreshold = 1m,
                StorageLocation = "Pantry shelf",
                Notes = "Keep sealed in the airtight tub."
            },
            new PantryItem
            {
                Name = "Large eggs",
                Category = PantryCategory.Dairy,
                Quantity = 4m,
                Unit = StockUnit.Each,
                ReorderThreshold = 6m,
                StorageLocation = "Fridge door",
                ExpiresOn = today.AddDays(5)
            },
            new PantryItem
            {
                Name = "Whole milk",
                Category = PantryCategory.Dairy,
                Quantity = 1.5m,
                Unit = StockUnit.Liter,
                ReorderThreshold = 1m,
                StorageLocation = "Fridge",
                ExpiresOn = today.AddDays(3)
            },
            new PantryItem
            {
                Name = "Olive oil",
                Category = PantryCategory.Condiments,
                Quantity = 0m,
                Unit = StockUnit.Milliliter,
                ReorderThreshold = 250m,
                StorageLocation = "Pantry shelf",
                Notes = "Ran out mid-recipe — buy the large bottle."
            },
            new PantryItem
            {
                Name = "Spaghetti",
                Category = PantryCategory.Grains,
                Quantity = 900m,
                Unit = StockUnit.Gram,
                ReorderThreshold = 400m,
                StorageLocation = "Pantry shelf"
            },
            new PantryItem
            {
                Name = "Basmati rice",
                Category = PantryCategory.Grains,
                Quantity = 3m,
                Unit = StockUnit.Kilogram,
                ReorderThreshold = 1m,
                StorageLocation = "Pantry shelf"
            },
            new PantryItem
            {
                Name = "Chopped tomatoes",
                Category = PantryCategory.Canned,
                Quantity = 5m,
                Unit = StockUnit.Can,
                ReorderThreshold = 3m,
                StorageLocation = "Pantry shelf"
            },
            new PantryItem
            {
                Name = "Chicken breast",
                Category = PantryCategory.Meat,
                Quantity = 1.2m,
                Unit = StockUnit.Pound,
                ReorderThreshold = 1m,
                StorageLocation = "Freezer",
                ExpiresOn = today.AddDays(45)
            },
            new PantryItem
            {
                Name = "Baby spinach",
                Category = PantryCategory.Produce,
                Quantity = 150m,
                Unit = StockUnit.Gram,
                ReorderThreshold = 100m,
                StorageLocation = "Fridge crisper",
                ExpiresOn = today.AddDays(2),
                Notes = "Use in the pasta bake before it turns."
            },
            new PantryItem
            {
                Name = "Yellow onions",
                Category = PantryCategory.Produce,
                Quantity = 6m,
                Unit = StockUnit.Each,
                ReorderThreshold = 3m,
                StorageLocation = "Vegetable basket"
            },
            new PantryItem
            {
                Name = "Garlic",
                Category = PantryCategory.Produce,
                Quantity = 2m,
                Unit = StockUnit.Each,
                ReorderThreshold = 2m,
                StorageLocation = "Vegetable basket",
                Notes = "Whole bulbs."
            },
            new PantryItem
            {
                Name = "Smoked paprika",
                Category = PantryCategory.Spices,
                Quantity = 45m,
                Unit = StockUnit.Gram,
                ReorderThreshold = 15m,
                StorageLocation = "Spice rack"
            },
            new PantryItem
            {
                Name = "Ground cinnamon",
                Category = PantryCategory.Spices,
                Quantity = 10m,
                Unit = StockUnit.Gram,
                ReorderThreshold = 15m,
                StorageLocation = "Spice rack"
            },
            new PantryItem
            {
                Name = "Frozen peas",
                Category = PantryCategory.Frozen,
                Quantity = 500m,
                Unit = StockUnit.Gram,
                ReorderThreshold = 200m,
                StorageLocation = "Freezer drawer"
            },
            new PantryItem
            {
                Name = "Greek yoghurt",
                Category = PantryCategory.Dairy,
                Quantity = 500m,
                Unit = StockUnit.Gram,
                ReorderThreshold = 250m,
                StorageLocation = "Fridge",
                ExpiresOn = today.AddDays(9)
            },
            new PantryItem
            {
                Name = "Salmon fillets",
                Category = PantryCategory.Seafood,
                Quantity = 0m,
                Unit = StockUnit.Each,
                ReorderThreshold = 2m,
                StorageLocation = "Freezer",
                Notes = "Needed for Friday's meal plan."
            },
            new PantryItem
            {
                Name = "Orange juice",
                Category = PantryCategory.Beverages,
                Quantity = 1m,
                Unit = StockUnit.Liter,
                ReorderThreshold = 1m,
                StorageLocation = "Fridge",
                ExpiresOn = today.AddDays(6)
            },
            new PantryItem
            {
                Name = "Baking powder",
                Category = PantryCategory.Baking,
                Quantity = 120m,
                Unit = StockUnit.Gram,
                ReorderThreshold = 50m,
                StorageLocation = "Pantry shelf"
            }
        ];
    }
}
