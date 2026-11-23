using Microsoft.AspNetCore.Identity;

namespace MvcMusicStore.Models
{
    /// <summary>
    /// Application user extending ASP.NET Core Identity IdentityUser.
    /// Replaces the legacy UserProfile / SimpleMembership schema.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        // Add custom profile properties here if needed
    }
}
