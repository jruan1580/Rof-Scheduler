using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AuthenticationService.Infrastructure.Shared
{
    public static class Utilities
    {
        public static async Task<T> ParseResponse<T>(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return default(T);
            }

            var strContent = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new ArgumentException(strContent);
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(strContent);
            }

            return JsonConvert.DeserializeObject<T>(strContent);
        }

        public static async Task ParseResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            var strContent = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new ArgumentException(strContent);
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(strContent);
            }
        }
    }
}
