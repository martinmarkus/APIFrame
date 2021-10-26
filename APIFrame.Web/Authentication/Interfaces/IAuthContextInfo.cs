namespace APIFrame.Web.Authentication.Interfaces
{
    public interface IAuthContextInfo
    {
        public string AuthToken { get; set; }

        public string AntiforgeryToken { get; set; }

        public string UserId { get; set; }

        public string ClientIp { get; set; }
    }
}
