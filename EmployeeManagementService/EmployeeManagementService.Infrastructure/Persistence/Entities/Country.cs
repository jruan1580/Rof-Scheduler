using System;
using System.Collections.Generic;

#nullable disable

namespace EmployeeManagementService.Infrastructure.Persistence.Entities
{
    public partial class Country
    {
        public Country()
        {
            Employees = new HashSet<Employee>();
        }

        public long Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }
    }
}
