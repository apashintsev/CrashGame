using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;
using TestApp.Models;
using TestApp.Models.Auth;

namespace TestApp.Services.Auth
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public UserService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<AppUser> AuthenticateUserAsync(AuthenticateRequest request)
        {
            var result = await _signInManager.PasswordSignInAsync(request.Email, request.Password, true, false);
            if (result.Succeeded)
            {
                return await _userManager.FindByEmailAsync(request.Email);
            }
            else
            {
                return null;
            }
        }

        public string GetUserId(ClaimsPrincipal user)
        {
            return _userManager.GetUserId(user);
        }
    }
}
