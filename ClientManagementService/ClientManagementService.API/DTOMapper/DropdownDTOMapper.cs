using ClientManagementService.API.DTO;
using ClientManagementService.Domain.Models;

namespace ClientManagementService.API.DTOMapper
{
    public class DropdownDTOMapper
    {
        public VaccineDTO ToVaccineDTO(Vaccine vaccine)
        {
            var dto = new VaccineDTO();

            dto.Id = vaccine.Id;
            dto.VaccineName = vaccine.VaxName;

            return dto;
        }

        public PetTypeDTO ToPetTypeDTO(PetType petType)
        {
            var dto = new PetTypeDTO();

            dto.Id = petType.Id;
            dto.PetTypeName = petType.PetTypeName;

            return dto;
        }
    }
}
