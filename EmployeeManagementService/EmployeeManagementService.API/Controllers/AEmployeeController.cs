using EmployeeManagementService.Domain.Exceptions;
using EmployeeManagementService.Domain.Mappers.DTO;
using EmployeeManagementService.Domain.Services;
using EmployeeManagementService.DTO;
using Microsoft.AspNetCore.Mvc;
using RofShared.Exceptions;
using System;
using System.Threading.Tasks;

namespace EmployeeManagementService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            catch (EntityNotFoundException)
            {
                return NotFound($"Employee with id: {id} not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{username}/username")]
        public async Task<IActionResult> GetEmployeeByUsername(string username)
        {
            try
            {
                var employee = await _employeeService.GetEmployeeByUsername(username);

                return Ok(EmployeeDTOMapper.ToDTOEmployee(employee));
            }
            catch (EntityNotFoundException)
            {
                return NotFound($"Employee with username: {username} not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPatch("login")]
        public async Task<IActionResult> EmployeeLogin([FromBody] EmployeeDTO employee)
        {
            try
            {
                var loginEmployee = await _employeeService.EmployeeLogIn(employee.Username, employee.Password);

                return Ok(new { Id = loginEmployee.Id, FirstName = loginEmployee.FirstName, Role = loginEmployee.Role });
            }
            catch (EntityNotFoundException)
            {
                return NotFound("Employee not found");
            }
            catch (EmployeeIsLockedException)
            {
                return BadRequest("Employee is locked. Contact Admin to get unlocked.");
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

        [HttpPatch("{id}/logout")]
        public async Task<IActionResult> EmployeeLogout(long id)
        {
            try
            {
                await _employeeService.EmployeeLogout(id);               

                return Ok();
            }
            catch(EntityNotFoundException)
            {
                return NotFound("Employee not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPatch("password")]
        public async Task<IActionResult> UpdatePassword([FromBody] PasswordDTO newPassword)
        {
            try
            {
                await _employeeService.UpdatePassword(newPassword.Id, newPassword.NewPassword);

                return Ok();
            }
            catch (EntityNotFoundException)
            {
                return NotFound($"Employee with id: {newPassword.Id} not found");
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

        public abstract Task<IActionResult> UpdateEmployeeInformation([FromBody] EmployeeDTO employee);
    }
}
