namespace MVCAppAuthorizationCodePKCE.NETCore.Models
{
    public class Pkce
    {
        public string CodeVerifier { get; set; }

        public string CodeChallenge { get; set; }
    }
}
