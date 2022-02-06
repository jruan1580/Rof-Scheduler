using EmployeeManagementService.Domain.Mappers;
using NUnit.Framework;
using CoreEmployee = EmployeeManagementService.Domain.Models.Employee;
using DbEmployee = EmployeeManagementService.Infrastructure.Persistence.Entities.Employee;

namespace EmployeeManagementService.Test.Mapper
{
    [TestFixture]
    public class EmployeeMapperTest
    {
        public void TestToCoreEmployee()
        {
            var dbEmployee = new DbEmployee();
            

            var entity = EmployeeMapper.ToCoreEmployee(dbEmployee);
        }
    }
}
