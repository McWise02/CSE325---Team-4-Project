using Microsoft.AspNetCore.Identity;

namespace RecipeAndMealTracker.Data;

public sealed class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }
}