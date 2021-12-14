using System.Threading.Tasks;
using TestApp.Models;

namespace TestApp.Services
{
    public interface ICrashService
    {
        Task CloseBet(int betId, int gameId, string ownerId);
        Task<CrashGame> CreateGame();
        Task<CrashGameResult> DetermineLosers(int gameId);
        Task<CrashGame> GetCurrentGame();
        Task MakeBet(decimal bet, int gameId, string ownerId);
    }
}