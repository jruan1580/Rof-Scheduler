using EmployeeManagementService.Domain.Mappers.Database;
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
            dbEmployee.CountryId = 1;
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
            dbEmployee.AddressLine1 = "123 test st.";
            dbEmployee.State = "CA";
            dbEmployee.City = "san diego";
            dbEmployee.ZipCode = "12345";

            var core = EmployeeMapper.ToCoreEmployee(dbEmployee);

            Assert.IsNotNull(core);
            Assert.AreEqual(1, core.Id);
            Assert.AreEqual(1, core.CountryId);
            Assert.AreEqual("John", core.FirstName);
            Assert.AreEqual("Doe", core.LastName);
            Assert.AreEqual("123-45-6789", core.GetNotSanitizedSSN());
            Assert.AreEqual("***-**-6789", core.Ssn);
            Assert.AreEqual("jdoe", core.Username);
            Assert.AreEqual(new byte[32], core.Password);
            Assert.AreEqual("Admin", core.Role);
            Assert.AreEqual(false, core.IsLocked);
            Assert.AreEqual(0, core.FailedLoginAttempts);
            Assert.AreEqual(true, core.TempPasswordChanged);
            Assert.AreEqual(false, core.Status);
            Assert.AreEqual(true, core.Active);
            Assert.AreEqual("John Doe", core.FullName);
            Assert.AreEqual("123 test st.", core.Address.AddressLine1);
            Assert.AreEqual("CA", core.Address.State);
            Assert.AreEqual("san diego", core.Address.City);
            Assert.AreEqual("12345", core.Address.ZipCode);
        }

        [Test]
        public void TestFromCoreEmployeeWithoutAddress()
        {
            var coreEmployee = new CoreEmployee();

            coreEmployee.Id = 1;
            coreEmployee.CountryId = 1;
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
            Assert.AreEqual(1, entity.CountryId);
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
            Assert.IsNull(entity.AddressLine1);
            Assert.IsNull(entity.AddressLine2);
            Assert.IsNull(entity.State);
            Assert.IsNull(entity.City);
            Assert.IsNull(entity.ZipCode);
        }

        [Test]
        public void TestFromCoreEmployeeWithAddress()
        {
            var coreEmployee = new CoreEmployee();

            coreEmployee.Id = 1;
            coreEmployee.CountryId = 1;
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

            coreEmployee.SetAddress("123 test st", null, "san diego", "CA", "12345");

            var entity = EmployeeMapper.FromCoreEmployee(coreEmployee);

            Assert.IsNotNull(entity);
            Assert.AreEqual(1, entity.Id);
            Assert.AreEqual(1, entity.CountryId);
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
            Assert.AreEqual("123 test st", entity.AddressLine1);
            Assert.IsNull(entity.AddressLine2);
            Assert.AreEqual("CA", entity.State);
            Assert.AreEqual("san diego", entity.City);
            Assert.AreEqual("12345", entity.ZipCode);
        }
    }
}
