using System.Collections.Generic;
using System.Security.Claims;

namespace APIFrame.Web.Authentication.Interfaces
{
    public interface IJwtGeneratorService
    {
        string GenerateJwtToken(IEnumerable<Claim> claims);
    }
}
