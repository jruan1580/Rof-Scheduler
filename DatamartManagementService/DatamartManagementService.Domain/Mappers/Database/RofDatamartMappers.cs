using System.Collections.Generic;
using DbDetailedRevenue = DatamartManagementService.Infrastructure.Persistence.RofDatamartEntities.RofRevenueFromServicesCompletedByDate;
using CoreDetailedRevenue = DatamartManagementService.Domain.Models.RofDatamartModels.RofRevenueFromServicesCompletedByDate;
using DbDetailedPayroll = DatamartManagementService.Infrastructure.Persistence.RofDatamartEntities.EmployeePayrollDetail;
using CoreDetailedPayroll = DatamartManagementService.Domain.Models.RofDatamartModels.EmployeePayrollDetail;
using DbJobExecutionHistory = DatamartManagementService.Infrastructure.Persistence.RofDatamartEntities.JobExecutionHistory;
using CoreJobExecutionHistory = DatamartManagementService.Domain.Models.RofDatamartModels.JobExecutionHistory;
using DbRevenueSummary = DatamartManagementService.Infrastructure.Persistence.RofDatamartEntities.RofRevenueByDate;
using CoreRevenueSummary = DatamartManagementService.Domain.Models.RofDatamartModels.RofRevenueByDate;
using DbPayrollSummary = DatamartManagementService.Infrastructure.Persistence.RofDatamartEntities.EmployeePayroll;
using CorePayrollSummary = DatamartManagementService.Domain.Models.RofDatamartModels.EmployeePayroll;

namespace DatamartManagementService.Domain.Mappers.Database
{
    public static class RofDatamartMappers
    {
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

            foreach (var dbDetailedRevenue in dbDetailedRevenues)
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

        public static List<DbDetailedRevenue> FromCoreDetailRevenue(List<CoreDetailedRevenue> coreDetailedRevenue)
        {
            var dbDetailedRevenue = new List<DbDetailedRevenue>();

            foreach (var coreRevenue in coreDetailedRevenue)
            {
                dbDetailedRevenue.Add(new DbDetailedRevenue()
                {
                   EmployeeId = coreRevenue.EmployeeId,
                   EmployeeFirstName = coreRevenue.EmployeeFirstName,
                   EmployeeLastName = coreRevenue.EmployeeLastName,
                   EmployeePay = coreRevenue.EmployeePay,
                   PetServiceId = coreRevenue.PetServiceId,
                   PetServiceName = coreRevenue.PetServiceName,
                   PetServiceRate = coreRevenue.PetServiceRate,
                   IsHolidayRate = coreRevenue.IsHolidayRate,
                   NetRevenuePostEmployeeCut = coreRevenue.NetRevenuePostEmployeeCut,
                   RevenueDate = coreRevenue.RevenueDate
                });
            }

            return dbDetailedRevenue;
        }

        public static List<CoreRevenueSummary> ToCoreRevenueSummary(List<DbRevenueSummary> dbRevenueSummary)
        {
            var coreRevenueSummary = new List<CoreRevenueSummary>();

            foreach(var dbRevenue in dbRevenueSummary)
            {
                coreRevenueSummary.Add(new CoreRevenueSummary()
                {
                    Id = dbRevenue.Id,
                    RevenueDate = dbRevenue.RevenueDate,
                    RevenueMonth = dbRevenue.RevenueMonth,
                    RevenueYear = dbRevenue.RevenueYear,
                    GrossRevenue = dbRevenue.GrossRevenue,
                    NetRevenuePostEmployeePay = dbRevenue.NetRevenuePostEmployeePay
                });
            }

            return coreRevenueSummary;
        }

        public static List<DbRevenueSummary> FromCoreRevenueSummary(List<CoreRevenueSummary> coreRevenueSummary)
        {
            var dbRevenueSummary = new List<DbRevenueSummary>();

            foreach(var coreRevenue in coreRevenueSummary)
            {
                dbRevenueSummary.Add(new DbRevenueSummary()
                {
                    Id = coreRevenue.Id,
                    PetServiceId = coreRevenue.PetServiceId,
                    RevenueDate = coreRevenue.RevenueDate,
                    RevenueMonth = coreRevenue.RevenueMonth,
                    RevenueYear = coreRevenue.RevenueYear,
                    GrossRevenue = coreRevenue.GrossRevenue,
                    NetRevenuePostEmployeePay = coreRevenue.NetRevenuePostEmployeePay
                });
            }

            return dbRevenueSummary;
        }

        public static List<DbPayrollSummary> FromCorePayrollSummary(List<CorePayrollSummary> corePayrollSummary)
        {
            var dbPayrollSummary = new List<DbPayrollSummary>();

            foreach (var corePayroll in corePayrollSummary)
            {
                dbPayrollSummary.Add(new DbPayrollSummary()
                {
                    Id = corePayroll.Id,
                    EmployeeId = corePayroll.EmployeeId,
                    FirstName = corePayroll.FirstName,
                    LastName = corePayroll.LastName,
                    EmployeeTotalPay = corePayroll.EmployeeTotalPay,
                    PayrollDate = corePayroll.PayrollDate,
                    PayrollMonth = corePayroll.PayrollMonth,
                    PayrollYear = corePayroll.PayrollYear
                });
            }

            return dbPayrollSummary;
        }

        public static List<DbDetailedPayroll> FromCoreEmployeePayrollDetail(List<CoreDetailedPayroll> corePayrollDetail)
        {
            var dbPayrollDetail = new List<DbDetailedPayroll>();

            foreach (var corePayroll in corePayrollDetail)
            {
                dbPayrollDetail.Add(new DbDetailedPayroll()
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
    }
}
