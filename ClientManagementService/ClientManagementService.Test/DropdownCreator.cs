using ClientManagementService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientManagementService.Test
{
    public static class DropdownCreator
    {
        public static Client GetClientForDropdown()
        {
            return new Client()
            {
                Id = 1,
                FullName = "Test User"
            };
        }

        public static Pet GetPetForDropdown()
        {
            return new Pet()
            {
                Id = 1,
                Name = "Layla"
            };
        }

        public static PetType GetPetTypeForDropdown()
        {
            return new PetType()
            {
                Id = 1,
                PetTypeName = "Dog"
            };
        }

        public static Vaccine GetVaccineForDropdown()
        {
            return new Vaccine()
            {
                Id = 1,
                VaxName = "Bordetella"
            };
        }

        public static Breed GetBreedForDropdown()
        {
            return new Breed()
            {
                Id = 1,
                BreedName = "Golden Retriever"
            };
        }
    }
}
