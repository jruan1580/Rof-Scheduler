using System;
using System.Collections.Generic;
using System.Text;

namespace ClientManagementService.Domain.Models
{
    public class Breed
    {
        public long Id { get; set; }
        
        public string BreedName { get; set; }
        
        public string Type { get; set; }
    }
}
