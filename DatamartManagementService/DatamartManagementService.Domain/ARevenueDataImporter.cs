using DatamartManagementService.Domain.Mappers.Database;
using DatamartManagementService.Domain.Models.RofSchedulerModels;
using DatamartManagementService.Infrastructure.Persistence.RofSchedulerRepos;
using System.Threading.Tasks;

namespace DatamartManagementService.Domain
{
    public abstract class ARevenueDataImporter
    {
        protected readonly IRofSchedRepo _rofSchedRepo;

        public ARevenueDataImporter(IRofSchedRepo rofSchedRepo)
        {
            _rofSchedRepo = rofSchedRepo;
        }

        public abstract Task ImportRevenueData();

        protected async Task UpdateToHolidayPayRate(PetServices petService)
        {
            var holidayRate = RofSchedulerMappers.ToCoreHolidayRate(
                    await _rofSchedRepo.GetHolidayRateByPetServiceId(petService.Id));

            petService.EmployeeRate = holidayRate.HolidayRate;
        }

        protected decimal CalculateNetRevenueForCompletedService(PetServices petService)
        {
            var pay = CalculatePayForCompletedService(petService);

            return petService.Price - pay;
        }

        private decimal CalculatePayForCompletedService(PetServices petService)
        {
            var grosswageEarnedPerService = 0m;

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
