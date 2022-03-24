using EmployeeManagementService.API.DTO;
using EmployeeManagementService.API.DTOMappers;
using EmployeeManagementService.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EmployeeManagementService.API.Controllers
{
    [ApiController]
    public abstract class AEmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public AEmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(long id)
        {
            try
            {
                var employee = await _employeeService.GetEmployeeById(id);

                return Ok(EmployeeDTOMapper.ToDTOEmployee(employee));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> GetEmployeeByUsername(string username)
        {
            try
            {
                var employee = await _employeeService.GetEmployeeByUsername(username);

                return Ok(EmployeeDTOMapper.ToDTOEmployee(employee));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        public abstract Task<IActionResult> UpdateEmployeeInformation([FromBody] EmployeeDTO employee);
    }
}
