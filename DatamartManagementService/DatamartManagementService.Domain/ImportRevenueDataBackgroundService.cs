using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DatamartManagementService.Domain
{
    public class ImportRevenueDataBackgroundService : BackgroundService
    {
        private readonly int _hoursInBetweenRun = 24;
        private readonly IDetailedRevenueImporter _singleRevenueDateImporter;

        public ImportRevenueDataBackgroundService(
            IDetailedRevenueImporter singleRevenueDateImporter)
        {
            _singleRevenueDateImporter = singleRevenueDateImporter;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //keep running unless told to stop AKA told to cancel
            while (!stoppingToken.IsCancellationRequested)
            {
                //TODO: import data
                Console.WriteLine("hello from the job");

                await _singleRevenueDateImporter.ImportRevenueData();
             
                await Task.Delay(TimeSpan.FromHours(_hoursInBetweenRun), stoppingToken);
            }       
        }
    }
}
