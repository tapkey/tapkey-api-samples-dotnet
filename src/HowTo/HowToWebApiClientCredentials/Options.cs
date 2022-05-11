using CommandLine;

namespace HowToWebApiClientCredentials
{
    public class Options
    {
        [Option('i', "clientId", Required = true, HelpText = "Client ID of your OAuth client")]
        public string ClientId { get; set; }

        [Option('s', "clientSecret", Required = true, HelpText = "Client secret of your OAuth client")]
        public string ClientSecret { get; set; }
    }
}
