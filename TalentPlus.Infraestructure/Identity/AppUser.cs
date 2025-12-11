using Microsoft.AspNetCore.Identity;

namespace TalentPlus.Infraestructure.Identity;

// I extend IdentityUser to add extra fields to the base login user if needed later
public class AppUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}