using AuthenticationService.Domain.Model;
using AuthenticationService.DTO.Accessors;

namespace AuthenticationService.Domain.Mappers
{
    public static class LoginResponseDtoMapper
    {
        public static BasicUserInfo MapEmployeeLoginResponseToBasicUserInfo(EmployeeLoginResponse employeeLoginResponse)
        {
            if (employeeLoginResponse == null)
            {
                return null;
            }

            return new BasicUserInfo()
            {
                Id = employeeLoginResponse.Id,
                FirstName = employeeLoginResponse.FirstName,
                Role = employeeLoginResponse.Role
            };
        }

        public static BasicUserInfo MapClientLoginResponseToBasicUserInfo(LoginResponse loginResponse)
        {
            if (loginResponse == null)
            {
                return null;
            }

            return new BasicUserInfo()
            {
                Id = loginResponse.Id,
                FirstName = loginResponse.FirstName,
                Role = "Client"
            };
        }
    }
}
