using DatamartManagementService.Domain;
using DatamartManagementService.Domain.Mappers.DTO;
using DatamartManagementService.Domain.Models.RofDatamartModels;
using DatamartManagementService.DTO;
using DatamartManagementService.DTO.RofSchedulerDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        public async Task<IActionResult> GetRevenueBetweenDatesByPetService([FromBody] DateTime startDate, [FromBody] DateTime endDate)
        {
            var revenuePerServiceDTO = new List<RevenueSummaryPerPetServiceDTO>();

            var revenuePerService = await _revenueSummaryRetrievalService.GetRevenueBetweenDatesByPetService(startDate, endDate);

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
