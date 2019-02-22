namespace MVCAppAuthorizationCodePKCE.NETCore
{
    public class AppConfiguration
    {
        public string ClientId { get; set; }

        public string TapkeyApiBaseUrl { get; set; }

        public string TapkeyAuthorizationServerUrl { get; set; }

        public string TapkeyAuthorizationEndpointPath { get; set; }

        public string TapkeyTokenEndpointPath { get; set; }
    }
}
