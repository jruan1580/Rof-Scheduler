using ClientManagementService.API.DTO;
using ClientManagementService.Domain.Models;
using System.Collections.Generic;

namespace ClientManagementService.API.DTOMapper
{
    public static class DropdownDTOMapper
    {
        public static List<VaccineDTO> ToVaccineDTO(List<Vaccine> vaccines)
        {
            var dtos = new List<VaccineDTO>();
            foreach(var vaccine in vaccines)
            {
                var dto = new VaccineDTO();

                dto.Id = vaccine.Id;
                dto.VaccineName = vaccine.VaxName;

                dtos.Add(dto);
            }
           
            return dtos;
        }

        public static List<PetTypeDTO> ToPetTypeDTO(List<PetType> petTypes)
        {
            var dtos = new List<PetTypeDTO>();

            foreach(var petType in petTypes)
            {
                var dto = new PetTypeDTO();

                dto.Id = petType.Id;
                dto.PetTypeName = petType.PetTypeName;

                dtos.Add(dto);
            }
          
            return dtos;
        }

        public static List<BreedDTO> ToBreedDTO(List<Breed> breeds)
        {
            var dtos = new List<BreedDTO>();

            foreach(var breed in breeds)
            {
                var dto = new BreedDTO()
                {
                    Id = breed.Id,
                    BreedName = breed.BreedName
                };

                dtos.Add(dto);
            }

            return dtos;
        }

        public static List<ClientDTO> ToClientDTO(List<Client> clients)
        {
            var dtos = new List<ClientDTO>();

            foreach(var client in clients)
            {
                dtos.Add(new ClientDTO()
                {
                    Id = client.Id,
                    FullName = client.FullName
                });
            }

            return dtos;
        }
    }
}
