using PetServiceManagement.API.DTO;
using PetServiceManagement.Domain.Models;
using PetServiceManagement.Infrastructure.Persistence.Entities;

namespace PetServiceManagement.Tests
{
    public static class HolidayRateFactory
    {
        public static HolidayRate GetHolidayRateDomainObj()
        {
            return new HolidayRate()
            {
                Id = 1,
                PetService = PetServiceFactory.GetPetServiceDomain(),
                Holiday = HolidayFactory.GetHolidayDomainObj(),
                Rate = 30m
            };
        }

        public static HolidayRates GetHoldayRateDbEntity()
        {
            return new HolidayRates()
            {
                Id = 1,
                PetServiceId = 1,
                HolidayId = 1,
                PetService = PetServiceFactory.GetPetServicesDbEntity(),
                Holiday = HolidayFactory.GetHolidayDbEntityObj(),
                HolidayRate = 20m
            };
        }

        public static HolidayRateDTO GetHolidayRateDTO()
        {
            return new HolidayRateDTO()
            {
                Id = 1,
                Rate = 20m,
                PetService = PetServiceFactory.GetPetServiceDTO(),
                Holiday = HolidayFactory.GetHolidayDTO()
            };
        }
    }
}
