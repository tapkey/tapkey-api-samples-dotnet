
using HowToWebApiAuthCode.Blazor.Models;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Text.Json;

namespace HowToWebApiAuthCode.Blazor
{
    public class TapkeyApi
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ProtectedLocalStorage _localStorage;

        public TapkeyApi(IHttpClientFactory clientFactory, ProtectedLocalStorage localStorage)
        {
            _localStorage = localStorage;
            _clientFactory = clientFactory;
        }

        public async Task<OwnerAccount[]> GetAllOwnerAccounts()
        {
            var client = await GetHttpClient();
            var ownersJsonResponse = await client.GetStringAsync("/api/v1/Owners");
            return JsonSerializer.Deserialize<OwnerAccount[]>(ownersJsonResponse);  
        }

        public async Task<BoundLock[]> GetAllBoundLocks(string ownerAccountId)
        {
            var client = await GetHttpClient();
            var boundLocksJsonResponse = await client.GetStringAsync($"/api/v1/Owners/{ownerAccountId}/BoundLocks");
            return JsonSerializer.Deserialize<BoundLock[]>(boundLocksJsonResponse);
        }

        public async Task<bool> CreateGrant(string ownerAccountId, Grant grant)
        {
            var client = await GetHttpClient();
            var createGrantResponse = await client.PutAsJsonAsync($"/api/v1/Owners/{ownerAccountId}/Grants", grant);
            return createGrantResponse.IsSuccessStatusCode;
        }

        private async Task<HttpClient> GetHttpClient()
        {
           var client = _clientFactory.CreateClient();
           client.BaseAddress = new Uri(AppConstants.TapkeyApiBaseUrl);
           var accessToken = await _localStorage.GetAsync<string>(AppConstants.AccessTokenKey);
           client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken.Value);
           return client;
        }
    }
}
