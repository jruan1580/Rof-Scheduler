using NUnit.Framework;
using PetServiceManagement.API.DTO;
using PetServiceManagement.API.DtoMapper;
using PetServiceManagement.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetServiceManagement.Tests.DtoMappers
{
    [TestFixture]
    public class PetServiceDtoMapperTests
    {
        [Test]
        public void ToPetServiceDtoTest()
        {
            var domain = new PetService()
            {
                Id = 1,
                Name = "Dog Walking",
                Description = "Walking dog",
                Price = 20m,
                EmployeeRate = 10m
            };

            var dto = PetServiceDtoMapper.ToPetServiceDTO(domain);

            Assert.IsNotNull(dto);
            Assert.AreEqual(domain.Id, dto.Id);
            Assert.AreEqual(domain.Name, dto.Name);
            Assert.AreEqual(domain.Description, dto.Description);
            Assert.AreEqual(domain.Price, dto.Rate);
            Assert.AreEqual(domain.EmployeeRate, dto.EmployeeRate);
        }

        [Test]
        public void FromPetServiceDtoTest()
        {
            var dto = new PetServiceDTO()
            {
                Id = 1,
                Name = "Dog Walking",
                Description = "Walking dog",
                Rate = 20m,
                EmployeeRate = 10m
            };

            var domain = PetServiceDtoMapper.FromPetServiceDTO(dto);

            Assert.IsNotNull(domain);
            Assert.AreEqual(dto.Id, domain.Id);
            Assert.AreEqual(dto.Name, domain.Name);
            Assert.AreEqual(dto.Description, domain.Description);
            Assert.AreEqual(dto.Rate, domain.Price);
            Assert.AreEqual(dto.EmployeeRate, domain.EmployeeRate);
        }
    }
}
