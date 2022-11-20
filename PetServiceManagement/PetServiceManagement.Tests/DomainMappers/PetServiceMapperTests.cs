using NUnit.Framework;
using PetServiceManagement.Domain.Constants;
using PetServiceManagement.Domain.Mappers;
using PetServiceManagement.Domain.Models;
using PetServiceManagement.Infrastructure.Persistence.Entities;

namespace PetServiceManagement.Tests.DomainMappers
{
    [TestFixture]
    public class PetServiceMapperTests
    {
        [Test]
        public void MapToDomainPetServiceTest()
        {
            var petServiceEntity = new PetServices()
            {
                Id = 1,
                ServiceName = "Dog Walking (30 Minutes)",
                EmployeeRate = 20m,
                Price = 20.99m,
                Description = "Waling dog for 30 minutes",
                Duration = 30,
                TimeUnit = TimeUnits.MINUTES
            };

            var domainPetService = PetServiceMapper.ToDomainPetService(petServiceEntity);

            Assert.IsNotNull(domainPetService);
            Assert.AreEqual(petServiceEntity.Id, domainPetService.Id);
            Assert.AreEqual(petServiceEntity.ServiceName, domainPetService.Name);
            Assert.AreEqual(petServiceEntity.Price, domainPetService.Price);
            Assert.AreEqual(petServiceEntity.Description, domainPetService.Description);
            Assert.AreEqual(petServiceEntity.EmployeeRate, domainPetService.EmployeeRate);
            Assert.AreEqual(petServiceEntity.Duration, domainPetService.Duration);
            Assert.AreEqual(petServiceEntity.TimeUnit, domainPetService.TimeUnit);
        }

        [Test]
        public void MapFromDomainPetServiceTest()
        {
            var petServiceDomain = new PetService()
            {
                Id = 1,
                Name = "Dog Walking (30 Minutes)",
                EmployeeRate = 20m,
                Price = 20.99m,
                Description = "Waling dog for 30 minutes",
                Duration = 30,
                TimeUnit = TimeUnits.MINUTES
            };

            var entityPetService = PetServiceMapper.FromDomainPetService(petServiceDomain);

            Assert.IsNotNull(entityPetService);
            Assert.AreEqual(petServiceDomain.Id, entityPetService.Id);
            Assert.AreEqual(petServiceDomain.Name, entityPetService.ServiceName);
            Assert.AreEqual(petServiceDomain.Price, entityPetService.Price);
            Assert.AreEqual(petServiceDomain.Description, entityPetService.Description);
            Assert.AreEqual(petServiceDomain.EmployeeRate, entityPetService.EmployeeRate);
            Assert.AreEqual(petServiceDomain.Duration, entityPetService.Duration);
            Assert.AreEqual(petServiceDomain.TimeUnit, entityPetService.TimeUnit);
        }
    }
}
