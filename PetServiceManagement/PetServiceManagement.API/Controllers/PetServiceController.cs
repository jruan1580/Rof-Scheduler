using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetServiceManagement.API.DTO;
using PetServiceManagement.API.DtoMapper;
using PetServiceManagement.Domain.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetServiceManagement.API.Controllers
{
    [Authorize("Administrator")]
    [Route("api/[controller]")]
    [ApiController]
    public class PetServiceController : ControllerBase
    {
        private readonly IPetServiceManagementService _petService;

        public PetServiceController(IPetServiceManagementService petService)
        {
            _petService = petService;
        }

        [HttpGet]
        public async Task<IActionResult> GetByPageAndServiceName([FromQuery]int page, [FromQuery]int offset, [FromQuery]string keyword)
        {
            try
            {
                var petServices = await _petService.GetPetServicesByPageAndKeyword(page, offset, keyword);

                var petServicesDto = new List<PetServiceDTO>();

                petServices.Item1.ForEach(service => petServicesDto.Add(PetServiceDtoMapper.ToPetServiceDTO(service)));

                return Ok(petServicesDto);
            }
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddPetService([FromBody]PetServiceDTO petServiceDTO)
        {
            try
            {
                var petService = PetServiceDtoMapper.FromPetServiceDTO(petServiceDTO);

                await _petService.AddNewPetService(petService);

                return Ok();
            }
            catch(ArgumentException argEx)
            {
                return BadRequest(argEx.Message);
            }
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePetService([FromBody]PetServiceDTO petServiceDTO)
        {
            try
            {
                var petService = PetServiceDtoMapper.FromPetServiceDTO(petServiceDTO);

                await _petService.UpdatePetService(petService);

                return Ok();
            }
            catch (ArgumentException argEx)
            {
                return BadRequest(argEx.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpDelete("id")]
        public async Task<IActionResult> DeletePetService(short id)
        {
            try
            {
                await _petService.DeletePetServiceById(id);

                return Ok();
            }          
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
