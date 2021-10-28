namespace APIFrame.Core.Enums
{
    public enum AntiforgeryValidation
    {
        Valid,
        NotMatchingTokens,
        MissingCookieToken,
        MissingHeaderToken
    }
}
