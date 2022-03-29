using System.Security.Cryptography;
using System.Text;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MVCAppAuthorizationCodePKCE.NET.Models;

namespace MVCAppAuthorizationCodePKCE.NET.Controllers
{
    public class AuthController : Controller
    {
        private readonly IMemoryCache _memoryCache;
        private readonly HttpClient _tapkeyAuthServerClient;
        private readonly AppConfiguration _appConfiguration;

        public AuthController(
            IMemoryCache memoryCache,
            IHttpClientFactory httpClientFactory,
            AppConfiguration appConfiguration)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _appConfiguration = appConfiguration ?? throw new ArgumentNullException(nameof(appConfiguration));

            // Get the HttpClient from the HttpClientFactory to talking with Tapkey Authorization Server
            _tapkeyAuthServerClient = httpClientFactory.CreateClient(AppConstants.TapkeyAuthorizationServerClient);
        }

        public async Task<IActionResult> Login()
        {
            var pkce = new Pkce
            {
                // uses the IdentityModel NuGet package to create a strongly random URL safe identifier
                // this will be our Code Verifier
                CodeVerifier = CryptoRandom.CreateUniqueId(32)
            };

            using (var sha256 = SHA256.Create())
            {
                // Here we create a hash of the code verifier
                var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(pkce.CodeVerifier));

                // and produce the "Code Challenge"  from it by base64Url encoding it.
                pkce.CodeChallenge = Base64Url.Encode(challengeBytes);
            }

            // Save the CodeVerifier in the memorycache so we are able to use it later
            // This is just for demonstration. Please consider saving this in a more robust solution.
            _memoryCache.Set(AppConstants.PKCECacheKey, pkce);

            // Build the authorize url
            var authorizeArgs = new Dictionary<string, string>
            {
                {"client_id", _appConfiguration.ClientId},
                {"scope", "read:core:entities read:owneraccounts offline_access"},
                {"redirect_uri", "https://localhost:44333/Auth/LoginCallback"},
                {"response_type", "code"},

                // Provide the Code Challenge along with the method (Sha256)
                {"code_challenge", pkce.CodeChallenge },
                {"code_challenge_method", "S256" }
            };

            var content = new FormUrlEncodedContent(authorizeArgs);
            var contentAsString = await content.ReadAsStringAsync();
            
            // prepare the URL for the authorize endpoint
            var url = $"{_appConfiguration.TapkeyAuthorizationServerUrl}/{_appConfiguration.TapkeyAuthorizationEndpointPath}?{contentAsString}";
            return Redirect(url);
        }

        public async Task<IActionResult> LoginCallback([FromQuery]string code)
        {
            if (string.IsNullOrEmpty(code))
                throw new ArgumentException("code cannot be empty.", nameof(code));

            // retrieve the PKCE from the cache
            var pkce = _memoryCache.Get<Pkce>(AppConstants.PKCECacheKey);

            var tokenRequest = new Dictionary<string, string>
            {
                {"client_id", _appConfiguration.ClientId},
                {"grant_type", "authorization_code"},
                {"redirect_uri", "https://localhost:44333/Auth/LoginCallback"},
                {"code", code},
                {"code_verifier", pkce.CodeVerifier}
            };

            var postContent = new FormUrlEncodedContent(tokenRequest);

            var response = await _tapkeyAuthServerClient.PostAsync($"{_appConfiguration.TapkeyTokenEndpointPath}", postContent);
            var tokenResponse = await ProtocolResponse.FromHttpResponseAsync<TokenResponse>(response);

            _memoryCache.Set(AppConstants.AccessTokenCacheKey, tokenResponse.AccessToken);
            _memoryCache.Set(AppConstants.RefreshTokenCacheKey, tokenResponse.RefreshToken);

            #region  Refresh Tokens - Example

            var refreshTokenRequest = new Dictionary<string, string>
            {
                {"client_id", _appConfiguration.ClientId},

                // specify the correct grant type
                {"grant_type", "refresh_token"},
                {"redirect_uri", "http://localhost:55183/Auth/LoginCallback"},

                // Need to pass the refresh token obtained from the token endpoint
                {"refresh_token", tokenResponse.RefreshToken},

                // The code verifier used to obtain the access_token
                {"code_verifier", pkce.CodeVerifier}
            };

            var refreshContent = new FormUrlEncodedContent(refreshTokenRequest);
            var refreshResponse = await _tapkeyAuthServerClient.PostAsync($"{_appConfiguration.TapkeyTokenEndpointPath}", refreshContent);
            //var refreshTokenResponse = new TokenResponse(await refreshResponse.Content.ReadAsStringAsync());
            var refreshTokenResponse = await ProtocolResponse.FromHttpResponseAsync<TokenResponse>(refreshResponse);

            #endregion

            return RedirectToAction("Index", "OwnerLocks");
        }
    }
}