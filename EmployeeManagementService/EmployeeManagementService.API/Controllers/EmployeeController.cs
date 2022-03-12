using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagementService.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementService.API.Controllers
{
    [Route("api/employee")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet("all/{page}/{offset}")]
        public async Task<IActionResult> GetAllEmployees(int page, int offset)
        {
            try
            {
                return Ok(await _employeeService.GetAllEmployees(page, offset));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("one/{id}")]
        public async Task<IActionResult> GetEmployeeById(long id)
        {
            try
            {
                return Ok(await _employeeService.GetEmployeeById(id));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("one/{username}")]
        public async Task<IActionResult> GetEmployeeById(string username)
        {
            try
            {
                return Ok(await _employeeService.GetEmployeeByUsername(username));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}