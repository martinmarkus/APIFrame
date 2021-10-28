using APIFrame.Web.Authentication.Interfaces;
using System;
using System.Text;

namespace APIFrame.Web.Authentication
{
    public class ContextInfo : IContextInfo
    {
        public string AuthToken { get; set; }

        public string AntiforgeryToken { get; set; }

        public string UserId { get; set; }

        public string ClientIp { get; set; }

        public DateTime RequestDate { get; set; }

        public long ElapsedMs { get; set; }

        public override string ToString()
        {
            var resultSb = new StringBuilder($"UserId: {UserId}; AuthToken: {AuthToken ?? string.Empty}; " +
                $"AntiforgeryToken: {AntiforgeryToken ?? string.Empty}; " +
                $"ClientIp: {ClientIp ?? string.Empty}; RequestDate: {RequestDate};");

            if (ElapsedMs > 0)
            {
                resultSb.Append($"ElapsedMs: {ElapsedMs}");
            }

            return resultSb.ToString();
        }
    }
}