using DatamartManagementService.Infrastructure.Persistence.RofDatamartEntities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DatamartManagementService.Infrastructure.RofDatamartRepos
{
    public interface IJobExecutionHistoryRepository
    {
        Task AddJobExecutionHistory(JobExecutionHistory jobExecutionHistory);
        Task<JobExecutionHistory> GetJobExecutionHistoryByJobType(string jobType);
    }

    public class JobExecutionHistoryRepository : IJobExecutionHistoryRepository
    {
        public async Task<JobExecutionHistory> GetJobExecutionHistoryByJobType(string jobType)
        {
            using var context = new RofDatamartContext();

            return await context.JobExecutionHistory
                .Where(j => j.JobType == jobType.ToLower())
                .OrderByDescending(j => j.Id)
                .FirstOrDefaultAsync();
        }

        public async Task AddJobExecutionHistory(JobExecutionHistory jobExecutionHistory)
        {
            using var context = new RofDatamartContext();

            context.JobExecutionHistory.Add(jobExecutionHistory);

            await context.SaveChangesAsync();
        }
    }
}
