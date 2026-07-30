using Recipe_and_Meal_Tracker.Components;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using RecipeAndMealTracker.Data;

Env.Load();
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


var connectionString = Environment.GetEnvironmentVariable("DefaultConnection");

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();



using (var scope = app.Services.CreateScope()) {
         var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
         if (dbContext.Database.CanConnect())     {
            Console.WriteLine("✅ Connection to Azure SQL succeeded!");
                 }
        else    {Console.WriteLine("❌ Connection failed! Check your .env file or Azure Firewall.");}
         }

app.Run();
