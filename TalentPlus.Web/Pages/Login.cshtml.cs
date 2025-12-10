using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TalentPlus.Infraestructure.Identity;

namespace TalentPlus.Web.Pages;

public class LoginModel : PageModel
{
    private readonly SignInManager<AppUser> _signInManager;

    public LoginModel(SignInManager<AppUser> signInManager)
    {
        _signInManager = signInManager;
    }

    // I define the structure of the form data
    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";
    }

    public void OnGet()
    {
        // If the user loads the page, I just show the empty form
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        // I attempt to sign the user in using Cookies (PasswordSignInAsync)
        var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, false, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            // If login is successful, I redirect them to the Dashboard
            return RedirectToPage("/Dashboard");
        }
        else
        {
            // If failed, I show an error message on the page
            ModelState.AddModelError(string.Empty, "Intento de inicio de sesión inválido.");
            return Page();
        }
    }
        
    // I create a method to handle Logout
    public async Task<IActionResult> OnGetLogoutAsync()
    {
        await _signInManager.SignOutAsync();
        return RedirectToPage("/Login");
    }
}