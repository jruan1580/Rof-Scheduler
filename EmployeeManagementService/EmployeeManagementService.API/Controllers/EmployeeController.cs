using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmployeeManagementService.API.DTO;
using EmployeeManagementService.API.DTOMappers;
using EmployeeManagementService.Domain.Services;
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
                var employeeList = new List<EmployeeDTO>();

                var employees = await _employeeService.GetAllEmployees(page, offset);

                foreach(var employee in employees)
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

        [HttpGet("one/{id}")]
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

        [HttpGet("one/{username}")]
        public async Task<IActionResult> GetEmployeeById(string username)
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
        public async Task<IActionResult> UpdateEmployeeInformation([FromBody] EmployeeDTO employee)
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
    }
}