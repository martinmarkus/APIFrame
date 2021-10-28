using System;

namespace APIFrame.Web.Authentication.Interfaces
{
    public interface IContextInfo
    {
        public string AuthToken { get; set; }

        public string AntiforgeryToken { get; set; }

        public string UserId { get; set; }

        public string ClientIp { get; set; }

        DateTime RequestDate { get; set; }
        long ElapsedMs { get; set; }

        string ToString();
    }
}
