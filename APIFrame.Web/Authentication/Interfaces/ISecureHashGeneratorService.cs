namespace APIFrame.Web.Authentication.Interfaces
{
    public interface ISecureHashGeneratorService
    {
        string CreateHash(string value, string salt, int hashSize = 32);
        string CreateHMACSHA256(string value, string key);
        string CreateSalt();
    }
}
