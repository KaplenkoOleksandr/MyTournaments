using Microsoft.AspNetCore.Identity;

namespace MyTournaments.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
    }
}