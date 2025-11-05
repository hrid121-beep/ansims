using IMS.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class ExpiryAlertBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ExpiryAlertBackgroundService> _logger;
    private Timer _timer;

    public ExpiryAlertBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<ExpiryAlertBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _timer = new Timer(CheckExpiryAlerts, null, TimeSpan.Zero, TimeSpan.FromHours(24));
        return Task.CompletedTask;
    }

    private async void CheckExpiryAlerts(object state)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var expiryService = scope.ServiceProvider.GetRequiredService<IExpiryTrackingService>();

            await expiryService.SendExpiryAlertsAsync();
            _logger.LogInformation("Expiry alerts checked at {time}", DateTime.Now);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking expiry alerts");
        }
    }

    public override void Dispose()
    {
        _timer?.Dispose();
        base.Dispose();
    }
}