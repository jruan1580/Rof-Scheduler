using AuthenticationService.DTO.Accessors;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RofShared.Exceptions;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Infrastructure.EmployeeManagement
{
    public interface IEmployeeManagementAccessor
    {
        Task<bool> CheckIfEmployee(string username, string token);
        Task<EmployeeLoginResponse> Login(string username, string password, string token);
        Task Logout(long userId, string relativeUrl, string token);
    }

    public class EmployeeManagementAccessor : ApiAccessor, IEmployeeManagementAccessor
    {
        private readonly string _employeeManagementBaseUrl;

        public EmployeeManagementAccessor(IConfiguration configuration, IHttpClientFactory httpClientFactory)
            : base(httpClientFactory)
        {
            _employeeManagementBaseUrl = configuration.GetSection("EmployeeManagement:URL").Value;
        }

        public async Task<bool> CheckIfEmployee(string username, string token)
        {
            using(var httpClient = GetHttpClient)
            {
                var url = $"{_employeeManagementBaseUrl}/api/employee/{username}/username";

                AddAuthHeader(httpClient, token);

                try
                {
                    var response = await httpClient.GetAsync(url);

                    await ValidateResponse(response);
                }
                catch (EntityNotFoundException)
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Makes a call to EmployeeManagementService to log an employee or admin in.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>returns response if successful</returns>
        public async Task<EmployeeLoginResponse> Login(string username, string password, string token)
        {
            using (var httpClient = GetHttpClient)
            {
                AddAuthHeader(httpClient, token);

                var url = $"{_employeeManagementBaseUrl}/api/employee/login";                

                var body = new { Username = username, Password = password };

                var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

                return await PatchRequestAndValidateResponse<EmployeeLoginResponse>(url, content);                        
            }
        }

        /// <summary>
        /// Calls EmployeeManagementService to log admin or employee out.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <param name="token"></param>
        /// <returns>returns true if successful</returns>
        /// <exception cref="Exception"></exception>
        public async Task Logout(long userId, string relativeUrl, string token)
        {
            using (var httpClient = GetHttpClient)
            {
                AddAuthHeader(httpClient, token);

                var url = $"{_employeeManagementBaseUrl}{relativeUrl}";

                await PatchRequestAndValidateResponse(url, null);                
            }
        }
    }
}
