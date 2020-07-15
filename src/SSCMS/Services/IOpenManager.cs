namespace SSCMS.Services
{
    public interface IOpenManager
    {
        (bool success, string token, string errorMessage) GetWxAccessToken(string appId, string appSecret);
    }
}
