using Recipe_and_Meal_Tracker.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using RecipeAndMealTracker.Data;
using Microsoft.AspNetCore.Identity;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' was not found.");

builder.Services.AddControllers();
builder.Services.AddAuthorization();

builder.Services.AddDbContextFactory<AppDbContext>(options =>
{
    options.UseSqlServer(
        connectionString,
        sqlServerOptions =>
        {
            sqlServerOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        });
});



builder.Services.AddCascadingAuthenticationState();
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme =
            IdentityConstants.ApplicationScheme;

        options.DefaultSignInScheme =
            IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;

        options.Password.RequiredLength = 10;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;

        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan =
            TimeSpan.FromMinutes(15);
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();



builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();


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

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapControllers();
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
