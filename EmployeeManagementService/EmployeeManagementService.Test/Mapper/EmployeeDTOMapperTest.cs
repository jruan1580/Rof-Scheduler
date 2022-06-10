using EmployeeManagementService.API.DTOMappers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using CoreEmployee = EmployeeManagementService.Domain.Models.Employee;
using DTOEmployee = EmployeeManagementService.API.DTO.EmployeeDTO;

namespace EmployeeManagementService.Test.Mapper
{
    [TestFixture]
    public class EmployeeDTOMapperTest
    {
        [Test]
        public void TestToDTOEmployee()
        {
            var coreEmp = new CoreEmployee();

            coreEmp.Id = 1;
            coreEmp.FirstName = "John";
            coreEmp.LastName = "Doe";
            coreEmp.Ssn = "123-45-6789";
            coreEmp.Username = "jdoe";
            coreEmp.Email = "test@email.com";
            coreEmp.PhoneNumber = "9998887776";
            coreEmp.Role = "Admin";
            coreEmp.IsLocked = false;
            coreEmp.FailedLoginAttempts = 0;
            coreEmp.TempPasswordChanged = true;
            coreEmp.Status = false;
            coreEmp.Active = true;
            coreEmp.SetFullName();

            coreEmp.Address = new Domain.Models.Address();
            coreEmp.Address.AddressLine1 = "123 test st.";
            coreEmp.Address.AddressLine2 = null;
            coreEmp.Address.City = "san diego";
            coreEmp.Address.State = "CA";
            coreEmp.Address.ZipCode = "12345";


            var dtoEmp = EmployeeDTOMapper.ToDTOEmployee(coreEmp);

            Assert.IsNotNull(dtoEmp);
            Assert.AreEqual(1, dtoEmp.Id);
            Assert.AreEqual("John", dtoEmp.FirstName);
            Assert.AreEqual("Doe", dtoEmp.LastName);
            Assert.AreEqual("123-45-6789", dtoEmp.Ssn);
            Assert.AreEqual("jdoe", dtoEmp.Username);
            Assert.AreEqual("test@email.com", dtoEmp.EmailAddress);
            Assert.AreEqual("9998887776", dtoEmp.PhoneNumber);
            Assert.AreEqual("Admin", dtoEmp.Role);
            Assert.AreEqual(false, dtoEmp.IsLocked);
            Assert.AreEqual(0, dtoEmp.FailedLoginAttempts);
            Assert.AreEqual(true, dtoEmp.TempPasswordChanged);
            Assert.AreEqual(false, dtoEmp.Status);
            Assert.AreEqual(true, dtoEmp.Active);
            Assert.AreEqual("John Doe", dtoEmp.FullName);
            Assert.AreEqual("123 test st.", dtoEmp.Address?.AddressLine1);
            Assert.IsNull(dtoEmp.Address?.AddressLine2);
            Assert.AreEqual("CA", dtoEmp.Address?.State);
            Assert.AreEqual("san diego", dtoEmp.Address?.City);
            Assert.AreEqual("12345", dtoEmp.Address?.ZipCode);
        }

        [Test]
        public void TestFromDTOEmployee()
        {
            var dtoEmp = new DTOEmployee();

            dtoEmp.Id = 1;
            dtoEmp.FirstName = "John";
            dtoEmp.LastName = "Doe";
            dtoEmp.Ssn = "123-45-6789";
            dtoEmp.Username = "jdoe";
            dtoEmp.EmailAddress = "test@email.com";
            dtoEmp.PhoneNumber = "9998887776";
            dtoEmp.Role = "Admin";
            dtoEmp.IsLocked = false;
            dtoEmp.FailedLoginAttempts = 0;
            dtoEmp.TempPasswordChanged = true;
            dtoEmp.Status = false;
            dtoEmp.Active = true;

            dtoEmp.Address = new API.DTO.AddressDTO();
            dtoEmp.Address.AddressLine1 = "123 test st.";
            dtoEmp.Address.AddressLine2 = null;
            dtoEmp.Address.City = "san diego";
            dtoEmp.Address.State = "CA";
            dtoEmp.Address.ZipCode = "12345";


            var coreEmp = EmployeeDTOMapper.FromDTOEmployee(dtoEmp);

            Assert.IsNotNull(coreEmp);
            Assert.AreEqual(1, coreEmp.Id);
            Assert.AreEqual("John", coreEmp.FirstName);
            Assert.AreEqual("Doe", coreEmp.LastName);
            Assert.AreEqual("123-45-6789", coreEmp.GetNotSanitizedSSN());
            Assert.AreEqual("***-**-6789", coreEmp.Ssn);
            Assert.AreEqual("jdoe", coreEmp.Username);
            Assert.AreEqual("test@email.com", coreEmp.Email);
            Assert.AreEqual("9998887776", coreEmp.PhoneNumber);
            Assert.AreEqual("Admin", coreEmp.Role);
            Assert.AreEqual(false, coreEmp.IsLocked);
            Assert.AreEqual(0, coreEmp.FailedLoginAttempts);
            Assert.AreEqual(true, coreEmp.TempPasswordChanged);
            Assert.AreEqual(false, coreEmp.Status);
            Assert.AreEqual(true, coreEmp.Active);
            Assert.AreEqual("John Doe", coreEmp.FullName);
            Assert.AreEqual("123 test st.", coreEmp.Address?.AddressLine1);
            Assert.IsNull(coreEmp.Address?.AddressLine2);
            Assert.AreEqual("CA", coreEmp.Address?.State);
            Assert.AreEqual("san diego", coreEmp.Address?.City);
            Assert.AreEqual("12345", coreEmp.Address?.ZipCode);
        }
    }
}
