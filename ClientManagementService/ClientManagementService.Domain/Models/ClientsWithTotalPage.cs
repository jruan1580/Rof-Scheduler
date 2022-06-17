using System.Collections.Generic;

namespace ClientManagementService.Domain.Models
{
    public class ClientsWithTotalPage
    {
        public ClientsWithTotalPage(List<Client> clients, int totalPages)
        {
            Clients = clients;
            TotalPages = totalPages;
        }

        public List<Client> Clients { get; set; }
        public int TotalPages { get; set; }
    }
}
