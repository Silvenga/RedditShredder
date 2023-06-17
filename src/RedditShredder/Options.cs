using CommandLine;

namespace RedditShredder
{
    public class Options
    {
        [Option("client-id", Required = true, HelpText = "Client id of an app registered with Reddit.")]
        public string ClientId { get; set; } = null!;

        [Option("client-secret", Required = true, HelpText = "Client secret of an app registered with Reddit.")]
        public string ClientSecret { get; set; } = null!;

        [Option("refresh-token", Required = true,
            HelpText =
                "A refresh token of a user after authenticating using the provided client-id (see https://not-an-aardvark.github.io/reddit-oauth-helper/).")]
        public string RefreshToken { get; set; } = null!;

        [Option('v', "verbose", HelpText = "Enable verbose logging.")]
        public bool Verbose { get; set; }

        [Option("footer", HelpText = "Any footer to add to the end of generated content.")]
        public string? Footer { get; set; }
    }
}
