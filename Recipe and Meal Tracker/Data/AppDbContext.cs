using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
// using RecipeAndMealTracker.Models;

namespace RecipeAndMealTracker.Data;

public sealed class AppDbContext(
    DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Recipe> Recipes { get; set; }
}