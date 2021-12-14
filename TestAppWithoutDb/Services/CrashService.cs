using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TestApp.Models;

namespace TestApp.Services
{
    public class CrashService : ICrashService
    {
        private readonly AppDbContext _appContext;
        private readonly ILogger<CrashService> _logger;

        public CrashService(AppDbContext appContext, ILogger<CrashService> logger)
        {
            _appContext = appContext;
            _logger = logger;
        }

        public async Task<CrashGame> CreateGame()
        {
            try
            {
                var mul = GenerateMultiplierForCrash();

                var game = new CrashGame
                {
                    Multiplier = mul,
                };
                _appContext.CrashGames.Add(game);
                await _appContext.SaveChangesAsync();
                return await _appContext.CrashGames.OrderBy(x=>x.Id).LastOrDefaultAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return null;
            }
        }

        public async Task MakeBet(decimal bet, int gameId, string ownerId)
        {
            var game = await _appContext.CrashGames.Include(g => g.Bets).FirstOrDefaultAsync(x => x.Id == gameId);
            game.Bets.Add(new CrashBet() { Amount = bet, OwnerId = ownerId });
            _appContext.CrashGames.Update(game);
            var user = await _appContext.Users.FirstOrDefaultAsync(x => x.Id == ownerId);
            user.Balance -= bet;
            await _appContext.SaveChangesAsync();
        }

        public async Task CloseBet(int betId, int gameId, string ownerId)
        {
            //начисляем пользователю на баланс ставка*текущий множитель
            throw new NotImplementedException();
        }

        public async Task<CrashGameResult> DetermineLosers(int gameId)
        {
            throw new NotImplementedException();
        }

        public async Task<CrashGame> GetCurrentGame()
        {
            return await _appContext.CrashGames.OrderBy(x=>x.Id).LastOrDefaultAsync();
        }



        private static string SHA512_ComputeHash(string text, string secretKey)
        {
            var hash = new StringBuilder(); ;
            byte[] secretkeyBytes = Encoding.UTF8.GetBytes(secretKey);
            byte[] inputBytes = Encoding.UTF8.GetBytes(text);
            using (var hmac = new HMACSHA512(secretkeyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }

            return hash.ToString();
        }
        private int GenerateNumber(int min, int max, int count)
        {
            if (count < 1)
            {
                throw new ArgumentOutOfRangeException("", "Amount of numbers cannot be negative");
            }

            if (min > max)
            {
                throw new ArgumentOutOfRangeException("", "Min border cannot be greater than max border");
            }
            if (min == max)
            {
                return Convert.ToInt32(string.Join(string.Empty, Enumerable.Repeat(min, count).ToList()));
            }

            using (var rng = new RNGCryptoServiceProvider())
            {
                var numbers = new List<int>();
                var data = new byte[16];
                for (int i = 0; i < count; i++)
                {
                    rng.GetBytes(data);

                    var generatedNumber = Math.Abs(BitConverter.ToUInt16(data, startIndex: 0));

                    int diff = max - min;
                    int mod = generatedNumber % diff;
                    int normalizedValue = min + mod;
                    numbers.Add(normalizedValue);
                }
                return Convert.ToInt32(string.Join(string.Empty, numbers));
            }
        }
        private decimal GenerateMultiplierForCrash()
        {
            var E = GenerateNumber(2, 1000, 1);
            var H = GenerateNumber(0, E - 2, 1);
            var hash = SHA512_ComputeHash(E.ToString() + Guid.NewGuid(), Guid.NewGuid() + H.ToString());
            var hexValue = hash.Substring(0, 13);
            var number1 = Convert.ToInt64(hexValue, 16);


            //var m = Math.Round(((E * 100.0 - H) / (E - H)) / 100.0,2);
            var m = Math.Round((99 / (1 - (number1 / Math.Pow(2, 52)))) / 100.0, 2);

            return (decimal)Math.Round(m, 2);
        }
    }
}
