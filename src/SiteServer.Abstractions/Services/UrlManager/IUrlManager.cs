namespace SiteServer.Abstractions
{
    public partial interface IUrlManager : IService
    {
        string ApplicationPath { get; }

        string GetRootUrl(string relatedUrl);

        string GetApiUrl(string route);

        string GetAdminUrl(string relatedUrl);
    }
}