using NUnit.Framework;
using PetServiceManagement.API.DTO;
using PetServiceManagement.API.DtoMapper;
using PetServiceManagement.Domain.Constants;
using PetServiceManagement.Domain.Models;

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
                EmployeeRate = 10m,
                Duration = 30,
                TimeUnit = TimeUnits.MINUTES
            };

            var dto = PetServiceDtoMapper.ToPetServiceDTO(domain);

            Assert.IsNotNull(dto);
            Assert.AreEqual(domain.Id, dto.Id);
            Assert.AreEqual(domain.Name, dto.Name);
            Assert.AreEqual(domain.Description, dto.Description);
            Assert.AreEqual(domain.Price, dto.Rate);
            Assert.AreEqual(domain.EmployeeRate, dto.EmployeeRate);
            Assert.AreEqual(domain.Duration, dto.Duration);
            Assert.AreEqual(domain.TimeUnit, dto.TimeUnit);
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
                EmployeeRate = 10m,
                Duration = 30,
                TimeUnit = TimeUnits.MINUTES
            };

            var domain = PetServiceDtoMapper.FromPetServiceDTO(dto);

            Assert.IsNotNull(domain);
            Assert.AreEqual(dto.Id, domain.Id);
            Assert.AreEqual(dto.Name, domain.Name);
            Assert.AreEqual(dto.Description, domain.Description);
            Assert.AreEqual(dto.Rate, domain.Price);
            Assert.AreEqual(dto.EmployeeRate, domain.EmployeeRate);
            Assert.AreEqual(dto.Duration, domain.Duration);
            Assert.AreEqual(dto.TimeUnit, domain.TimeUnit);
        }
    }
}
