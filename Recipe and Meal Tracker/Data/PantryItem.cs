using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeAndMealTracker.Models;

/// <summary>
/// A single ingredient the household keeps on hand. Stock levels here are what
/// the pantry dashboard reports on, so <see cref="ReorderThreshold"/> is what
/// separates "plenty left" from "add it to the shopping list".
/// </summary>
public class PantryItem
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Please enter an item name.")]
    [MaxLength(120)]
    [Display(Name = "Item name")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Category")]
    public PantryCategory Category { get; set; } = PantryCategory.Other;

    [Range(0, 100000, ErrorMessage = "Quantity must be between 0 and 100,000.")]
    [Display(Name = "Quantity on hand")]
    public decimal Quantity { get; set; }

    [Display(Name = "Unit")]
    public StockUnit Unit { get; set; } = StockUnit.Each;

    [Range(0, 100000, ErrorMessage = "The reorder point must be between 0 and 100,000.")]
    [Display(Name = "Reorder at or below")]
    public decimal ReorderThreshold { get; set; }

    [MaxLength(60)]
    [Display(Name = "Stored in")]
    public string? StorageLocation { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Best before")]
    public DateOnly? ExpiresOn { get; set; }

    [MaxLength(400)]
    [Display(Name = "Notes")]
    public string? Notes { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    /// <summary>How many days before the best-before date an item counts as "use it soon".</summary>
    public const int ExpiringSoonWindowDays = 7;

    /// <remarks>
    /// Derived rather than stored so it can never drift out of sync with the quantity.
    /// PantryService repeats this rule as a SQL-translatable predicate when it filters
    /// by status — keep the two in step.
    /// </remarks>
    [NotMapped]
    public StockStatus Status => Quantity <= 0
        ? StockStatus.OutOfStock
        : Quantity <= ReorderThreshold
            ? StockStatus.LowStock
            : StockStatus.InStock;

    [NotMapped]
    public bool IsExpired =>
        ExpiresOn is { } expires && expires < DateOnly.FromDateTime(DateTime.UtcNow.Date);

    [NotMapped]
    public bool IsExpiringSoon
    {
        get
        {
            if (ExpiresOn is not { } expires)
            {
                return false;
            }

            var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);
            return expires >= today && expires <= today.AddDays(ExpiringSoonWindowDays);
        }
    }
}

public enum PantryCategory
{
    Produce,
    Meat,
    Seafood,
    Dairy,
    Grains,
    Baking,
    Spices,
    Canned,
    Frozen,
    Beverages,
    Condiments,
    Other
}

public enum StockUnit
{
    Each,
    Gram,
    Kilogram,
    Ounce,
    Pound,
    Milliliter,
    Liter,
    Cup,
    Tablespoon,
    Teaspoon,
    Can,
    Package
}

public enum StockStatus
{
    InStock,
    LowStock,
    OutOfStock
}
