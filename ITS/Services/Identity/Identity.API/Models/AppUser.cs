using Microsoft.AspNetCore.Identity;

namespace Identity.API.Models;

public class AppUser : IdentityUser
{
    public string DisplayName { get; set; }
    public string Email { get; set; }

}
