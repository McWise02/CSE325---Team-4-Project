namespace RecipeAndMealTracker.Models;

/// <summary>
/// Turns the pantry enums into the wording the UI shows. Kept out of the entity so
/// the model stays about storage and this stays about presentation.
/// </summary>
public static class PantryDisplay
{
    public static string Label(PantryCategory category) => category switch
    {
        PantryCategory.Produce => "Produce",
        PantryCategory.Meat => "Meat & poultry",
        PantryCategory.Seafood => "Seafood",
        PantryCategory.Dairy => "Dairy & eggs",
        PantryCategory.Grains => "Grains & pasta",
        PantryCategory.Baking => "Baking",
        PantryCategory.Spices => "Herbs & spices",
        PantryCategory.Canned => "Canned & jarred",
        PantryCategory.Frozen => "Frozen",
        PantryCategory.Beverages => "Beverages",
        PantryCategory.Condiments => "Condiments & oils",
        _ => "Other"
    };

    public static string Label(StockUnit unit) => unit switch
    {
        StockUnit.Each => "each",
        StockUnit.Gram => "grams",
        StockUnit.Kilogram => "kilograms",
        StockUnit.Ounce => "ounces",
        StockUnit.Pound => "pounds",
        StockUnit.Milliliter => "millilitres",
        StockUnit.Liter => "litres",
        StockUnit.Cup => "cups",
        StockUnit.Tablespoon => "tablespoons",
        StockUnit.Teaspoon => "teaspoons",
        StockUnit.Can => "cans",
        _ => "packages"
    };

    /// <summary>Short form used in dense places such as the inventory table.</summary>
    public static string Abbreviation(StockUnit unit) => unit switch
    {
        StockUnit.Each => "ea",
        StockUnit.Gram => "g",
        StockUnit.Kilogram => "kg",
        StockUnit.Ounce => "oz",
        StockUnit.Pound => "lb",
        StockUnit.Milliliter => "ml",
        StockUnit.Liter => "L",
        StockUnit.Cup => "cup",
        StockUnit.Tablespoon => "tbsp",
        StockUnit.Teaspoon => "tsp",
        StockUnit.Can => "can",
        _ => "pkg"
    };

    public static string Label(StockStatus status) => status switch
    {
        StockStatus.InStock => "In stock",
        StockStatus.LowStock => "Low stock",
        _ => "Out of stock"
    };

    /// <summary>Maps a status onto the tone classes defined in <c>app.css</c>.</summary>
    public static string ToneClass(StockStatus status) => status switch
    {
        StockStatus.InStock => "tone-ok",
        StockStatus.LowStock => "tone-warn",
        _ => "tone-danger"
    };

    /// <summary>
    /// A sensible nudge for the +/- buttons: grams and millilitres move in bigger
    /// jumps than eggs do, so a flat step of 1 would be useless for both.
    /// </summary>
    public static decimal StepFor(StockUnit unit) => unit switch
    {
        StockUnit.Gram or StockUnit.Milliliter => 50m,
        StockUnit.Kilogram or StockUnit.Liter => 0.5m,
        StockUnit.Ounce or StockUnit.Cup => 0.5m,
        StockUnit.Pound => 0.5m,
        StockUnit.Tablespoon or StockUnit.Teaspoon => 1m,
        _ => 1m
    };

    /// <summary>Drops trailing zeros so "2.000 kg" reads as "2 kg".</summary>
    public static string Quantity(decimal quantity)
    {
        var trimmed = decimal.Round(quantity, 3).ToString("0.###");
        return trimmed.Length == 0 ? "0" : trimmed;
    }

    public static string QuantityWithUnit(PantryItem item) =>
        $"{Quantity(item.Quantity)} {Abbreviation(item.Unit)}";
}
