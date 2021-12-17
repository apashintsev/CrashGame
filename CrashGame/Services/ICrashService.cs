using System.Threading.Tasks;
using TestApp.Models;

namespace TestApp.Services
{
    public interface ICrashService
    {
        Task CloseBet(int betId, int gameId, string ownerId);
        Task<Game> CreateGame();
        Task<CrashGameResult> StartGame(int gameId);
        Task<Game> GetCurrentGame();
        Task MakeBet(decimal bet, int gameId, string ownerId);
    }
}