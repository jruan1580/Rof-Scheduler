using DatamartManagementService.Infrastructure.Persistence.RofDatamartRepos;
using DatamartManagementService.Infrastructure.Persistence.RofSchedulerRepos;
using Microsoft.Extensions.DependencyInjection;

namespace DataMart
{
    public static class DatabaseDependencyInitializer
    {
        public static IServiceCollection AddDatabaseDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IRofSchedRepo, RofSchedRepo>();
            services.AddSingleton<IRevenueFromServicesUpsertRepository, RevenueFromServicesUpsertRepository>();
            services.AddSingleton<IJobExecutionHistoryRepository, JobExecutionHistoryRepository>();
            services.AddSingleton<IPayrollDetailUpsertRepository, PayrollDetailUpsertRepository>();
            services.AddSingleton<IPayrollUpsertRepository, PayrollUpsertRepository>();
            services.AddSingleton<IRevenueFromServicesRetrievalRepository, RevenueFromServicesRetrievalRepository>();
            services.AddSingleton<IRevenueByDateUpsertRepository, RevenueByDateUpsertRepository>();

            return services;
        }
    }
}
