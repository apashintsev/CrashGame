using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TestApp.Models;
using TestApp.Models.Auth;
using TestApp.Options;
using TestApp.Services.Auth;

namespace TestApp.Controllers
{
    [AllowAnonymous]
    public class AuthController : BaseController
    {
        private readonly IUserService _userService;
        private readonly AppSettings _appSettings;

        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            IUserService userService, IOptions<AppSettings> appSettings) : base(userManager, signInManager)
        {
            _userService = userService;
            _appSettings = appSettings.Value;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] AuthenticateRequest model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser { Email = model.Email, UserName = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return Ok();
                }
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values.SelectMany(it => it.Errors).Select(it => it.ErrorMessage));
            var user = await _userService.AuthenticateUserAsync(request);
            if (user != null)
            {
                return Ok(GenerateUserToken(user));
            }
            else
            {
                return Unauthorized();
            }
        }

        private UserToken GenerateUserToken(AppUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_appSettings.AuthenticationJwtSecret);

            var expires = DateTime.UtcNow.AddDays(7);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(JwtRegisteredClaimNames.NameId, user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                }),
                Expires = expires,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);

            var token = tokenHandler.WriteToken(securityToken);

            return new UserToken
            {
                UserId = user.Id,
                Email = user.Email,
                Token = token,
                Expires = expires
            };
        }
    }
}
