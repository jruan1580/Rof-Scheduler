using DatamartManagementService.Domain.Mappers.Database;
using DatamartManagementService.Domain.Models.RofSchedulerModels;
using DatamartManagementService.Infrastructure.Persistence.RofSchedulerRepos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatamartManagementService.Domain
{
    public abstract class AImportRevenue
    {
        protected readonly IRofSchedRepo _rofSchedRepo;

        public AImportRevenue(IRofSchedRepo rofSchedRepo)
        {
            _rofSchedRepo = rofSchedRepo;
        }

        public abstract Task ImportRevenueData();

        protected async Task<PetServices> GetPetServiceWithCorrectPayRate(short petServiceId, DateTime jobDate)
        {
            var petService = RofSchedulerMappers.ToCorePetService(
                await _rofSchedRepo.GetPetServiceById(petServiceId));

            await IfDateIsHolidayUpdateRate(petService, jobDate);

            return petService;
        }

        private async Task IfDateIsHolidayUpdateRate(PetServices petService, DateTime jobDate)
        {
            var holiday = await _rofSchedRepo.CheckIfJobDateIsHoliday(jobDate);

            if (holiday != null)
            {
                await UpdateToHolidayPayRate(petService);
            }
        }

        private async Task UpdateToHolidayPayRate(PetServices petService)
        {
            var holidayRate = RofSchedulerMappers.ToCoreHolidayRate(
                    await _rofSchedRepo.GetHolidayRateByPetServiceId(petService.Id));

            petService.EmployeeRate = holidayRate.HolidayRate;
        }


        //Pay and Revenue Calculation
        protected async Task<decimal> CalculateNetRevenueForCompletedService(PetServices petService)
        {
            var pay = await CalculatePayForCompletedService(petService);

            return petService.Price - pay;
        }

        private async Task<decimal> CalculatePayForCompletedService(PetServices petService)
        {
            decimal grosswageEarnedPerService = 0;

            if (petService.TimeUnit.ToLower() == "hour")
            {
               grosswageEarnedPerService = petService.EmployeeRate * petService.Duration;
            }
            else if (petService.TimeUnit.ToLower() == "min")
            {
                var time = petService.Duration / 60; //gets how many of an hour

                grosswageEarnedPerService = petService.EmployeeRate * time;
            }

            return grosswageEarnedPerService;
        }
    }
}
