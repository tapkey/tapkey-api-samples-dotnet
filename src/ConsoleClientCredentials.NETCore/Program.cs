using IdentityModel.Client;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Tapkey.Api.Models;

namespace ConsoleClientCredentials.NETCore
{
    class Program
    {
        // Tapkey Api configuration
        private static HttpClient _tapkeyApiClient = new HttpClient();
        private const string TapkeyApiUrl = "https://my.tapkey.com/";
        private const string TapkeyApiVersion = "api/v1";

        // Tapkey Authorization Server configuration
        private static HttpClient _tapkeyAuthServer = new HttpClient();
        private const string TapkeyAuthorizationServer = "https://login.tapkey.com";

        public static int Main(string[] args)
        {
            var app = new CommandLineApplication();
            app.HelpOption("-h|--help");

            var clientId = app.Option("-i|--id <clientId>", "Your client ID", CommandOptionType.SingleValue);
            var clientSecret = app.Option("-s|--secret <clientSecret>", "Your client secret.", CommandOptionType.SingleValue);

            //pre-configure the TapkeyHttpClient
            _tapkeyApiClient.BaseAddress = new Uri(TapkeyApiUrl);
            _tapkeyApiClient.DefaultRequestHeaders.Add(
                "User-Agent",
                ".NET Core Console Native App Authorization Code with PKCE flow");

            app.OnExecute(async () =>
            {
                if (!clientId.HasValue() || !clientSecret.HasValue())
                {
                    app.ShowHelp();
                    return 1;
                }
                try
                {
                    var access_token = await RequestClientCredentialsToken(clientId.Value(), clientSecret.Value());

                    // Get the bound locks which are managed by this service account user
                    await QueryBoundLocksAsync(access_token);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR: {ex.Message}");
                }

                return 0;
            });
            return app.Execute(args);
        }

        /// <summary>
        /// Initiates a client credentials flow and returns the access_token
        /// </summary>
        /// <returns></returns>
        private static async Task<string> RequestClientCredentialsToken(string clientId, string clientSecret)
        {
            // uses the RequestClientCredentialsTokenAsync extension method on IdentityModel2 NuGet package
            var response = await _tapkeyAuthServer.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = $"{TapkeyAuthorizationServer}/connect/token",
                ClientId = clientId,
                ClientSecret = clientSecret,
                Scope = "read:core:entities read:owneraccounts"
            });

            if (response.IsError)
                throw new Exception(response.Error);

            return response.AccessToken;
        }

        private static async Task QueryBoundLocksAsync(string access_token)
        {
            _tapkeyApiClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", access_token);

            // Get the owner accounts that this service account has access (as co-admin)
            var ownersJsonResponse = await _tapkeyApiClient.GetStringAsync($"{TapkeyApiVersion}/owners");
            var ownerAccounts = JsonConvert.DeserializeObject<List<OwnerAccount>>(ownersJsonResponse);

            if (!ownerAccounts.Any())
            {
                Console.WriteLine("This user has no Owner Accounts.");
                return;
            }

            // Loop over user's Owner Accounts...
            foreach (var ownerAccount in ownerAccounts)
            {
                // ... and query for bound locks
                var boundLocksJson = await _tapkeyApiClient.GetStringAsync($"{TapkeyApiVersion}/owners/{ownerAccount.Id}/boundlocks");
                var boundLocks = JsonConvert.DeserializeObject<List<BoundLock>>(boundLocksJson);

                // Make sure the selected owner account has bound locks
                if (!boundLocks.Any())
                {
                    Console.WriteLine($"No bound locks for owner account {ownerAccount.Name} ({ownerAccount.Id}).");
                    continue;
                }

                // Print a list of all bound locks
                Console.WriteLine($"Displaying Bound Locks for Owner Account {ownerAccount.Name} ({ownerAccount.Id}):");
                foreach (var boundLock in boundLocks)
                {
                    var lockInfo = new StringBuilder();
                    lockInfo.AppendLine("-------------------------");
                    lockInfo.AppendLine($"Lock ID: {boundLock.Id}");
                    lockInfo.AppendLine($"Title: {boundLock.Title}");
                    lockInfo.AppendLine($"Description: {boundLock.Description}");

                    if (boundLock.LockType != null)
                    {
                        lockInfo.AppendLine($"Lock model name: {boundLock.LockType.ModelName}");
                    }

                    lockInfo.AppendLine($"Binding date: {boundLock.BindDate}");
                    lockInfo.AppendLine("-------------------------");
                    Console.WriteLine(lockInfo);
                }
            }
        }
    }
}
