namespace APIFrame.Core.Configuration
{
    public sealed class BaseOptions
    {
        public string AppName { get; set; }

        public string CustomLogContainerPath { get; set; }

        public string AuthSecretKey { get; set; }

        public string AuthIssuerName { get; set; }
    }
}
