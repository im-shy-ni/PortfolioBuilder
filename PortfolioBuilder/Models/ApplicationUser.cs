
using Microsoft.AspNetCore.Identity;

namespace PortfolioBuilder.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? Bio { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}
