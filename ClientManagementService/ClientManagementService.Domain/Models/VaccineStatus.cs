namespace ClientManagementService.Domain.Models
{
    /// <summary>
    /// Class is a vaccine with status
    /// True - has been vaccinated with corresponding vaccine
    /// False - has not been vaccinated with corresponding vaccine.
    /// </summary>
    public class VaccineStatus : Vaccine
    {
        public bool Inoculated { get; set; }
    }
}
