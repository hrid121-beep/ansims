using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Application.Services.BackgroundServices
{
    public class DailySummaryBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public DailySummaryBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Generate daily summary reports
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}
