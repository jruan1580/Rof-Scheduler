using DatamartManagementService.Domain;
using DatamartManagementService.Domain.Mappers.DTO;
using DatamartManagementService.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace DataMart.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class RevenueSummaryController : ControllerBase
    {
        private readonly IRevenueSummaryRetrievalService _revenueSummaryRetrievalService;

        public RevenueSummaryController(IRevenueSummaryRetrievalService revenueSummaryRetrievalService)
        {
            _revenueSummaryRetrievalService = revenueSummaryRetrievalService;
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> GetRevenueBetweenDatesByPetService([FromQuery] string startDate, [FromQuery] string endDate)
        {
            var revenuePerServiceDTO = new List<RevenueSummaryPerPetServiceDTO>();

            var start = new DateTime();
            var end = new DateTime();


            if (!DateTime.TryParse(startDate, out start))
            {
                throw new ArgumentException("Start date is not a date.");
            }

            if(!DateTime.TryParse(endDate, out end))
            {
                throw new ArgumentException("End date is not a date.");
            }

            var revenuePerService = await _revenueSummaryRetrievalService.GetRevenueBetweenDatesByPetService(start, end);

            foreach(var service in revenuePerService)
            {
                revenuePerServiceDTO.Add(new RevenueSummaryPerPetServiceDTO(
                    PetServicesDTOMapper.ToDTOPetServices(service.PetService),
                    service.Count, 
                    service.GrossRevenuePerService, 
                    service.NetRevenuePerService));               
            }

            return Ok(revenuePerServiceDTO);
        }
    }
}
