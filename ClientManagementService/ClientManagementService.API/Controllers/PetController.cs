using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClientManagementService.API.DTO;
using ClientManagementService.API.DTOMapper;
using ClientManagementService.Domain.Exceptions;
using ClientManagementService.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClientManagementService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PetController : ControllerBase
    {
        private readonly IPetService _petService;

        public PetController(IPetService petService)
        {
            _petService = petService;
        }

        [HttpPost]
        public async Task<IActionResult> AddPet([FromBody] PetDTO pet)
        {
            try
            {
                await _petService.AddPet(PetDTOMapper.FromDTOPet(pet));

                return StatusCode(201);
            }
            catch (ArgumentException argEx)
            {
                return BadRequest(argEx.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPets([FromQuery] int page, [FromQuery] int offset, [FromQuery] string keyword)
        {
            try
            {
                var petList = new List<PetDTO>();

                var result = await _petService.GetAllPetsByKeyword(page, offset, keyword);

                foreach (var pet in result.Pets)
                {
                    petList.Add(PetDTOMapper.ToDTOPet(pet));
                }

                return Ok(new { pets = petList, totalPages = result.TotalPages });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPetById(long id)
        {
            try
            {
                var pet = await _petService.GetPetById(id);

                return Ok(PetDTOMapper.ToDTOPet(pet));
            }
            catch (EntityNotFoundException notFound)
            {
                return NotFound(notFound.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{name}/name")]
        public async Task<IActionResult> GetPetByName(string name)
        {
            try
            {
                var pet = await _petService.GetPetByName(name);

                return Ok(PetDTOMapper.ToDTOPet(pet));
            }
            catch (EntityNotFoundException notFound)
            {
                return NotFound(notFound.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{clientId}/clientId")]
        public async Task<IActionResult> GetPetsByClientId(long clientId)
        {
            try
            {
                var petList = await _petService.GetPetsByClientId(clientId);

                return Ok(petList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("updatePet")]
        public async Task<IActionResult> UpdatePetInfo([FromBody] PetDTO pet)
        {
            try
            {
                await _petService.UpdatePet(PetDTOMapper.FromDTOPet(pet));

                return Ok();
            }
            catch (EntityNotFoundException notFound)
            {
                return NotFound(notFound.Message);
            }
            catch (ArgumentException argEx)
            {
                return BadRequest(argEx.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePetById(long id)
        {
            try
            {
                await _petService.DeletePetById(id);

                return Ok();
            }
            catch (ArgumentException argEx)
            {
                return BadRequest(argEx.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}