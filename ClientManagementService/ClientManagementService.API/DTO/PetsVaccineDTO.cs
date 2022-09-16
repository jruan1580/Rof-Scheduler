namespace ClientManagementService.API.DTO
{
    public class PetsVaccineDTO : VaccineDTO
    {
        public long PetsVaccineId { get;set; }

        public bool Inoculated { get; set; }   
    }
}
