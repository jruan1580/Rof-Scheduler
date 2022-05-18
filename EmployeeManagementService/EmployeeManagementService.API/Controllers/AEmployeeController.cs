using EmployeeManagementService.API.Authentication;
using EmployeeManagementService.API.DTO;
using EmployeeManagementService.API.DTOMappers;
using EmployeeManagementService.Domain.Exceptions;
using EmployeeManagementService.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EmployeeManagementService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class AEmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly ITokenHandler _tokenHandler;        

        public AEmployeeController(IEmployeeService employeeService, ITokenHandler tokenHandler)
        {
            _employeeService = employeeService;
            _tokenHandler = tokenHandler;
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

        [HttpGet("{username}/username")]
        public async Task<IActionResult> GetEmployeeByUsername(string username)
        {
            try
            {
                var employee = await _employeeService.GetEmployeeByUsername(username);

                if (employee == null)
                {
                    return NotFound($"Employee with username: {username} not found.");
                }

                return Ok(EmployeeDTOMapper.ToDTOEmployee(employee));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPatch("login")]
        public async Task<IActionResult> EmployeeLogin([FromBody] EmployeeDTO employee)
        {
            try
            {
                var loginEmployee = await _employeeService.EmployeeLogIn(employee.Username, employee.Password);

                return Ok(new { Id = loginEmployee.Id, FirstName = loginEmployee.FirstName, Role = loginEmployee.Role });
            }
            catch (EmployeeNotFoundException)
            {
                return NotFound("Employee not found");
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

        [HttpPatch("logout/{id}")]
        public async Task<IActionResult> EmployeeLogout(long id)
        {
            try
            {
                await _employeeService.EmployeeLogout(id);               

                return Ok();
            }
            catch(EmployeeNotFoundException)
            {
                return NotFound("Employee not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPatch("password/update")]
        public async Task<IActionResult> UpdatePassword([FromBody] PasswordDTO newPassword)
        {
            try
            {
                await _employeeService.UpdatePassword(newPassword.Id, newPassword.NewPassword);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        public abstract Task<IActionResult> UpdateEmployeeInformation([FromBody] EmployeeDTO employee);
    }
}
