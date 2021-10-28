using APIFrame.Core.Enums;

namespace APIFrame.Web.Authentication.Interfaces
{
    public interface IAntiforgeryService
    {
        string GenerateAntiforgeryToken();
        AntiforgeryValidation ValidateAntiforgeryTokens(string cookieToken, string headerToken);
    }
}
