namespace SiteServer.Plugin.Apis
{
    public interface IAuthApi
    {
        bool IsAuthorized();

        bool IsAuthorized(int publishmentSystemId);

        string EncryptStringBySecretKey(string inputString);

        string DecryptStringBySecretKey(string inputString);
    }
}
