using APIFrame.Web.Authentication.Interfaces;

namespace APIFrame.Web.Authentication
{
    public class AuthContextInfo : IAuthContextInfo
    {
        public string AuthToken { get; set; }
        public string UserId { get; set; }
        public string ClientIp { get; set; }
    }
}