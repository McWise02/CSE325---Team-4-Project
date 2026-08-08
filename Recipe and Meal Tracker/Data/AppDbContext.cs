using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecipeAndMealTracker.Models;

namespace RecipeAndMealTracker.Data;

public sealed class AppDbContext(
    DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Recipe> Recipes { get; set; }
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