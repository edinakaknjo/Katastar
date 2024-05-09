using Microsoft.AspNetCore.Identity;
using Katastar.Models;

namespace Katastar.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

    }
}