using System.Collections.Generic;
using CoreSingleRevenue = DatamartManagementService.Domain.Models.RofRevenueFromServicesCompletedByDate;
using DbSingleRevenue = DatamartManagementService.Infrastructure.Persistence.RofDatamartEntities.RofRevenueFromServicesCompletedByDate;
using CorePayrollDetail = DatamartManagementService.Domain.Models.EmployeePayrollDetail;
using DbPayrollDetail = DatamartManagementService.Infrastructure.Persistence.RofDatamartEntities.EmployeePayrollDetail;
using DbJobExecutionHistory = DatamartManagementService.Infrastructure.Persistence.RofDatamartEntities.JobExecutionHistory;
using CoreJobExecutionHistory = DatamartManagementService.Domain.Models.JobExecutionHistory;
using DbDetailedRevenue = DatamartManagementService.Infrastructure.Persistence.RofDatamartEntities.RofRevenueFromServicesCompletedByDate;
using CoreDetailedRevenue = DatamartManagementService.Domain.Models.RofRevenueFromServicesCompletedByDate;
using DbRevenueSummary = DatamartManagementService.Infrastructure.Persistence.RofDatamartEntities.RofRevenueByDate;
using CoreRevenueSummary = DatamartManagementService.Domain.Models.RofRevenueByDate;

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

        public static List<CoreDetailedRevenue> ToCoreDetailedRevenue(List<DbDetailedRevenue> dbDetailedRevenues)
        {
            var coreDetailedRevenues = new List<CoreDetailedRevenue>();

            foreach(var dbDetailedRevenue in dbDetailedRevenues)
            {
                coreDetailedRevenues.Add(new CoreDetailedRevenue
                {
                    Id = dbDetailedRevenue.Id,
                    RevenueDate = dbDetailedRevenue.RevenueDate,
                    PetServiceRate = dbDetailedRevenue.PetServiceRate,
                    NetRevenuePostEmployeeCut = dbDetailedRevenue.NetRevenuePostEmployeeCut
                });
            } 

            return coreDetailedRevenues;
        }

        public static DbRevenueSummary FromCoreRevenueSummary(CoreRevenueSummary coreRevenueSummary)
        {
            var dbRevenueSummary = new DbRevenueSummary();

            dbRevenueSummary.Id = coreRevenueSummary.Id;
            dbRevenueSummary.RevenueDate = coreRevenueSummary.RevenueDate;
            dbRevenueSummary.RevenueMonth = coreRevenueSummary.RevenueMonth;
            dbRevenueSummary.RevenueYear = coreRevenueSummary.RevenueYear;
            dbRevenueSummary.GrossRevenue = coreRevenueSummary.GrossRevenue;
            dbRevenueSummary.NetRevenuePostEmployeePay = coreRevenueSummary.NetRevenuePostEmployeePay;

            return dbRevenueSummary;
        }
    }
}
