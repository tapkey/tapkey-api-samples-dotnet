using System.Text.Json;

namespace HowToWebApiClientCredentials
{
    public class Authentication
    {
        public static async Task<TokenResponse> RetrieveAccessToken(string clientId, string clientSecret)
        {
            HttpClient client = new HttpClient();

            var parameters = new Dictionary<string, string>
            {
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "scope", "read:owneraccounts read:core:entities write:core:entities write:grants" },
                { "grant_type", "client_credentials" }
            };

            var response = await client.PostAsync("https://login.tapkey.com/connect/token", new FormUrlEncodedContent(parameters));
            var tokenResponseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Problem while requesting token: {tokenResponseString}");

            return JsonSerializer.Deserialize<TokenResponse>(tokenResponseString);
        }
    }
}
