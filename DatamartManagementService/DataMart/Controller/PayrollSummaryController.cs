using DatamartManagementService.Domain.Mappers.DTO;
using DatamartManagementService.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using DatamartManagementService.Domain;

namespace DataMart.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayrollSummaryController : ControllerBase
    {
        private readonly IPayrollSummaryRetrievalService _payrollSummaryRetrievalService;

        public PayrollSummaryController(IPayrollSummaryRetrievalService payrollSummaryRetrievalService)
        {
            _payrollSummaryRetrievalService = payrollSummaryRetrievalService;
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> GetPayrollBetweenDatesByEmployee([FromQuery] string startDate, [FromQuery] string endDate, 
            [FromQuery] string firstName, [FromQuery] string lastName, [FromQuery] int page)
        {
            var payrollPerEmployeeDTO = new List<PayrollSummaryPerEmployeeDTO>();

            var start = new DateTime();
            var end = new DateTime();

            if (!DateTime.TryParse(startDate, out start))
            {
                throw new ArgumentException("Start date is not a date.");
            }

            if (!DateTime.TryParse(endDate, out end))
            {
                throw new ArgumentException("End date is not a date.");
            }

            var payrollSummaryWithPages = await _payrollSummaryRetrievalService.GetPayrollSummaryPerEmployeeByDate(firstName, lastName,
                start, end, page);

            foreach(var payrollSummaryPerEmployee in payrollSummaryWithPages.PayrollSummaryPerEmployee)
            {
                payrollPerEmployeeDTO.Add(PayrollSummaryPerEmployeeDTOMapper
                    .ToDTOPayrollSummaryPerEmployee(payrollSummaryPerEmployee));
            }

            return Ok(new PayrollSummaryWithTotalPagesDTO(payrollPerEmployeeDTO, payrollSummaryWithPages.TotalPages));
        }
    }
}
