using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DatamartManagementService.Domain
{
    public class ImportPayrollDataBackgroundService : BackgroundService
    {
        private readonly int _hoursInBetweenRun = 24;
        private readonly IDetailedPayrollImporter _detailedPayrollImporter;

        public ImportPayrollDataBackgroundService(
            IDetailedPayrollImporter detailedPayrollImporter)
        {
            _detailedPayrollImporter = detailedPayrollImporter;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //keep running unless told to stop AKA told to cancel
            while (!stoppingToken.IsCancellationRequested)
            {
                //TODO: import data
                Console.WriteLine("hello from payroll job");

                await _detailedPayrollImporter.ImportPayrollData();

                await Task.Delay(TimeSpan.FromHours(_hoursInBetweenRun), stoppingToken);
            }
        }
    }
}
