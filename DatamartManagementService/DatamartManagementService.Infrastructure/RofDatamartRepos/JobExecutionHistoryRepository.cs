﻿using DatamartManagementService.Infrastructure.Persistence.RofDatamartEntities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DatamartManagementService.Infrastructure.RofDatamartRepos
{
    public class JobExecutionHistoryRepository
    {
        public async Task<JobExecutionHistory> GetJobExecutionHistoryByJobType(string jobType)
        {
            using var context = new RofDatamartContext();

            var jobExecutionHistory = await context.JobExecutionHistory.Where(j => j.JobType == jobType)
                .OrderByDescending(j => j.Id).ToListAsync();

            return jobExecutionHistory[0];
        }

        public async Task AddJobExecutionHistory(JobExecutionHistory jobExecutionHistory)
        {
            using var context = new RofDatamartContext();

            context.JobExecutionHistory.Add(jobExecutionHistory);

            await context.SaveChangesAsync();
        }
    }
}
