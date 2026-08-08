using Microsoft.EntityFrameworkCore;
using RecipeAndMealTracker.Models;

namespace RecipeAndMealTracker.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<MealEntry> MealEntries => Set<MealEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MealEntry>()
            .HasOne(m => m.Recipe)
            .WithMany()
            .HasForeignKey(m => m.RecipeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Speeds up the calendar queries (fetch all entries for a date range + meal type)
        modelBuilder.Entity<MealEntry>()
            .HasIndex(m => new { m.Date, m.MealType });
    }
}