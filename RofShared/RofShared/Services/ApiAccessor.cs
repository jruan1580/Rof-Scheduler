using Newtonsoft.Json;
using RofShared.Exceptions;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace RofShared.Services
{
    public class ApiAccessor
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ApiAccessor(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        protected HttpClient GetHttpClient => _httpClientFactory.CreateClient();

        protected async Task ExecuteRequestWithNoBody(string url, HttpMethod method, string token)
        {
            var response = await SendRequestWithNoBody(url, method, token);

            await ValidateResponse(response);
        }

        protected async Task<T> ExecuteRequestWithNoBody<T>(string url, HttpMethod method, string token)
        {
            var response = await SendRequestWithNoBody(url, method, token);

            await ValidateResponse(response);

            return await ParseResponse<T>(response);
        }

        protected async Task ExecuteRequestWithBody(string url, HttpMethod method, StringContent bodyContent, string token)
        {         
            var response = await SendRequestWithBody(url, method, bodyContent, token);

            await ValidateResponse(response);
        }

        protected async Task<T> ExecuteRequestWithBody<T>(string url, HttpMethod method, StringContent bodyContent, string token)
        {
            var response = await SendRequestWithBody(url, method, bodyContent, token);

            await ValidateResponse(response);

            return await ParseResponse<T>(response);
        }

        private async Task<HttpResponseMessage> SendRequestWithNoBody(string url, HttpMethod method, string token)
        {
            var httpClient = GetHttpClient;

            AddAuthHeader(httpClient, token);

            var request = new HttpRequestMessage(method, url);

            return await httpClient.SendAsync(request);
        }

        private async Task<HttpResponseMessage> SendRequestWithBody(string url, HttpMethod method, StringContent bodyContent, string token)
        {
            var httpClient = GetHttpClient;

            AddAuthHeader(httpClient, token);

            var request = new HttpRequestMessage(method, url);

            if (bodyContent != null)
            {
                request.Content = bodyContent;
            }

            return await httpClient.SendAsync(request);
        }

        private void AddAuthHeader(HttpClient httpClient, string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return;
            }

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        private async Task ValidateResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            await ThrowProperExceptionForUnsuccessfulResponse(response);
        }

        private async Task<T> ParseResponse<T>(HttpResponseMessage response)
        {
            var strContent = await GetResponseContentAsString(response);

            return JsonConvert.DeserializeObject<T>(strContent);
        }

        private async Task<string> GetResponseContentAsString(HttpResponseMessage response)
        {
            return await response.Content?.ReadAsStringAsync() ?? string.Empty;
        }

        private async Task ThrowProperExceptionForUnsuccessfulResponse(HttpResponseMessage response)
        {
            var errMsg = await GetResponseContentAsString(response);

            ThrowArgumentExceptionIfBadRequest(response, errMsg);

            ThrowEntityNotFoundExceptionIfNotFound(response, errMsg);

            ThrowGenericExceptionIfUnsuccessfulStatusCode(response, errMsg);
        }

        private void ThrowArgumentExceptionIfBadRequest(HttpResponseMessage response, string errMsg)
        {
            if (response.StatusCode != HttpStatusCode.BadRequest)
            {
                return;
            }

            throw new ArgumentException(errMsg);
        }

        private void ThrowEntityNotFoundExceptionIfNotFound(HttpResponseMessage response, string errMsg)
        {
            if (response.StatusCode != HttpStatusCode.NotFound)
            {
                return;                
            }

            throw new EntityNotFoundException(errMsg);
        }

        private void ThrowGenericExceptionIfUnsuccessfulStatusCode(HttpResponseMessage response, string errMsg)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            throw new Exception(errMsg);
        }
    }
}
