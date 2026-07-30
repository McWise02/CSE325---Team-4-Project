using Microsoft.EntityFrameworkCore;
using RecipeAndMealTracker.Models;

namespace RecipeAndMealTracker.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<Recipe> Recipes => Set<Recipe>();
}