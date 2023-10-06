using System.Collections.Generic;
using CoreSingleRevenue = DatamartManagementService.Domain.Models.RofRevenueFromServicesCompletedByDate;
using DbSingleRevenue = DatamartManagementService.Infrastructure.Persistence.RofDatamartEntities.RofRevenueFromServicesCompletedByDate;
using CorePayrollDetail = DatamartManagementService.Domain.Models.EmployeePayrollDetail;
using DbPayrollDetail = DatamartManagementService.Infrastructure.Persistence.RofDatamartEntities.EmployeePayrollDetail;
using DbJobExecutionHistory = DatamartManagementService.Infrastructure.Persistence.RofDatamartEntities.JobExecutionHistory;
using CoreJobExecutionHistory = DatamartManagementService.Domain.Models.JobExecutionHistory;

namespace DatamartManagementService.Domain.Mappers.Database
{
    public static class RofDatamartMappers
    {
        public static List<DbSingleRevenue> FromCoreRofRevenueFromServicesCompletedByDate(List<CoreSingleRevenue> coreSingleRevs)
        {
            var dbSingleRevs = new List<DbSingleRevenue>();

            foreach (var coreSingleRev in coreSingleRevs)
            {
                dbSingleRevs.Add(new DbSingleRevenue()
                {
                   EmployeeId = coreSingleRev.EmployeeId,
                   EmployeeFirstName = coreSingleRev.EmployeeFirstName,
                   EmployeeLastName = coreSingleRev.EmployeeLastName,
                   EmployeePay = coreSingleRev.EmployeePay,
                   PetServiceId = coreSingleRev.PetServiceId,
                   PetServiceName = coreSingleRev.PetServiceName,
                   PetServiceRate = coreSingleRev.PetServiceRate,
                   IsHolidayRate = coreSingleRev.IsHolidayRate,
                   NetRevenuePostEmployeeCut = coreSingleRev.NetRevenuePostEmployeeCut,
                   RevenueDate = coreSingleRev.RevenueDate
                });
            }

            return dbSingleRevs;
        }

        public static List<DbPayrollDetail> FromCoreEmployeePayrollDetail(List<CorePayrollDetail> corePayrollDetail)
        {
            var dbPayrollDetail = new List<DbPayrollDetail>();

            foreach (var corePayroll in corePayrollDetail)
            {
                dbPayrollDetail.Add(new DbPayrollDetail()
                {
                    EmployeeId = corePayroll.EmployeeId,
                    FirstName = corePayroll.FirstName,
                    LastName = corePayroll.LastName,
                    EmployeePayForService = corePayroll.EmployeePayForService,
                    PetServiceId = corePayroll.PetServiceId,
                    PetServiceName = corePayroll.PetServiceName,
                    ServiceDuration = corePayroll.ServiceDuration,
                    ServiceDurationTimeUnit = corePayroll.ServiceDurationTimeUnit,
                    JobEventId = corePayroll.JobEventId,
                    IsHolidayPay = corePayroll.IsHolidayPay,
                    ServiceStartDateTime = corePayroll.ServiceStartDateTime,
                    ServiceEndDateTime = corePayroll.ServiceEndDateTime
                });
            }

            return dbPayrollDetail;
        }

        public static CoreJobExecutionHistory ToCoreJobExecutionHistory(DbJobExecutionHistory dbHistory)
        {
            var coreHistory = new CoreJobExecutionHistory();

            coreHistory.Id = dbHistory.Id;
            coreHistory.JobType = dbHistory.JobType;
            coreHistory.LastDatePulled = dbHistory.LastDatePulled;

            return coreHistory;
        }

        public static DbJobExecutionHistory FromCoreJobExecutionHistory(CoreJobExecutionHistory coreHistory)
        {
            var dbHistory = new DbJobExecutionHistory();

            dbHistory.Id = coreHistory.Id;
            dbHistory.JobType = coreHistory.JobType;
            dbHistory.LastDatePulled = coreHistory.LastDatePulled;

            return dbHistory;
        }
    }
}
