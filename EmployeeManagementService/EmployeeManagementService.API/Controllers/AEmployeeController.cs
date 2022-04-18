using EmployeeManagementService.API.Authentication;
using EmployeeManagementService.API.DTO;
using EmployeeManagementService.API.DTOMappers;
using EmployeeManagementService.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EmployeeManagementService.API.Controllers
{
    [ApiController]
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

                var token = _tokenHandler.GenerateTokenForUserAndRole(loginEmployee.Role);

                if (loginEmployee.Role == "Administrator")
                {
                    Response.Cookies.Append("X-Access-Token-Admin", token, new CookieOptions() { HttpOnly = true, Expires = DateTimeOffset.Now.AddMinutes(30) });
                }
                else
                {
                    Response.Cookies.Append("X-Access-Token-Employee", token, new CookieOptions() { HttpOnly = true, Expires = DateTimeOffset.Now.AddMinutes(30) });
                }                

                return Ok(new { accessToken = token, Id = loginEmployee.Id });
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
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        public abstract Task<IActionResult> UpdateEmployeeInformation([FromBody] EmployeeDTO employee);
    }
}
