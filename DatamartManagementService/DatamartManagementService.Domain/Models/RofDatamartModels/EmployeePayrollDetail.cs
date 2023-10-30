using System;

namespace DatamartManagementService.Domain.Models.RofDatamartModels
{
    public class EmployeePayrollDetail
    {
        public long Id { get; set; }

        public long EmployeeId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public short PetServiceId { get; set; }

        public string PetServiceName { get; set; }

        public decimal EmployeePayForService { get; set; }

        public bool IsHolidayPay { get; set; }

        public int ServiceDuration { get; set; }

        public string ServiceDurationTimeUnit { get; set; }

        public long JobEventId { get; set; }

        public DateTime ServiceStartDateTime { get; set; }

        public DateTime ServiceEndDateTime { get; set; }
    }
}
