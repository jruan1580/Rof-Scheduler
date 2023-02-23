﻿using AuthenticationService.Infrastructure.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Infrastructure.ClientManagement
{
    public interface IClientManagementAccessor
    {
        Task<LoginResponse> Login(string username, string password, string token);
        Task Logout(long userId, string token);
    }

    public class ClientManagementAccessor : ApiAccessor, IClientManagementAccessor
    {
        private readonly string _clientManagementBaseUrl;
        private readonly IHttpClientFactory _httpClientFactory;

        public ClientManagementAccessor(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;            
            _clientManagementBaseUrl = configuration.GetSection("ClientManagement:URL").Value;
        }

        public async Task<LoginResponse> Login(string username, string password, string token)
        {
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                AddAuthHeader(httpClient, token);

                var url = $"{_clientManagementBaseUrl}/api/client/login";

                var body = new { Username = username, Password = password };

                var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

                var response = await httpClient.PatchAsync(url, content);

                return await ValidateAndParseResponse<LoginResponse>(response);                
            }
        }

        public async Task Logout(long userId, string token)
        {
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                AddAuthHeader(httpClient, token);

                var url = $"{_clientManagementBaseUrl}/api/client/{userId}/logout";                

                var response = await httpClient.PatchAsync(url, null);

                await ValidateResponse(response);
            }
        }       
    }
}
