using ClientManagementService.API.DTOMapper;
using ClientManagementService.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RofShared.FilterAttributes;
using System.Threading.Tasks;

namespace ClientManagementService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [CookieActionFilter]
    [Authorize(Roles = "Administrator,Employee,Client")]
    public class DropdownController : ControllerBase
    {
        private readonly IDropdownService _dropdownService;

        public DropdownController(IDropdownService dropdownService)
        {
            _dropdownService = dropdownService;
        }

        [HttpGet("clients")]
        public async Task<IActionResult> GetClients()
        {
            var clients = await _dropdownService.GetClients();

            return Ok(DropdownDTOMapper.ToClientDTO(clients));
        }

        [HttpGet("pets")]
        public async Task<IActionResult> GetPets()
        {
            var pets = await _dropdownService.GetPets();

            return Ok(DropdownDTOMapper.ToPetDTO(pets));
        }

        [HttpGet("petTypes")]
        public async Task<IActionResult> GetPetTypes()
        {
            var petTypes = await _dropdownService.GetPetTypes();

            return Ok(DropdownDTOMapper.ToPetTypeDTO(petTypes));
        }

        [HttpGet("{petTypeId}/vaccines")]
        public async Task<IActionResult> GetVaccineByPetType(short petTypeId)
        {
            var vaccines = await _dropdownService.GetVaccinesByPetType(petTypeId);

            return Ok(DropdownDTOMapper.ToVaccineDTO(vaccines));
        }

        [HttpGet("{petTypeId}/breeds")]
        public async Task<IActionResult> GetBreedsByPetType(short petTypeId)
        {
            var breeds = await _dropdownService.GetBreedsByPetType(petTypeId);

            return Ok(DropdownDTOMapper.ToBreedDTO(breeds));
        }
    }
}
