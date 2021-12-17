using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestApp.Services;

namespace CrashGame
{
    public class CrashTimeService : BackgroundService
    {
        private readonly ILogger<CrashTimeService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private ICrashService _crashService;

        public CrashTimeService(ILogger<CrashTimeService> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {

            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //await Task.Delay(100, stoppingToken);
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    _crashService = scope.ServiceProvider.GetService<ICrashService>();
                    var game = await _crashService.GetCurrentGame();
                    if (game is null)
                    {
                        await _crashService.CreateGame();
                        game = await _crashService.GetCurrentGame();
                    }

                    //рассчитываем коэффициент и обновляем в Redis
                    var currentMultiplier = 0;


                    if (game.Multiplier==currentMultiplier)
                    {
                        await _crashService.StartGame(game.Id);
                    }
                }
            }
        }
    }
}
