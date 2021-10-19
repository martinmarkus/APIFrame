namespace APIFrame.Web.Services.Interfaces
{
    public interface IPasswordService
    {
        string CreateHash(string value, string salt, int hashSize = 32);
        string CreateHMACSHA256(string value, string key);
        string CreateSalt();
    }
}
