using EmployeeManagementService.DTO;
using System.Text;
using DbEmployee = EmployeeManagementService.Infrastructure.Persistence.Entities.Employee;
using DomainEmployee = EmployeeManagementService.Domain.Models.Employee;

namespace EmployeeManagementService.Test
{
    public static class EmployeeCreator
    {
        public static DbEmployee GetDbEmployee(byte[] encryptedPassword)
        {
            return new DbEmployee()
            {
                Id = 1,
                CountryId = 1,
                FirstName = "John",
                LastName = "Doe",
                Ssn = "123-45-6789",
                Username = "jdoe",
                EmailAddress = "test@email.com",
                Password = encryptedPassword,
                Role = "Employee",
                TempPasswordChanged = false,
                AddressLine1 = "123 Abc St",
                AddressLine2 = "",
                City = "Oakland",
                State = "CA",
                ZipCode = "12345"
            };
        }

        public static DomainEmployee GetDomainEmployee(byte[] encryptedPassword)
        {
            return new DomainEmployee()
            {
                CountryId = 1,
                FirstName = "John",
                LastName = "Doe",
                Ssn = "123-45-6789",
                Username = "jdoe",
                Email = "test@email.com",
                PhoneNumber = "9998887776",
                Role = "Employee",
                Password = encryptedPassword,
                Address = new Domain.Models.Address
                {
                    AddressLine1 = "123 Abc St",
                    AddressLine2 = "",
                    City = "Oakland",
                    State = "CA",
                    ZipCode = "12345"
                }
            };
        }

        public static EmployeeDTO GetEmployeeDTO(string role)
        {
            return new EmployeeDTO()
            {
                FirstName = "John",
                LastName = "Doe",
                Ssn = "123-45-6789",
                Username = "jdoe",
                Role = role,
                Address = new AddressDTO
                {
                    AddressLine1 = "123 Abc St",
                    AddressLine2 = "",
                    City = "Oakland",
                    State = "CA",
                    ZipCode = "12345"
                }
            };
        }
    }
}
