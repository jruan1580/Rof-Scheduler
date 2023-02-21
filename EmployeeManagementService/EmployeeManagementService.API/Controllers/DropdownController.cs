using EmployeeManagementService.Domain.Mappers.DTO;
using EmployeeManagementService.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RofShared.FilterAttributes;
using System;
using System.Threading.Tasks;

namespace EmployeeManagementService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [CookieActionFilter]
    [Authorize(Roles = "Administrator,Employee")]
    public class DropdownController : ControllerBase
    {
        private readonly IDropdownService _dropdownService;

        public DropdownController(IDropdownService dropdownService)
        {
            _dropdownService = dropdownService;
        }

        [HttpGet("employees")]
        public async Task<IActionResult> GetEmployees()
        {
            try
            {
                var employees = await _dropdownService.GetEmployees();

                return Ok(DropdownDTOMapper.ToEmployeeDTO(employees));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
