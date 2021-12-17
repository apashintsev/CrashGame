using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using TestApp.Models;
using TestApp.Models.Auth;
using TestApp.Options;
using TestApp.Services;
using TestApp.Services.Auth;

namespace TestApp.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CrashController : BaseController
    {
        private readonly IUserService _userService;
        private readonly ICrashService _crashService;
        public string CurrentUserId => _userService.GetUserId(User);

        public CrashController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            IUserService userService, ICrashService crashService) : base(userManager, signInManager)
        {
            _userService = userService;
            _crashService = crashService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGame()
        {
            return Ok(await _crashService.CreateGame());
        }

        [HttpPost]
        public async Task<IActionResult> MakeBet([FromBody] decimal bet)
        {
            var currentGame = await _crashService.GetCurrentGame();
            await _crashService.MakeBet(bet, currentGame.Id, CurrentUserId);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CloseBet([FromBody] int betId, int gameId)
        {
            //забрать ставку, если ставка забрана до конца игры, то игрок победил и надо начислить ему 
            //средств ставка умноженная на текущий множитель игры
            await _crashService.CloseBet(betId, gameId, CurrentUserId);
            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> StartGame([FromBody] int gameId)
        {
            //запускает игру, после того, как игра запущена, множитель начинает экспоненциальнорасти до указанного при создании игры
            return Ok(await _crashService.StartGame(gameId));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCurrentMultiplier([FromQuery] int gameId)
        {
            //получает текущий множитель игры
            return Ok();
        }
        
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCurrentGame()
        {
            return Ok(await _crashService.GetCurrentGame());
        }
    }
}
