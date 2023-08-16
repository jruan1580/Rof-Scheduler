using DatamartManagementService.Domain.Models;
using DatamartManagementService.Infrastructure.Persistence.RofSchedulerEntities;
using DatamartManagementService.Infrastructure.RofDatamartRepos;
using DatamartManagementService.Infrastructure.RofSchedulerRepos;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DatamartManagementService.Domain
{
    public class ImportRevenueDataJob : BackgroundService
    {
        private readonly int _hoursInBetweenRun = 12;
        private readonly IRofSchedRepo _rofSchedRepo;

        public ImportRevenueDataJob(IRofSchedRepo rofSchedRepo)
        {
            _rofSchedRepo = rofSchedRepo;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //keep running unless told to stop AKA told to cancel
            while (!stoppingToken.IsCancellationRequested)
            {
                //TODO: import data
                Console.WriteLine("hello from the job");

                await Task.Delay(TimeSpan.FromHours(_hoursInBetweenRun), stoppingToken);
            }       
        }
    }
}
