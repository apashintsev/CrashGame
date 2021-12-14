using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TestApp.Models
{
    public class AppUser : IdentityUser
    {
        public decimal Balance { get; set; }
    }
}
