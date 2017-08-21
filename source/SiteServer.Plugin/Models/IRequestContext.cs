using System.Web;

namespace SiteServer.Plugin.Models
{
    /// <summary>
    /// Web.API request that plugin can use
    /// </summary>
    public interface IRequestContext
    {
        bool IsAdministratorLoggin { get; }

        bool IsUserLoggin { get; }

        string UserName { get; }

        string AdministratorName { get; }

        int PublishmentSystemId { get; }

        int ChannelId { get; }

        int ContentId { get; }

        string ReturnUrl { get; }

        HttpRequest Request { get; }

        string GetQueryString(string name);

        int GetQueryInt(string name, int defaultValue = 0);

        bool GetQueryBool(string name, bool defaultValue = false);

        string GetPostString(string name);

        int GetPostInt(string name, int defaultValue = 0);

        bool GetPostBool(string name, bool defaultValue = false);
    }
}
