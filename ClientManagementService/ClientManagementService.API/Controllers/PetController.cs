using System.Collections.Generic;
using System.Threading.Tasks;
using ClientManagementService.Domain.Mappers.DTO;
using ClientManagementService.Domain.Services;
using ClientManagementService.DTO;
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
        private readonly IPetRetrievalService _petRetrievalService;
        private readonly IPetUpsertService _petUpsertService;

        public PetController(IPetRetrievalService petRetrievalService,
            IPetUpsertService petUpsertService)
        {
            _petRetrievalService = petRetrievalService;
            _petUpsertService = petUpsertService;
        }

        [Authorize(Roles = "Administrator,Employee,Client")]
        [HttpPost]
        public async Task<IActionResult> AddPet([FromBody] PetDTO pet)
        {
            var petId = await _petUpsertService.AddPet(PetDTOMapper.FromDTOPet(pet));

            return Ok(new { Id = petId });
        }

        [Authorize(Roles = "Administrator,Employee")]
        [HttpGet]
        public async Task<IActionResult> GetAllPets([FromQuery] int page, [FromQuery] int offset, [FromQuery] string keyword)
        {
            var petList = new List<PetDTO>();

            var result = await _petRetrievalService.GetAllPetsByKeyword(page, offset, keyword);

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
            var pet = await _petRetrievalService.GetPetById(id);

            return Ok(PetDTOMapper.ToDTOPet(pet));
        }

        [Authorize(Roles = "Administrator,Employee,Client")]
        [HttpGet("{name}/name")]
        public async Task<IActionResult> GetPetByName(string name)
        {
            var pet = await _petRetrievalService.GetPetByName(name);

            return Ok(PetDTOMapper.ToDTOPet(pet));
        }

        [Authorize(Roles = "Administrator,Employee,Client")]
        [HttpGet("clientId")]
        public async Task<IActionResult> GetPetsByClientId([FromQuery] long clientId, [FromQuery] int page, [FromQuery] int offset, [FromQuery] string keyword)
        {
            var petList = new List<PetDTO>();

            var result = await _petRetrievalService.GetPetsByClientIdAndKeyword(clientId, page, offset, keyword);

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
            await _petUpsertService.UpdatePet(PetDTOMapper.FromDTOPet(pet));

            return Ok();
        }

        [Authorize(Roles = "Administrator,Employee,Client")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePetById(long id)
        {
            await _petUpsertService.DeletePetById(id);

            return Ok();
        }
    }
}