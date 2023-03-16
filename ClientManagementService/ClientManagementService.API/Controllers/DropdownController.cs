using ClientManagementService.API.DTOMapper;
using ClientManagementService.API.Filters;
using ClientManagementService.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
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
            try
            {
                var clients = await _dropdownService.GetClients();

                return Ok(DropdownDTOMapper.ToClientDTO(clients));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("pets")]
        public async Task<IActionResult> GetPets()
        {
            try
            {
                var pets = await _dropdownService.GetPets();

                return Ok(DropdownDTOMapper.ToPetDTO(pets));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("petTypes")]
        public async Task<IActionResult> GetPetTypes()
        {
            try
            {
                var petTypes = await _dropdownService.GetPetTypes();

                return Ok(DropdownDTOMapper.ToPetTypeDTO(petTypes));
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{petTypeId}/vaccines")]
        public async Task<IActionResult> GetVaccineByPetType(short petTypeId)
        {
            try
            {
                var vaccines = await _dropdownService.GetVaccinesByPetType(petTypeId);

                return Ok(DropdownDTOMapper.ToVaccineDTO(vaccines));
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{petTypeId}/breeds")]
        public async Task<IActionResult> GetBreedsByPetType(short petTypeId)
        {
            try
            {
                var breeds = await _dropdownService.GetBreedsByPetType(petTypeId);

                return Ok(DropdownDTOMapper.ToBreedDTO(breeds));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
