using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RecipeAndMealTracker.Models;

namespace RecipeAndMealTracker.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(
        AppDbContext db,
        UserManager<ApplicationUser> userManager)
    {
        Console.WriteLine("Starting database seed...");

        // -------------------------------------------------
        // 1. Create tester user
        // -------------------------------------------------

        const string email = "tester@test.com";
        const string password = "TestPassword!23";

        var user = await userManager.FindByEmailAsync(email);

        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                DisplayName = "Tester",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(x => x.Description));

                throw new Exception(
                    $"Could not create tester user: {errors}");
            }

            Console.WriteLine("Created tester@test.com");
        }
        else
        {
            Console.WriteLine("Tester user already exists.");
        }


        // -------------------------------------------------
        // 2. Measurement units
        // -------------------------------------------------

        var measurements = new List<MeasurementUnit>
        {
            new()
            {
                Id = "grams",
                Name = "Grams"
            },
            new()
            {
                Id = "kilograms",
                Name = "Kilograms"
            },
            new()
            {
                Id = "millilitres",
                Name = "Millilitres"
            },
            new()
            {
                Id = "litres",
                Name = "Litres"
            },
            new()
            {
                Id = "teaspoons",
                Name = "Teaspoons"
            },
            new()
            {
                Id = "tablespoons",
                Name = "Tablespoons"
            },
            new()
            {
                Id = "cups",
                Name = "Cups"
            },
            new()
            {
                Id = "pieces",
                Name = "Pieces"
            },
            new()
            {
                Id = "slices",
                Name = "Slices"
            },
            new()
            {
                Id = "cloves",
                Name = "Cloves"
            },
            new()
            {
                Id = "cans",
                Name = "Cans"
            }
        };

        foreach (var measurement in measurements)
        {
            var exists = await db.MeasurementUnits
                .AnyAsync(x => x.Id == measurement.Id);

            if (!exists)
            {
                db.MeasurementUnits.Add(measurement);
            }
        }

        await db.SaveChangesAsync();

        Console.WriteLine("Measurement units seeded.");


        // -------------------------------------------------
        // 3. Recipes
        // -------------------------------------------------

        var oatmeal = await CreateRecipe(
            db,
            user.Id,
            "Banana Oatmeal",
            """
            Add the oats and milk to a saucepan.
            Cook until thickened.
            Slice the banana and place on top.
            """,
            [
                new Ingredient
                {
                    Name = "Oats",
                    Amount = 80,
                    UnitId = "grams",
                    Price = 0.80m
                },
                new Ingredient
                {
                    Name = "Milk",
                    Amount = 250,
                    UnitId = "millilitres",
                    Price = 0.70m
                },
                new Ingredient
                {
                    Name = "Banana",
                    Amount = 1,
                    UnitId = "pieces",
                    Price = 0.35m
                }
            ]);


        var chickenRice = await CreateRecipe(
            db,
            user.Id,
            "Chicken Rice Bowl",
            """
            Cook the rice.
            Season and cook the chicken.
            Serve the chicken over rice with vegetables.
            """,
            [
                new Ingredient
                {
                    Name = "Chicken Breast",
                    Amount = 200,
                    UnitId = "grams",
                    Price = 2.50m
                },
                new Ingredient
                {
                    Name = "Rice",
                    Amount = 100,
                    UnitId = "grams",
                    Price = 0.45m
                },
                new Ingredient
                {
                    Name = "Mixed Vegetables",
                    Amount = 150,
                    UnitId = "grams",
                    Price = 1.20m
                },
                new Ingredient
                {
                    Name = "Olive Oil",
                    Amount = 1,
                    UnitId = "tablespoons",
                    Price = 0.15m
                }
            ]);


        var spaghetti = await CreateRecipe(
            db,
            user.Id,
            "Spaghetti Bolognese",
            """
            Cook the spaghetti.
            Brown the beef with the onion and garlic.
            Add tomatoes and simmer.
            Serve the sauce over the spaghetti.
            """,
            [
                new Ingredient
                {
                    Name = "Spaghetti",
                    Amount = 120,
                    UnitId = "grams",
                    Price = 0.60m
                },
                new Ingredient
                {
                    Name = "Beef Mince",
                    Amount = 200,
                    UnitId = "grams",
                    Price = 2.40m
                },
                new Ingredient
                {
                    Name = "Chopped Tomatoes",
                    Amount = 1,
                    UnitId = "cans",
                    Price = 0.85m
                },
                new Ingredient
                {
                    Name = "Garlic",
                    Amount = 2,
                    UnitId = "cloves",
                    Price = 0.20m
                },
                new Ingredient
                {
                    Name = "Onion",
                    Amount = 1,
                    UnitId = "pieces",
                    Price = 0.40m
                }
            ]);


        var eggsToast = await CreateRecipe(
            db,
            user.Id,
            "Eggs on Toast",
            """
            Toast the bread.
            Cook the eggs.
            Serve the eggs on top of the toast.
            """,
            [
                new Ingredient
                {
                    Name = "Eggs",
                    Amount = 2,
                    UnitId = "pieces",
                    Price = 0.70m
                },
                new Ingredient
                {
                    Name = "Bread",
                    Amount = 2,
                    UnitId = "slices",
                    Price = 0.30m
                },
                new Ingredient
                {
                    Name = "Butter",
                    Amount = 10,
                    UnitId = "grams",
                    Price = 0.15m
                }
            ]);


        var tunaPasta = await CreateRecipe(
            db,
            user.Id,
            "Tuna Pasta",
            """
            Cook the pasta.
            Drain the tuna.
            Mix the tuna, pasta and tomatoes together.
            Season and serve.
            """,
            [
                new Ingredient
                {
                    Name = "Pasta",
                    Amount = 120,
                    UnitId = "grams",
                    Price = 0.55m
                },
                new Ingredient
                {
                    Name = "Tuna",
                    Amount = 1,
                    UnitId = "cans",
                    Price = 1.30m
                },
                new Ingredient
                {
                    Name = "Cherry Tomatoes",
                    Amount = 100,
                    UnitId = "grams",
                    Price = 0.90m
                },
                new Ingredient
                {
                    Name = "Olive Oil",
                    Amount = 1,
                    UnitId = "tablespoons",
                    Price = 0.15m
                }
            ]);


        var vegetableCurry = await CreateRecipe(
            db,
            user.Id,
            "Vegetable Curry",
            """
            Chop and cook the vegetables.
            Add curry seasoning and coconut milk.
            Simmer until the vegetables are tender.
            Serve with rice.
            """,
            [
                new Ingredient
                {
                    Name = "Rice",
                    Amount = 100,
                    UnitId = "grams",
                    Price = 0.45m
                },
                new Ingredient
                {
                    Name = "Mixed Vegetables",
                    Amount = 250,
                    UnitId = "grams",
                    Price = 1.60m
                },
                new Ingredient
                {
                    Name = "Coconut Milk",
                    Amount = 1,
                    UnitId = "cans",
                    Price = 1.20m
                },
                new Ingredient
                {
                    Name = "Curry Powder",
                    Amount = 2,
                    UnitId = "teaspoons",
                    Price = 0.20m
                },
                new Ingredient
                {
                    Name = "Garlic",
                    Amount = 2,
                    UnitId = "cloves",
                    Price = 0.20m
                }
            ]);

        Console.WriteLine("Recipes seeded.");


        // -------------------------------------------------
        // 4. Meal plan - August 10, 2026
        // -------------------------------------------------

        var august10 = new DateOnly(2026, 8, 10);

        await CreateMealEntry(
            db,
            user.Id,
            august10,
            MealType.Breakfast,
            oatmeal.Id);

        await CreateMealEntry(
            db,
            user.Id,
            august10,
            MealType.Lunch,
            chickenRice.Id);

        await CreateMealEntry(
            db,
            user.Id,
            august10,
            MealType.Dinner,
            spaghetti.Id);


        // -------------------------------------------------
        // 5. Meal plan - August 11, 2026
        // -------------------------------------------------

        var august11 = new DateOnly(2026, 8, 11);

        await CreateMealEntry(
            db,
            user.Id,
            august11,
            MealType.Breakfast,
            eggsToast.Id);

        await CreateMealEntry(
            db,
            user.Id,
            august11,
            MealType.Lunch,
            tunaPasta.Id);

        await CreateMealEntry(
            db,
            user.Id,
            august11,
            MealType.Dinner,
            vegetableCurry.Id);

        Console.WriteLine("Meal entries seeded.");

        Console.WriteLine();
        Console.WriteLine("================================");
        Console.WriteLine("Database seed complete!");
        Console.WriteLine("================================");
        Console.WriteLine("Email: tester@test.com");
        Console.WriteLine("Password: TestPassword!23");
        Console.WriteLine("================================");
    }


    // -------------------------------------------------
    // Create recipe if it doesn't already exist
    // -------------------------------------------------

    private static async Task<Recipe> CreateRecipe(
        AppDbContext db,
        string userId,
        string name,
        string instructions,
        List<Ingredient> ingredients)
    {
        var existingRecipe = await db.Recipes
            .Include(x => x.Ingredients)
            .FirstOrDefaultAsync(x =>
                x.Name == name &&
                x.CreatedById == userId);

        if (existingRecipe != null)
        {
            return existingRecipe;
        }

        var recipe = new Recipe
        {
            Name = name,
            Instructions = instructions,
            CreatedById = userId,
            CreatedAtUtc = DateTime.UtcNow,
            Ingredients = ingredients
        };

        db.Recipes.Add(recipe);

        await db.SaveChangesAsync();

        return recipe;
    }


    // -------------------------------------------------
    // Create meal entry if it doesn't already exist
    // -------------------------------------------------

    private static async Task CreateMealEntry(
        AppDbContext db,
        string userId,
        DateOnly date,
        MealType mealType,
        int recipeId)
    {
        var exists = await db.MealEntries
            .AnyAsync(x =>
                x.UserId == userId &&
                x.Date == date &&
                x.MealType == mealType);

        if (exists)
        {
            return;
        }

        var meal = new MealEntry
        {
            UserId = userId,
            Date = date,
            MealType = mealType,
            RecipeId = recipeId,
            IsEaten = false,
            CreatedAtUtc = DateTime.UtcNow
        };

        db.MealEntries.Add(meal);

        await db.SaveChangesAsync();
    }
}