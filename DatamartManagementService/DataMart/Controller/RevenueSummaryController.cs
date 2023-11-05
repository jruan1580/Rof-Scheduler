using DatamartManagementService.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> GetRevenueBetweenDatesByPetService([FromBody] int month, [FromBody] int year)
        {
            
        }
    }
}
