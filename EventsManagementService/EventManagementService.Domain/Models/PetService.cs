using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagementService.Domain.Models
{
    public class PetService
    {
        public short Id { get; set; }
        
        public string ServiceName { get; set; }
        
        public decimal Price { get; set; }
        
        public string Description { get; set; }
    }
}
