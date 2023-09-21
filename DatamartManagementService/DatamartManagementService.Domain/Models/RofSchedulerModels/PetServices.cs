﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DatamartManagementService.Domain.Models.RofSchedulerModels
{
    public class PetServices
    {
        public short Id { get; set; }
        
        public string ServiceName { get; set; }
        
        public decimal Price { get; set; }
        
        public decimal EmployeeRate { get; set; }
        
        public int Duration { get; set; }
        
        public string TimeUnit { get; set; }
    }
}
