using Newtonsoft.Json;
using RofShared.Exceptions;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AuthenticationService.Infrastructure
{
    public class ApiAccessor
    {
        protected void AddAuthHeader(HttpClient httpClient, string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return;
            }

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        protected async Task<T> ValidateAndParseResponse<T>(HttpResponseMessage response)
        {
            await CheckResponseStatus(response);

            var strContent = await response.Content?.ReadAsStringAsync() ?? string.Empty;

            return JsonConvert.DeserializeObject<T>(strContent);
        }

        protected async Task ValidateResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            await CheckResponseStatus(response);
        }

        private static async Task CheckResponseStatus(HttpResponseMessage response)
        {            
            var strContent = (response.Content == null) 
                ? string.Empty
                : await response.Content?.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new ArgumentException(strContent);
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new EntityNotFoundException(string.Empty);
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(strContent);
            }           
        }
    }
}
