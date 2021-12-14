using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TestApp.Models;
using TestApp.Models.Auth;

namespace TestApp.Services.Auth
{
    public interface IUserService
    {
        Task<AppUser> AuthenticateUserAsync(AuthenticateRequest request);
        string GetUserId(ClaimsPrincipal user);
    }
}
