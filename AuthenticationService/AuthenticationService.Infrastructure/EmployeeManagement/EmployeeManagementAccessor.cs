using AuthenticationService.Infrastructure.EmployeeManagement.Models;
using AuthenticationService.Infrastructure.Shared;
using AuthenticationService.Infrastructure.Shared.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Infrastructure.EmployeeManagement
{
    public interface IEmployeeManagementAccessor
    {
        Task<bool> CheckIfEmployee(string username, string token);
        Task<EmployeeLoginResponse> Login(string username, string password, string token);
        Task<LogoutResponse> Logout(long userId, string relativeUrl, string token);
    }

    public class EmployeeManagementAccessor : IEmployeeManagementAccessor
    {
        private readonly string _employeeManagementBaseUrl;
        private readonly IHttpClientFactory _httpClientFactory;

        public EmployeeManagementAccessor(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _employeeManagementBaseUrl = configuration.GetSection("EmployeeManagement:URL").Value;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> CheckIfEmployee(string username, string token)
        {
            using(var httpClient = _httpClientFactory.CreateClient())
            {
                var url = $"{_employeeManagementBaseUrl}/api/employee/{username}/username";

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await httpClient.GetAsync(url);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return false;
                }

                await Utilities.ParseResponse(response);

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
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var url = $"{_employeeManagementBaseUrl}/api/employee/login";                

                var body = new { Username = username, Password = password };
                var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

                var response = await httpClient.PatchAsync(url, content);

                return await Utilities.ParseResponse<EmployeeLoginResponse>(response);               
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
        public async Task<LogoutResponse> Logout(long userId, string relativeUrl, string token)
        {
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var url = $"{_employeeManagementBaseUrl}{relativeUrl}";

                var response = await httpClient.PatchAsync(url, null);

                //if not found, return no response
                if(response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }

                await Utilities.ParseResponse(response);

                return new LogoutResponse(userId, true);
            }
        }
    }
}
