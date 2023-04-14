using System.Collections.Generic;
using System.Threading.Tasks;
using ClientManagementService.API.DTO;
using ClientManagementService.API.DTOMapper;
using ClientManagementService.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RofShared.FilterAttributes;

namespace ClientManagementService.API.Controllers
{
    [CookieActionFilter]
    [Route("api/[controller]")]
    [ApiController]
    public class PetController : ControllerBase
    {
        private readonly IPetService _petService;

        public PetController(IPetService petService)
        {
            _petService = petService;
        }

        [Authorize(Roles = "Administrator,Employee,Client")]
        [HttpPost]
        public async Task<IActionResult> AddPet([FromBody] PetDTO pet)
        {
            var petId = await _petService.AddPet(PetDTOMapper.FromDTOPet(pet));

            return Ok(new { Id = petId });
        }

        [Authorize(Roles = "Administrator,Employee")]
        [HttpGet]
        public async Task<IActionResult> GetAllPets([FromQuery] int page, [FromQuery] int offset, [FromQuery] string keyword)
        {
            var petList = new List<PetDTO>();

            var result = await _petService.GetAllPetsByKeyword(page, offset, keyword);

            foreach (var pet in result.Pets)
            {
                petList.Add(PetDTOMapper.ToDTOPet(pet));
            }

            return Ok(new { pets = petList, totalPages = result.TotalPages });
        }

        [Authorize(Roles = "Administrator,Employee,Client")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPetById(long id)
        {
            var pet = await _petService.GetPetById(id);

            return Ok(PetDTOMapper.ToDTOPet(pet));
        }

        [Authorize(Roles = "Administrator,Employee,Client")]
        [HttpGet("{name}/name")]
        public async Task<IActionResult> GetPetByName(string name)
        {
            var pet = await _petService.GetPetByName(name);

            return Ok(PetDTOMapper.ToDTOPet(pet));
        }

        [Authorize(Roles = "Administrator,Employee,Client")]
        [HttpGet("clientId")]
        public async Task<IActionResult> GetPetsByClientId([FromQuery] long clientId, [FromQuery] int page, [FromQuery] int offset, [FromQuery] string keyword)
        {
            var petList = new List<PetDTO>();

            var result = await _petService.GetPetsByClientIdAndKeyword(clientId, page, offset, keyword);

            foreach (var pet in result.Pets)
            {
                petList.Add(PetDTOMapper.ToDTOPet(pet));
            }

            return Ok(new { pets = petList, totalPages = result.TotalPages });
        }

        [Authorize(Roles = "Administrator,Employee,Client")]
        [HttpPut("updatePet")]
        public async Task<IActionResult> UpdatePetInfo([FromBody] PetDTO pet)
        {
            await _petService.UpdatePet(PetDTOMapper.FromDTOPet(pet));

            return Ok();
        }

        [Authorize(Roles = "Administrator,Employee,Client")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePetById(long id)
        {
            await _petService.DeletePetById(id);

            return Ok();
        }

        [Authorize(Roles = "Administrator,Employee,Client")]
        [HttpGet("{petId}/vax")]
        public async Task<IActionResult> GetVaccinesByPetId(long petId)
        {
            var vaxStats = new List<PetsVaccineDTO>();

            var result = await _petService.GetVaccinesByPetId(petId);

            foreach (var petVax in result)
            {
                vaxStats.Add(PetDTOMapper.ToDTOPetsVaccine(petVax));
            }

            return Ok(vaxStats);
        }
    }
}