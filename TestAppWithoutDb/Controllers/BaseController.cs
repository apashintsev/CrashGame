using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TestApp.Models;

namespace TestApp.Controllers
{
    [Route("api/[controller]/[action]")]
    public abstract class BaseController : Controller
    {
        protected readonly UserManager<AppUser> _userManager;
        protected readonly SignInManager<AppUser> _signInManager;

        protected BaseController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        protected async Task<AppUser> GetCurrentUser() => await _userManager.FindByIdAsync(User.Identity.Name);

        protected string GetUserId() => User.Identity.Name;

        protected IActionResult ReturnBadRequest(ILogger _logger, Exception e)
        {
            _logger.LogError("Get Property Failed " + e);
            return BadRequest(e.Message);
        }
    }
}
