namespace SiteServer.Plugin.Apis
{
    public interface IAuthApi
    {
        bool IsGlobalAuthorized();

        bool IsAuthorized(int publishmentSystemId);
    }
}
