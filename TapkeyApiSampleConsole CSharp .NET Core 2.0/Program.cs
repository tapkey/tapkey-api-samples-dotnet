using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OAuthHelpers;

namespace TapkeyApiSampleConsole
{
    public static class Program
    {
        private static string _clientId;
        private static string _clientSecret;

        private static UserCredential _credential;

        private static readonly Uri TapkeyApiUri = new Uri("https://my.tapkey.com/api/");
        private const string TapkeyApiVersion = "v1";

        private static async Task QueryBoundLocks()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add(
                "User-Agent",
                "Tapkey API .NET Core Console Sample Application"
            );

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _credential.Token.AccessToken);

            var ownerTask =
                client.GetStringAsync(
                    $"{new Uri(TapkeyApiUri, TapkeyApiVersion).AbsoluteUri}/owners");
            var json = await ownerTask;
            var ownerAccounts =
                JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json);

            // Check if the current user has at least one owner account
            if (ownerAccounts.Count > 0)
            {
                // Loop over user's Owner Accounts...
                foreach (var ownerAccount in ownerAccounts)
                {
                    // ... and query for bound locks
                    var boundLockTask = client.GetStringAsync(
                        $"{new Uri(TapkeyApiUri, TapkeyApiVersion).AbsoluteUri}/owners/{ownerAccount["id"]}/boundlocks");
                    json = await boundLockTask;

                    var boundLocks =
                        JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json);
                    // Make sure the selected owner account has bound locks
                    if (boundLocks.Count > 0)
                    {
                        Console.WriteLine($"Displaying Bound Locks for Owner Account {ownerAccount["name"]} ({ownerAccount["id"]}):");

                        // Print a list of all bound locks
                        foreach (var boundLock in boundLocks)
                        {
                            var lockInfo = new StringBuilder();
                            lockInfo.AppendLine("-------------------------");
                            lockInfo.AppendLine($"Lock ID: {boundLock["id"]}");
                            lockInfo.AppendLine($"Title: {boundLock["title"]}");
                            lockInfo.AppendLine($"Description: {boundLock["description"]}");
                            lockInfo.AppendLine(
                                $"Lock model name: {(boundLock["lockType"] as JObject)?.GetValue("modelName")}");
                            lockInfo.AppendLine($"Binding date: {boundLock["bindDate"]}");
                            lockInfo.AppendLine("-------------------------");
                            Console.WriteLine(lockInfo);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"No bound locks for owner account {ownerAccount["name"]} ({ownerAccount["id"]}).");
                    }
                }
            }
            else
            {
                Console.WriteLine("This user has no Owner Accounts.");
            }
        }

        public static void Main(params string[] args)
        {
            var commandLineApplication = new CommandLineApplication(throwOnUnexpectedArg: false);

            var clientId = commandLineApplication.Option(
                "-i|--id <clientId>",
                "Your client ID.",
                CommandOptionType.SingleValue);

            var clientSecret = commandLineApplication.Option(
                "-s|--secret <clientSecret>",
                "Your client secret.",
                CommandOptionType.SingleValue);

            commandLineApplication.OnExecute(async () =>
            {
                if (clientId.HasValue() && clientSecret.HasValue())
                {
                    _clientId = clientId.Value();
                    _clientSecret = clientSecret.Value();

                    try
                    {
                        _credential = await AuthorizeTapkey.Run(_clientId, _clientSecret);
                        await QueryBoundLocks();
                    }
                    catch (AggregateException ex)
                    {
                        foreach (var e in ex.InnerExceptions)
                        {
                            Console.WriteLine("ERROR: " + e.Message);
                        }
                    }
                }
                else
                {
                    commandLineApplication.ShowHelp();
                }
                return 0;
            });
            commandLineApplication.Execute(args);
        }
    }
}
