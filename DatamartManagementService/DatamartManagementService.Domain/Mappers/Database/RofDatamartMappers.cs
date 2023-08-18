using CoreSingleRevenue = DatamartManagementService.Domain.Models.RofRevenueFromServicesCompletedByDate;
using DbSingleRevenue = DatamartManagementService.Infrastructure.Persistence.RofDatamartEntities.RofRevenueFromServicesCompletedByDate;
using System.Collections.Generic;

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
    }
}
