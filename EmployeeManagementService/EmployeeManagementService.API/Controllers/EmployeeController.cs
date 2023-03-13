﻿using System;
using System.Threading.Tasks;
using EmployeeManagementService.Domain.Mappers.DTO;
using EmployeeManagementService.Domain.Services;
using EmployeeManagementService.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RofShared.Exceptions;
using RofShared.FilterAttributes;

namespace EmployeeManagementService.API.Controllers
{
    [CookieActionFilter]
    [Authorize(Roles = "Employee,Internal")]
    [ApiController]
    public class EmployeeController : AEmployeeController
    {
        public EmployeeController(IEmployeeAuthService employeeAuthService,
            IEmployeeRetrievalService employeeRetrievalService,
            IEmployeeUpsertService employeeUpsertService)
        : base(employeeAuthService, employeeRetrievalService, employeeUpsertService)
        { }            

        [HttpPut("info")]
        public override async Task<IActionResult> UpdateEmployeeInformation([FromBody] EmployeeDTO employee)
        {
            try
            {
                await _employeeUpsertService.UpdateEmployeeInformation(EmployeeDTOMapper.FromDTOEmployee(employee));

                return Ok();
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException argEx)
            {
                return BadRequest(argEx.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }              
    }
}