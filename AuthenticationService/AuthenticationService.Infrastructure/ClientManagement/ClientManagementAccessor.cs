using AuthenticationService.Infrastructure.ClientManagement.Models;
using AuthenticationService.Infrastructure.Shared.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Infrastructure.ClientManagement
{
    public interface IClientManagementAccessor
    {
        Task<ClientLoginResponse> Login(string username, string password);
        Task<LogoutResponse> Logout(long userId, string token);
    }

    public class ClientManagementAccessor : IClientManagementAccessor
    {
        private readonly string _clientManagementBaseUrl;
        private readonly IHttpClientFactory _httpClientFactory;

        public ClientManagementAccessor(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _clientManagementBaseUrl = configuration.GetSection("ClientManagement:URL").Value;
        }

        public async Task<ClientLoginResponse> Login(string username, string password)
        {
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                var url = $"{_clientManagementBaseUrl}/api/client/login";

                var body = new { Username = username, Password = password };
                var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

                var response = await httpClient.PatchAsync(url, content);

                var contentAsStr = await response.Content.ReadAsStringAsync();

                //return null when username is not found
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to login with error message: {contentAsStr}");
                }

                return JsonConvert.DeserializeObject<ClientLoginResponse>(contentAsStr);
            }
        }

        public async Task<LogoutResponse> Logout(long userId, string token)
        {
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                var url = $"{_clientManagementBaseUrl}/api/client/{userId}/logout";

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await httpClient.PatchAsync(url, null);

                //if not found, return no response
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }

                if (!response.IsSuccessStatusCode)
                {
                    var errMsg = await response.Content.ReadAsStringAsync();

                    throw new Exception($"Failed to login with error message: {errMsg}");
                }

                return new LogoutResponse(userId, true);
            }
        }
    }
}
