using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClientManagementService.API.DTO;
using ClientManagementService.API.DTOMapper;
using ClientManagementService.Domain.Exceptions;
using ClientManagementService.Domain.Services;
using Microsoft.AspNetCore.Http;
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
            catch (PetNotFoundException)
            {
                return NotFound($"Pet with id {id} not found");
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
            catch (PetNotFoundException)
            {
                return NotFound($"Pet with name {name} not found");
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

        //public async Task UpdatePet(Pet updatePet)
        //{
        //    var invalidErrs = updatePet.IsValidPetToUpdate().ToArray();

        //    if (invalidErrs.Length > 0)
        //    {
        //        var errMsg = string.Join("\n", invalidErrs);

        //        throw new ArgumentException(errMsg);
        //    }

        //    var petExists = await _petRepository.PetAlreadyExists(updatePet.OwnerId, updatePet.BreedId, updatePet.Name);
        //    if (petExists)
        //    {
        //        throw new ArgumentException($"Pet with same name and breed already exist under this owner id {updatePet.OwnerId}");
        //    }

        //    var origPet = await _petRepository.GetPetByFilter(new GetPetFilterModel<long>(GetPetFilterEnum.Id, updatePet.Id));
        //    if (origPet == null)
        //    {
        //        throw new PetNotFoundException();
        //    }

        //    origPet.Name = updatePet.Name;
        //    origPet.Weight = updatePet.Weight;
        //    origPet.Dob = updatePet.Dob;
        //    origPet.BordetellaVax = updatePet.BordetellaVax;
        //    origPet.RabieVax = updatePet.RabieVax;
        //    origPet.Dhppvax = updatePet.Dhppvax;
        //    origPet.BreedId = updatePet.BreedId;
        //    origPet.OwnerId = updatePet.OwnerId;
        //    origPet.OtherInfo = updatePet.OtherInfo;
        //    origPet.Picture = updatePet.Picture;

        //    await _petRepository.UpdatePet(origPet);
        //}

        //public async Task DeletePetById(long petId)
        //{
        //    await _petRepository.DeletePetById(petId);
        //}
    }
}