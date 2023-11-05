using DatamartManagementService.DTO.RofSchedulerDTO;
using CorePetService = DatamartManagementService.Domain.Models.RofSchedulerModels.PetServices;

namespace DatamartManagementService.Domain.Mappers.DTO
{
    public static class PetServicesDTOMapper
    {
        public static PetServicesDTO ToDTOPetServices(CorePetService corePetService)
        {
            var dtoPetService = new PetServicesDTO();

            dtoPetService.Id = corePetService.Id;
            dtoPetService.ServiceName = corePetService.ServiceName;
            dtoPetService.EmployeeRate = corePetService.EmployeeRate;
            dtoPetService.Price = corePetService.Price;
            dtoPetService.Duration = corePetService.Duration;
            dtoPetService.TimeUnit = corePetService.TimeUnit;

            return dtoPetService;
        }

        public static CorePetService FromDTOPetServices(PetServicesDTO dtoPetService)
        {
            var corePetService = new CorePetService();

            corePetService.Id = dtoPetService.Id;
            corePetService.ServiceName = dtoPetService.ServiceName;
            corePetService.EmployeeRate = dtoPetService.EmployeeRate;
            corePetService.Price = dtoPetService.Price;
            corePetService.Duration = dtoPetService.Duration;
            corePetService.TimeUnit = dtoPetService.TimeUnit;

            return corePetService;
        }
    }
}
