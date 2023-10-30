using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DatamartManagementService.Domain.Importer
{
    public class ImportRevenueDataBackgroundService : BackgroundService
    {
        private readonly int _hoursInBetweenRun = 24;
        private readonly IDetailedRevenueImporter _detailedRevenueImporter;
        private readonly IRevenueSummaryImporter _revenueSummaryImporter;

        public ImportRevenueDataBackgroundService(
            IDetailedRevenueImporter detailedRevenueImporter,
            IRevenueSummaryImporter revenueSummaryImporter)
        {
            _detailedRevenueImporter = detailedRevenueImporter;
            _revenueSummaryImporter = revenueSummaryImporter;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //keep running unless told to stop AKA told to cancel
            while (!stoppingToken.IsCancellationRequested)
            {
                //TODO: import data
                Console.WriteLine("hello from revenue job");

                await _detailedRevenueImporter.ImportRevenueData();
                await _revenueSummaryImporter.ImportRevenueSummary();

                await Task.Delay(TimeSpan.FromHours(_hoursInBetweenRun), stoppingToken);
            }
        }
    }
}
