using EmployeeManagementService.Domain.Mappers.DTO;
using EmployeeManagementService.Domain.Services;
using EmployeeManagementService.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RofShared.FilterAttributes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeManagementService.API.Controllers
{
    [CookieActionFilter]
    [Authorize(Roles = "Administrator,Internal")]
    [ApiController]
    public class AdminController : AEmployeeController
    {
        public AdminController(IEmployeeAuthService employeeAuthService,
            IEmployeeRetrievalService employeeRetrievalService,
            IEmployeeUpsertService employeeUpsertService) 
        : base(employeeAuthService, employeeRetrievalService, employeeUpsertService)
        { }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees([FromQuery] int page, [FromQuery] int offset, [FromQuery] string keyword)
        {
            var employeeList = new List<EmployeeDTO>();

            var result = await _employeeRetrievalService.GetAllEmployeesByKeyword(page, offset, keyword);

            foreach (var employee in result.Employees)
            {
                employeeList.Add(EmployeeDTOMapper.ToDTOEmployee(employee));
            }

            return Ok(new { employees = employeeList, totalPages = result.TotalPages });
        }
 
        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeDTO employee)
        {
            await _employeeUpsertService.CreateEmployee(EmployeeDTOMapper.FromDTOEmployee(employee), employee.Password);

            return StatusCode(201);
        }

        [HttpPut("info")]
        public override async Task<IActionResult> UpdateEmployeeInformation([FromBody] EmployeeDTO employee)
        {
            await _employeeUpsertService.UpdateEmployeeInformation(EmployeeDTOMapper.FromDTOEmployee(employee));

            return Ok();
        }

        [HttpPatch("{id}/locked")]
        public async Task<IActionResult> ResetLockedStatus(long id)
        {
            await _employeeUpsertService.ResetEmployeeFailedLoginAttempt(id);

            return Ok();      
        }

        [HttpPatch("{id}/activate")]
        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> UpdateEmployeeStatus(long id)
        {
            var active = (Request.Path.Value.Contains("deactivate")) ? false : true;

            await _employeeUpsertService.UpdateEmployeeActiveStatus(id, active);

            return Ok();           
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployeeById(long id)
        {
            await _employeeUpsertService.DeleteEmployeeById(id);

            return Ok();
        }
    }
}
