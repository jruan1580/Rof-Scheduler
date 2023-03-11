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
        private readonly IHttpClientFactory _httpClientFactory;
        public ApiAccessor(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        protected HttpClient GetHttpClient => _httpClientFactory.CreateClient();
        
        protected async Task ExecuteGetRequestAndValidateResponse(string url, string token)
        {
            var httpClient = GetHttpClient;

            AddAuthHeader(httpClient, token);

            var response = await httpClient.GetAsync(url);

            await ValidateResponse(response);
        }

        protected async Task<T> ExecutePatchRequestAndValidateAndParseResponse<T>(string url, string token, StringContent content)
        {
            var httpClient = GetHttpClient;

            AddAuthHeader(httpClient, token);

            var response = await httpClient.PatchAsync(url, content);

            return await ValidateAndParseResponse<T>(response);
        }

        protected async Task ExecutePatchRequestAndValidateResponse(string url, string token, StringContent content)
        {
            var httpClient = GetHttpClient;

            AddAuthHeader(httpClient, token);

            var response = await httpClient.PatchAsync(url, content);

            await ValidateResponse(response);
        }

        private void AddAuthHeader(HttpClient httpClient, string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return;
            }

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        private async Task<T> ValidateAndParseResponse<T>(HttpResponseMessage response)
        {
            await CheckResponseStatus(response);

            var strContent = await response.Content?.ReadAsStringAsync() ?? string.Empty;

            return JsonConvert.DeserializeObject<T>(strContent);
        }

        private async Task ValidateResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            await CheckResponseStatus(response);
        }

        private async Task CheckResponseStatus(HttpResponseMessage response)
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
