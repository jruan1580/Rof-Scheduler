namespace DatamartManagementService.Domain.Models.RofSchedulerModels
{
    public class HolidayRates
    {
        public int Id { get; set; }
        
        public short PetServiceId { get; set; }
        
        public short HolidayId { get; set; }
        
        public decimal HolidayRate { get; set; }
    }
}
