using System;

namespace ClientManagementService.Domain.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string message) 
            : base(message)
        {

        }
    }
}
