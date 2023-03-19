using EmployeeManagementService.Domain.Exceptions;
using EmployeeManagementService.Domain.Mappers.DTO;
using EmployeeManagementService.Domain.Services;
using EmployeeManagementService.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EmployeeManagementService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class AEmployeeController : ControllerBase
    {
        protected readonly IEmployeeAuthService _employeeAuthService;

        protected readonly IEmployeeRetrievalService _employeeRetrievalService;

        protected readonly IEmployeeUpsertService _employeeUpsertService;

        public AEmployeeController(IEmployeeAuthService employeeAuthService,
            IEmployeeRetrievalService employeeRetrievalService,
            IEmployeeUpsertService employeeUpsertService)
        {
            _employeeAuthService = employeeAuthService;
            
            _employeeRetrievalService = employeeRetrievalService;
            
            _employeeUpsertService = employeeUpsertService;
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(long id)
        {
            var employee = await _employeeRetrievalService.GetEmployeeById(id);

            return Ok(EmployeeDTOMapper.ToDTOEmployee(employee));
        }

        [HttpGet("{username}/username")]
        public async Task<IActionResult> GetEmployeeByUsername(string username)
        {
            var employee = await _employeeRetrievalService.GetEmployeeByUsername(username);

            return Ok(EmployeeDTOMapper.ToDTOEmployee(employee));          
        }

        [HttpGet("employees")]
        public async Task<IActionResult> GetEmployeesForDropdown()
        {
            try
            {
                var employees = await _employeeRetrievalService.GetEmployeesForDropdown();

                return Ok(EmployeeDTOMapper.ToEmployeeDTOForDropdown(employees));
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
                var loginEmployee = await _employeeAuthService.EmployeeLogIn(employee.Username, employee.Password);

                return Ok(new { Id = loginEmployee.Id, FirstName = loginEmployee.FirstName, Role = loginEmployee.Role });
            }     
            catch (EmployeeIsLockedException)
            {
                return BadRequest("Employee is locked. Contact Admin to get unlocked.");
            }            
        }

        [HttpPatch("{id}/logout")]
        public async Task<IActionResult> EmployeeLogout(long id)
        {
            await _employeeAuthService.EmployeeLogout(id);

            return Ok();            
        }

        [HttpPatch("password")]
        public async Task<IActionResult> UpdatePassword([FromBody] PasswordDTO newPassword)
        {
            await _employeeUpsertService.UpdatePassword(newPassword.Id, newPassword.NewPassword);

            return Ok();          
        }

        public abstract Task<IActionResult> UpdateEmployeeInformation([FromBody] EmployeeDTO employee);
    }
}
