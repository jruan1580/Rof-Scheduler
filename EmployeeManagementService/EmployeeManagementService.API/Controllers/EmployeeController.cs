using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmployeeManagementService.API.Authentication;
using EmployeeManagementService.API.DTO;
using EmployeeManagementService.API.DTOMappers;
using EmployeeManagementService.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementService.API.Controllers
{
    [Authorize(Roles = "Employee")]
    [ApiController]
    public class EmployeeController : AEmployeeController
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService, ITokenHandler tokenHandler) 
            : base(employeeService, tokenHandler)
        {
            _employeeService = employeeService;
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
    }
}