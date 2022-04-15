using EmployeeManagementService.API.Authentication;
using EmployeeManagementService.API.DTO;
using EmployeeManagementService.API.DTOMappers;
using EmployeeManagementService.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeManagementService.API.Controllers
{
    [Authorize(Roles = "Administrator")]
    [Route("api/admin")]
    [ApiController]
    public class AdminController : AEmployeeController
    {
        private readonly IEmployeeService _employeeService;

        public AdminController(IEmployeeService employeeService, ITokenHandler tokenHandler) 
            : base(employeeService, tokenHandler)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees([FromQuery] int page, [FromQuery] int offset)
        {
            try
            {
                var employeeList = new List<EmployeeDTO>();

                var employees = await _employeeService.GetAllEmployees(page, offset);

                foreach (var employee in employees)
                {
                    employeeList.Add(EmployeeDTOMapper.ToDTOEmployee(employee));
                }

                return Ok(employeeList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
 

        [HttpPost("create")]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeDTO employee)
        {
            try
            {
                await _employeeService.CreateEmployee(EmployeeDTOMapper.FromDTOEmployee(employee), employee.Password);

                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("info")]
        public override async Task<IActionResult> UpdateEmployeeInformation([FromBody] EmployeeDTO employee)
        {
            try
            {
                await _employeeService.UpdateEmployeeInformation(EmployeeDTOMapper.FromDTOEmployee(employee));

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPatch("reset/{id}/locked")]
        public async Task<IActionResult> ResetLockedStatus(long id)
        {
            try
            {
                await _employeeService.ResetEmployeeFailedLoginAttempt(id);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
