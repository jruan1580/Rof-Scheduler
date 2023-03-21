using System;

namespace EventManagementService.API.DTO
{
    public class EventDTO
    {
        public int Id { get; set; }

        public long EmployeeId { get; set; }

        public string EmployeeFullName { get; set; }

        public long PetId { get; set; }

        public string PetName { get; set; }

        public short PetServiceId { get; set; }

        public string PetServiceName { get; set; }

        public string EventStartTime { get; set; }

        public string EventEndTime { get; set; }

        public bool Completed { get; set; }
    }
}
