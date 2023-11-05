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

            return dtoPetService;
        }

        public static CorePetService FromDTOPetServices(PetServicesDTO dtoPetService)
        {
            var corePetService = new CorePetService();

            corePetService.Id = dtoPetService.Id;
            corePetService.ServiceName = dtoPetService.ServiceName;

            return corePetService;
        }
    }
}
