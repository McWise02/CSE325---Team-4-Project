using System.ComponentModel.DataAnnotations;

namespace RecipeAndMealTracker.Models;

public class Recipe
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public string? Instructions { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}