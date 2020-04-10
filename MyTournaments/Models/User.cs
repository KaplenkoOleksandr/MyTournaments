using Microsoft.AspNetCore.Identity;

namespace MyTournaments.Models
{
    public class User : IdentityUser
    {
        public int Year { get; set; }
    }
}