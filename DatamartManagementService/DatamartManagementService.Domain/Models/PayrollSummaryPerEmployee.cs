namespace DatamartManagementService.Domain.Models
{
    public class PayrollSummaryPerEmployee
    {
        public PayrollSummaryPerEmployee(string firstName, string lastName, decimal totalPay)
        {
            FirstName = firstName;
            LastName = lastName;
            TotalPay = totalPay;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal TotalPay { get; set; }
    }
}
