using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RecipeAndMealTracker.Data;
namespace RecipeAndMealTracker.Models;

public class Ingredient
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public double? Amount { get; set; }

    public string? UnitId { get; set; }

    [ForeignKey(nameof(UnitId))]
    public MeasurementUnit? Unit { get; set; }

    public int RecipeId { get; set; }

    [ForeignKey(nameof(RecipeId))]
    public Recipe? Recipe { get; set; }

    public decimal? Price { get; set; }
}