using AuthenticationService.DTO.Accessors;
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

        public ClientManagementAccessor(IConfiguration configuration, IHttpClientFactory httpClientFactory)
            :base(httpClientFactory)
        {
            _clientManagementBaseUrl = configuration.GetSection("ClientManagement:URL").Value;
        }

        public async Task<LoginResponse> Login(string username, string password, string token)
        {
            var url = $"{_clientManagementBaseUrl}/api/client/login";

            var body = new { Username = username, Password = password };

            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            return await ExecutePatchRequestAndValidateAndParseResponse<LoginResponse>(url, token, content);                      
        }

        public async Task Logout(long userId, string token)
        {
            var url = $"{_clientManagementBaseUrl}/api/client/{userId}/logout";

            await ExecutePatchRequestAndValidateResponse(url, token, null);            
        }       
    }
}
