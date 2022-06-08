using HowToWebApiAuthCode.Blazor.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Authentication;
using System.Text.Json;

namespace HowToWebApiAuthCode.Blazor.Data
{
    public class AuthService
    {
        private readonly HttpClient _tapkeyAuthServerClient;
        private readonly AppConfig _appConfiguration;
        private readonly NavigationManager _navigationManager;
        private readonly ProtectedLocalStorage _localStorage;

        public AuthService(
            IHttpClientFactory httpClientFactory,
            AppConfig appConfiguration,
            NavigationManager navigationManager,
            ProtectedLocalStorage localStorage)
        {
            _appConfiguration = appConfiguration ?? throw new ArgumentNullException(nameof(appConfiguration));
            _navigationManager = navigationManager;
            _tapkeyAuthServerClient = httpClientFactory.CreateClient(AppConstants.TapkeyAuthorizationServerClient);
            _localStorage = localStorage;
        }

        public async Task Login()
        {
            var stateBytes = new byte[16];
            System.Security.Cryptography.RandomNumberGenerator.Create().GetBytes(stateBytes);
            var state = Convert.ToBase64String(stateBytes);

            var authorizeArgs = new Dictionary<string, string>
            {
                {"client_id", _appConfiguration.ClientId},
                {"scope", "read:owneraccounts read:core:entities write:core:entities write:grants"},
                {"redirect_uri", "https://localhost:7259/Auth/LoginCallback"},
                {"response_type", "code"},
                {"state", state},
            };

            await _localStorage.SetAsync(AppConstants.StateKey, state);

            var content = new FormUrlEncodedContent(authorizeArgs);
            var contentAsString = await content.ReadAsStringAsync();

            var url = $"{AppConstants.TapkeyAuthorizationServerUrl}/{AppConstants.TapkeyAuthorizationEndpointPath}?{contentAsString}";
            _navigationManager.NavigateTo(url);
        }

        public async Task LoginCallback(string code, string state)
        {
            var previousState = await _localStorage.GetAsync<string>(AppConstants.StateKey);
            if (!previousState.Success || previousState.Value != state)
            {
                throw new AuthenticationException();
            }

            var tokenRequest = new Dictionary<string, string>
            {
                {"client_id", _appConfiguration.ClientId},
                {"client_secret", _appConfiguration.ClientSecret},
                {"grant_type", "authorization_code"},
                {"redirect_uri", "https://localhost:7259/Auth/LoginCallback"},
                {"code", code}
            };

            var postContent = new FormUrlEncodedContent(tokenRequest);

            var response = await _tapkeyAuthServerClient.PostAsync($"{AppConstants.TapkeyTokenEndpointPath}", postContent);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Problem while requesting token: {responseString}");

            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseString);
            await _localStorage.SetAsync(AppConstants.AccessTokenKey, tokenResponse.access_token);
            _navigationManager.NavigateTo("/owners");
        }
    }
}