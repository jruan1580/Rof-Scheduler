using EmployeeManagementService.Domain.Mappers.DTO;
using EmployeeManagementService.Domain.Services;
using EmployeeManagementService.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RofShared.Exceptions;
using RofShared.FilterAttributes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeManagementService.API.Controllers
{
    [CookieActionFilter]
    [Authorize(Roles = "Administrator,Internal")]
    [ApiController]
    public class AdminController : AEmployeeController
    {
        private readonly IEmployeeService _employeeService;

        public AdminController(IEmployeeService employeeService) 
            : base(employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees([FromQuery] int page, [FromQuery] int offset, [FromQuery] string keyword)
        {
            try
            {
                var employeeList = new List<EmployeeDTO>();

                var result = await _employeeService.GetAllEmployeesByKeyword(page, offset, keyword);

                foreach (var employee in result.Employees)
                {
                    employeeList.Add(EmployeeDTOMapper.ToDTOEmployee(employee));
                }

                return Ok(new { employees = employeeList, totalPages = result.TotalPages});
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
 
        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeDTO employee)
        {
            try
            {
                await _employeeService.CreateEmployee(EmployeeDTOMapper.FromDTOEmployee(employee), employee.Password);

                return StatusCode(201);
            }
            catch(ArgumentException argEx)
            {
                return BadRequest(argEx.Message);
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
            catch (EntityNotFoundException)
            {
                return NotFound($"Employee not found");
            }
            catch(ArgumentException argEx)
            {
                return BadRequest(argEx.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPatch("{id}/locked")]
        public async Task<IActionResult> ResetLockedStatus(long id)
        {
            try
            {
                await _employeeService.ResetEmployeeFailedLoginAttempt(id);

                return Ok();
            }
            catch (EntityNotFoundException)
            {
                return NotFound($"Employee with id: {id} not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPatch("{id}/activate")]
        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> UpdateEmployeeStatus(long id)
        {
            try
            {
                var active = (Request.Path.Value.Contains("deactivate")) ? false : true;

                await _employeeService.UpdateEmployeeActiveStatus(id, active);

                return Ok();
            }
            catch (EntityNotFoundException)
            {
                return NotFound($"Employee with id: {id} not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployeeById(long id)
        {
            try
            {
                await _employeeService.DeleteEmployeeById(id);

                return Ok();
            }
            catch (EntityNotFoundException)
            {
                return NotFound($"Employee with id: {id} not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
