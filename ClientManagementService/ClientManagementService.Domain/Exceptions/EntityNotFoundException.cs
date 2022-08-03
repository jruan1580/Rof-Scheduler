using System;

namespace ClientManagementService.Domain.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string entity) 
            : base($"{entity} was not found.")
        {

        }
    }
}
