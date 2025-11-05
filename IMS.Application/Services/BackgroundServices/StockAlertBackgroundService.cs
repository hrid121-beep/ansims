using IMS.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace IMS.Application.Services.BackgroundServices
{
    public class StockAlertBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<StockAlertBackgroundService> _logger;
        private Timer _timer;

        public StockAlertBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<StockAlertBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(CheckStockAlerts, null, TimeSpan.Zero, TimeSpan.FromHours(6));
            return Task.CompletedTask;
        }

        private async void CheckStockAlerts(object state)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var stockAlertService = scope.ServiceProvider.GetRequiredService<IStockAlertService>();

                await stockAlertService.CheckAndSendLowStockAlertsAsync();
                _logger.LogInformation("Stock alerts checked at {time}", DateTime.Now);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking stock alerts");
            }
        }

        public override void Dispose()
        {
            _timer?.Dispose();
            base.Dispose();
        }
    }
}
