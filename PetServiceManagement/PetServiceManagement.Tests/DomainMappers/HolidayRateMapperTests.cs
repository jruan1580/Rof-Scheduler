using NUnit.Framework;
using PetServiceManagement.Domain.Mappers;
using PetServiceManagement.Domain.Models;
using PetServiceManagement.Infrastructure.Persistence.Entities;
using System;

namespace PetServiceManagement.Tests.DomainMappers
{
    [TestFixture]
    public class HolidayRateMapperTests
    {
        [Test]
        public void MapToDomainHolidayRateTest()
        {
            var holidayRateEntity = new HolidayRates()
            {
                Id = 1,
                HolidayRate = 20m,
                PetServiceId = 1,
                PetService = new PetServices()
                {
                    Id = 1,
                    ServiceName = "Dog Walking (30 Minutes)",
                    EmployeeRate = 20m,
                    Price = 20.99m,
                    Description = "Waling dog for 30 minutes"
                },
                HolidayDateId = 1,
                HolidayDate = new Holidays()
                {
                    Id = 1,
                    HolidayName = "CNY",
                    HolidayDate = DateTime.Now
                }
            };

            var holidayRateDomain = HolidayRatesMapper.ToDomainHolidayRate(holidayRateEntity);

            Assert.IsNotNull(holidayRateDomain);
            Assert.AreEqual(holidayRateEntity.Id, holidayRateDomain.Id);
            Assert.AreEqual(holidayRateEntity.HolidayRate, holidayRateDomain.Rate);

            Assert.IsNotNull(holidayRateDomain.PetService);
            Assert.AreEqual(holidayRateEntity.PetServiceId, holidayRateDomain.PetService.Id);
            Assert.AreEqual(holidayRateEntity.PetService.ServiceName, holidayRateDomain.PetService.Name);
            Assert.AreEqual(holidayRateEntity.PetService.Description, holidayRateDomain.PetService.Description);
            Assert.AreEqual(holidayRateEntity.PetService.Price, holidayRateDomain.PetService.Price);
            Assert.AreEqual(holidayRateEntity.PetService.EmployeeRate, holidayRateDomain.PetService.EmployeeRate);

            Assert.IsNotNull(holidayRateDomain.Holiday);
            Assert.AreEqual(holidayRateEntity.HolidayDateId, holidayRateDomain.Holiday.Id);
            Assert.AreEqual(holidayRateEntity.HolidayDate.HolidayName, holidayRateDomain.Holiday.Name);
            Assert.AreEqual(holidayRateEntity.HolidayDate.HolidayDate, holidayRateDomain.Holiday.HolidayDate);
        }

        [Test]
        public void MapFromDomainHolidayRateTest()
        {
            var holidayRateDomain = new HolidayRate()
            {
                Id = 1,
                Rate = 20m,
                PetService = new PetService()
                {
                    Id = 1,
                    Name = "Dog Walking (30 Minutes)",
                    EmployeeRate = 20m,
                    Price = 20.99m,
                    Description = "Waling dog for 30 minutes"
                },
                Holiday = new Holiday()
                {
                    Id = 1,
                    Name = "CNY",
                    HolidayDate = DateTime.Now
                }
            };

            var holidayRateEntity = HolidayRatesMapper.FromDomainHolidayRate(holidayRateDomain);

            Assert.IsNotNull(holidayRateEntity);
            Assert.AreEqual(holidayRateDomain.Id, holidayRateEntity.Id);
            Assert.AreEqual(holidayRateDomain.Rate, holidayRateEntity.HolidayRate);
            Assert.AreEqual(holidayRateDomain.PetService.Id, holidayRateEntity.PetServiceId);
            Assert.AreEqual(holidayRateDomain.Holiday.Id, holidayRateEntity.HolidayDateId);
        }
    }
}
