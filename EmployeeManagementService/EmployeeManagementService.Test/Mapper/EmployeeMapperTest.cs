using EmployeeManagementService.Domain.Mappers;
using NUnit.Framework;
using CoreEmployee = EmployeeManagementService.Domain.Models.Employee;
using DbEmployee = EmployeeManagementService.Infrastructure.Persistence.Entities.Employee;

namespace EmployeeManagementService.Test.Mapper
{
    [TestFixture]
    public class EmployeeMapperTest
    {
        [Test]
        public void TestToCoreEmployee()
        {
            var dbEmployee = new DbEmployee();

            dbEmployee.Id = 1;
            dbEmployee.FirstName = "John";
            dbEmployee.LastName = "Doe";
            dbEmployee.Ssn = "123-45-6789";
            dbEmployee.Username = "jdoe";
            dbEmployee.Password = new byte[32];
            dbEmployee.Role = "Admin";
            dbEmployee.IsLocked = false;
            dbEmployee.FailedLoginAttempts = 0;
            dbEmployee.TempPasswordChanged = true;
            dbEmployee.Status = false;
            dbEmployee.Active = true;

            var entity = EmployeeMapper.ToCoreEmployee(dbEmployee);

            Assert.IsNotNull(entity);
            Assert.AreEqual(1, entity.Id);
            Assert.AreEqual("John", entity.FirstName);
            Assert.AreEqual("Doe", entity.LastName);
            Assert.AreEqual("123-45-6789", entity.GetNotSanitizedSSN());
            Assert.AreEqual("jdoe", entity.Username);
            Assert.AreEqual(new byte[32], entity.Password);
            Assert.AreEqual("Admin", entity.Role);
            Assert.AreEqual(false, entity.IsLocked);
            Assert.AreEqual(0, entity.FailedLoginAttempts);
            Assert.AreEqual(true, entity.TempPasswordChanged);
            Assert.AreEqual(false, entity.Status);
            Assert.AreEqual(true, entity.Active);
            Assert.AreEqual("John Doe", entity.FullName);
        }

        [Test]
        public void TestFromCoreEmployee()
        {
            var coreEmployee = new CoreEmployee();

            coreEmployee.Id = 1;
            coreEmployee.FirstName = "John";
            coreEmployee.LastName = "Doe";
            coreEmployee.SetSSN("123-45-6789");
            coreEmployee.Username = "jdoe";
            coreEmployee.Password = new byte[32];
            coreEmployee.Role = "Admin";
            coreEmployee.IsLocked = false;
            coreEmployee.FailedLoginAttempts = 0;
            coreEmployee.TempPasswordChanged = true;
            coreEmployee.Status = false;
            coreEmployee.Active = true;

            var entity = EmployeeMapper.FromCoreEmployee(coreEmployee);

            Assert.IsNotNull(entity);
            Assert.AreEqual(1, entity.Id);
            Assert.AreEqual("John", entity.FirstName);
            Assert.AreEqual("Doe", entity.LastName);
            Assert.AreEqual("123-45-6789", entity.Ssn);
            Assert.AreEqual("jdoe", entity.Username);
            Assert.AreEqual(new byte[32], entity.Password);
            Assert.AreEqual("Admin", entity.Role);
            Assert.AreEqual(false, entity.IsLocked);
            Assert.AreEqual(0, entity.FailedLoginAttempts);
            Assert.AreEqual(true, entity.TempPasswordChanged);
            Assert.AreEqual(false, entity.Status);
            Assert.AreEqual(true, entity.Active);
        }
    }
}
