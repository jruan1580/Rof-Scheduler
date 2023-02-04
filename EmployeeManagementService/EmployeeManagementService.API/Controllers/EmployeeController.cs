using System;
using System.Threading.Tasks;
using EmployeeManagementService.Domain.Mappers.DTO;
using EmployeeManagementService.Domain.Services;
using EmployeeManagementService.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RofShared.Exceptions;
using RofShared.FilterAttributes;

namespace EmployeeManagementService.API.Controllers
{
    [CookieActionFilter]
    [Authorize(Roles = "Employee,Internal")]
    [ApiController]
    public class EmployeeController : AEmployeeController
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService) 
            : base(employeeService)
        {
            _employeeService = employeeService;
        }
            

        [HttpPut("info")]
        public override async Task<IActionResult> UpdateEmployeeInformation([FromBody] EmployeeDTO employee)
        {
            try
            {
                await _employeeService.UpdateEmployeeInformation(EmployeeDTOMapper.FromDTOEmployee(employee));

                return Ok();
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
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