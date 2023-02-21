using EmployeeManagementService.Domain.Mappers.DTO;
using EmployeeManagementService.Domain.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementService.Test.Mapper
{
    [TestFixture]
    public class DropdownDTOMapperTest
    {
        [Test]
        public void TestToEmployeeDTO()
        {
            var employee = new Employee()
            {
                Id = 1,
                FullName = "Test User"
            };

            var dtos = DropdownDTOMapper.ToEmployeeDTO(new List<Employee> { employee });

            Assert.IsNotNull(dtos);
            Assert.AreEqual(1, dtos.Count);

            var dto = dtos[0];
            Assert.AreEqual(dto.Id, employee.Id);
            Assert.AreEqual(dto.FullName, employee.FullName);
        }
    }
}
