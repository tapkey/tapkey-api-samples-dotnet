using HowToWebApiClientCredentials.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace HowToWebApiClientCredentials
{
    public class TapkeyApi
    {
        private HttpClient client;

        public TapkeyApi(string accessToken)
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            client.BaseAddress = new Uri("https://my.tapkey.com");
        }

        public async Task<OwnerAccount[]> GetAllOwnerAccounts()
        {
            var ownersJsonResponse = await client.GetStringAsync("/api/v1/Owners");
            return JsonSerializer.Deserialize<OwnerAccount[]>(ownersJsonResponse);  
        }

        public async Task<BoundLock[]> GetAllBoundLocks(string ownerAccountId)
        {
            var boundLocksJsonResponse = await client.GetStringAsync($"/api/v1/Owners/{ownerAccountId}/BoundLocks");
            return JsonSerializer.Deserialize<BoundLock[]>(boundLocksJsonResponse);
        }

        public async Task<bool> CreateGrant(string ownerAccountId, Grant grant)
        {
            var createGrantResponse = await client.PutAsJsonAsync<Grant>($"/api/v1/Owners/{ownerAccountId}/Grants", grant);
            return createGrantResponse.IsSuccessStatusCode;
        }
    }
}
