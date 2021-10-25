namespace APIFrame.Core.Configuration
{
    public sealed class BaseOptions
    {
        public string AppName { get; set; }

        public string AuthSecretKey { get; set; }

        public string AuthIssuerName { get; set; }

        public string[] AllowedCORSPolicyURLs { get; set; }

        public bool UseDefaultCorsPolicies { get; set; }

        public bool UseDefaultAuthentication { get; set; }

        public bool UseDefaultSwagger { get; set; }

        public bool UseBackgroundJobs { get; set; }
    }
}
