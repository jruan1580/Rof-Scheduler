using System;
using System.Collections.Generic;

#nullable disable

namespace ClientManagementService.Infrastructure.Persistence.Entities
{
    public partial class Country
    {
        public Country()
        {
            Clients = new HashSet<Client>();
        }

        public long Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Client> Clients { get; set; }
    }
}
