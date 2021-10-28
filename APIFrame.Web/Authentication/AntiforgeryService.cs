using APIFrame.Core.Enums;
using APIFrame.Utils.String;
using APIFrame.Web.Authentication.Interfaces;
using System;

namespace APIFrame.Web.Authentication
{
    public class AntiforgeryService : IAntiforgeryService
    {
        private readonly ISecureHashGeneratorService _hashGeneratorService;
        private readonly StringGenerator _stringGenerator;

        public AntiforgeryService(
            ISecureHashGeneratorService hashGeneratorService,
            StringGenerator stringGenerator)
        {
            _hashGeneratorService = hashGeneratorService;
            _stringGenerator = stringGenerator;
        }

        public string GenerateAntiforgeryToken()
        {
            var randomString = _stringGenerator.GetRandomString(32);
            var randomSalt = _stringGenerator.GetRandomString(16);

            return _hashGeneratorService.CreateHash(randomString, randomSalt);
        }

        public AntiforgeryValidation ValidateAntiforgeryTokens(string cookieToken, string headerToken)
        {
            if (string.IsNullOrEmpty(cookieToken))
            {
                return AntiforgeryValidation.MissingCookieToken;
            }
            else if (string.IsNullOrEmpty(headerToken))
            {
                return AntiforgeryValidation.MissingHeaderToken;
            }
            else if (cookieToken.Equals(headerToken, StringComparison.OrdinalIgnoreCase))
            {
                return AntiforgeryValidation.NotMatchingTokens;
            }

            return AntiforgeryValidation.Valid;
        }
    }
}
