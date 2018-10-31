using System;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Util.Store;

namespace OAuthHelpers
{
    public static class AuthorizeTapkey
    {
        private static readonly Uri TapkeyAuthorizationServerUri =
            new Uri("https://login.tapkey.com");

        private const string TapkeyAuthorizationEndpointPath = "/connect/authorize";
        private const string TapkeyTokenEndpointPath = "/connect/token";
        private const int RedirectUriPortNumber = 8111;

        public static async Task<UserCredential> Run(string clientId, string clientSecret)
        {
            // Set up OAuth 2.0 authorization flow
            var initializer = new AuthorizationCodeFlow.Initializer(
                new Uri(TapkeyAuthorizationServerUri, TapkeyAuthorizationEndpointPath).AbsoluteUri,
                new Uri(TapkeyAuthorizationServerUri, TapkeyTokenEndpointPath).AbsoluteUri)
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret
                },
                Scopes = new[]
                {
                    "manage:contacts",
                    "manage:grants",
                    "read:grants",
                    "read:logs"
                },
                // Write token to disk
                // (this will resolve to AppData\TapkeyApi on Windows)
                DataStore = new FileDataStore("TapkeyApi")
            };
            var flow = new AuthorizationCodeFlow(initializer);

            // Host a page that receives the code (redirect_uri)
            // (resolves to either http://localhost:8111/authorize/ or
            // http://127.0.0.1:8111/authorize/)
            var codeReceiver = new FixedPortLocalServerCodeReceiver(RedirectUriPortNumber);

            // Create an authorization code installed app instance and authorize the user
            return await new AuthorizationCodeInstalledApp(flow, codeReceiver)
                .AuthorizeAsync("user", CancellationToken.None).ConfigureAwait(false);
        }
    }
}
