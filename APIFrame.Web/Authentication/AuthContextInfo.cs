using APIFrame.Web.Authentication.Interfaces;
using System;

namespace APIFrame.Web.Authentication
{
    public class AuthContextInfo : IAuthContextInfo
    {
        public string AuthToken { get; set; }

        public string AntiforgeryToken { get; set; }

        public string UserId { get; set; }

        public string ClientIp { get; set; }

        public DateTime RequestDate { get; set; }
    }
}