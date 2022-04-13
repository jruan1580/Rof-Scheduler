using System;
using System.Collections.Generic;

#nullable disable

namespace EmployeeManagementService.Infrastructure.Persistence.Entities
{
    public partial class Country
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
