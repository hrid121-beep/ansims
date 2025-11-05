using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using IMS.Application.Services;

namespace IMS.Infrastructure.Services
{
    public class LifeSpanAlertBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<LifeSpanAlertBackgroundService> _logger;
        private Timer _timer;

        public LifeSpanAlertBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<LifeSpanAlertBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Life Span Alert Service is starting.");

            _timer = new Timer(
                CheckLifeSpanAlerts,
                null,
                TimeSpan.Zero,
                TimeSpan.FromHours(24) // Run daily
            );

            return Task.CompletedTask;
        }

        private async void CheckLifeSpanAlerts(object state)
        {
            _logger.LogInformation("Running life span alert check at {time}", DateTimeOffset.Now);

            using (var scope = _serviceProvider.CreateScope())
            {
                var personnelItemLifeService = scope.ServiceProvider
                    .GetRequiredService<IPersonnelItemLifeService>();

                try
                {
                    await personnelItemLifeService.ProcessExpiryAlertsAsync();
                    _logger.LogInformation("Life span alert check completed successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while checking life span alerts");
                }
            }
        }

        public override void Dispose()
        {
            _timer?.Dispose();
            base.Dispose();
        }
    }
}