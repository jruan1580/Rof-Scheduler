using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetServiceManagement.API.DTO;
using PetServiceManagement.API.DtoMapper;
using PetServiceManagement.Domain.BusinessLogic;
using RofShared.FilterAttributes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetServiceManagement.API.Controllers
{
    [CookieActionFilter]
    [Authorize(Roles = "Administrator")]
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
            var petServices = await _petService.GetPetServicesByPageAndKeyword(page, offset, keyword);

            var petServicesDto = new List<PetServiceDTO>();

            petServices.Item1.ForEach(service => petServicesDto.Add(PetServiceDtoMapper.ToPetServiceDTO(service)));

            return Ok(new PetServicesWithTotalPageDTO(petServicesDto, petServices.Item2));
        }

        [HttpPost]
        public async Task<IActionResult> AddPetService([FromBody]PetServiceDTO petServiceDTO)
        {
            var petService = PetServiceDtoMapper.FromPetServiceDTO(petServiceDTO);

            await _petService.AddNewPetService(petService);

            return Ok();            
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePetService([FromBody]PetServiceDTO petServiceDTO)
        {
            var petService = PetServiceDtoMapper.FromPetServiceDTO(petServiceDTO);

            await _petService.UpdatePetService(petService);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePetService(short id)
        {
            await _petService.DeletePetServiceById(id);

            return Ok();            
        }
    }
}
