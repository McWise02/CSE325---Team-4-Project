using Microsoft.EntityFrameworkCore;
using RecipeAndMealTracker.Models;

namespace RecipeAndMealTracker.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<Recipe> Recipes => Set<Recipe>();

    public DbSet<PantryItem> PantryItems => Set<PantryItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PantryItem>(entity =>
        {
            // Recipes work in fractions (0.25 cups), so quantities need a scale.
            entity.Property(i => i.Quantity).HasPrecision(9, 3);
            entity.Property(i => i.ReorderThreshold).HasPrecision(9, 3);

            // The inventory list sorts and searches by name by default.
            entity.HasIndex(i => i.Name);
        });
    }
}