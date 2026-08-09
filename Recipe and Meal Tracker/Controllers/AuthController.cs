using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RecipeAndMealTracker.Data;

namespace RecipeAndMealTracker.Controllers;

[AllowAnonymous]
[Route("auth")]
public class AuthController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost("register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(
        [FromForm] RegisterRequest request)
    {
        var email = request.Email?.Trim();
        var userName = request.UserName?.Trim();


        if (string.IsNullOrWhiteSpace(email))
        {
            return RedirectWithError(
                "/register",
                "Email is required.");
        }

        if (string.IsNullOrWhiteSpace(userName))
        {
            return RedirectWithError(
                "/register",
                "Username is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return RedirectWithError(
                "/register",
                "Password is required.");
        }

        if (request.Password != request.ConfirmPassword)
        {
            return RedirectWithError(
                "/register",
                "Passwords do not match.");
        }

        var existingUser =
            await _userManager.FindByEmailAsync(email);

        if (existingUser is not null)
        {
            return RedirectWithError(
                "/register",
                "An account with that email already exists.");
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email
        };

        var result =
            await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errorMessage = string.Join(
                " ",
                result.Errors.Select(error => error.Description));

            return RedirectWithError(
                "/register",
                errorMessage);
        }

        await _signInManager.SignInAsync(
            user,
            isPersistent: false);

        return LocalRedirect("/");
    }

    [HttpPost("login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        [FromForm] LoginRequest request)
    {
        var email = request.Email?.Trim();

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return RedirectWithError(
                "/login",
                "Email and password are required.");
        }

        var result =
            await _signInManager.PasswordSignInAsync(
                userName: email,
                password: request.Password,
                isPersistent: request.RememberMe,
                lockoutOnFailure: true);

        if (result.Succeeded)
        {
            return LocalRedirect(
                GetSafeReturnUrl(request.ReturnUrl));
        }

        if (result.IsLockedOut)
        {
            return RedirectWithError(
                "/login",
                "Your account is temporarily locked.");
        }

        return RedirectWithError(
            "/login",
            "Invalid email or password.");
    }

    [Authorize]
    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        return LocalRedirect("/");
    }

    private IActionResult RedirectWithError(
        string page,
        string error)
    {
        return LocalRedirect(
            $"{page}?error={Uri.EscapeDataString(error)}");
    }

    private static string GetSafeReturnUrl(string? returnUrl)
    {
        if (string.IsNullOrWhiteSpace(returnUrl))
        {
            return "/dashboard";
        }

        if (!returnUrl.StartsWith('/') ||
            returnUrl.StartsWith("//") ||
            returnUrl.StartsWith("/\\"))
        {
            return "/dashboard";
        }

        return returnUrl;
    }
}

public class RegisterRequest
{
    public string? Email { get; set; }

    public string? UserName { get; set; }

    public string? Password { get; set; }

    public string? ConfirmPassword { get; set; }
}

public class LoginRequest
{
    public string? Email { get; set; }

    public string? Password { get; set; }

    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}