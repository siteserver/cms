using System;
using System.Web;

namespace SiteServer.Plugin.Models
{
    /// <summary>
    /// Web.API request that plugin can use
    /// </summary>
    public interface IRequestContext
    {
        bool IsAdministratorLoggin { get; }

        string AdministratorName { get; }

        bool IsUserLoggin { get; }

        string UserName { get; }

        string ReturnUrl { get; }

        HttpRequest Request { get; }

        string GetQueryString(string name);

        int GetQueryInt(string name, int defaultValue = 0);

        decimal GetQueryDecimal(string name, decimal defaultValue = 0);

        bool GetQueryBool(string name, bool defaultValue = false);

        string GetPostString(string name);

        int GetPostInt(string name, int defaultValue = 0);

        decimal GetPostDecimal(string name, decimal defaultValue = 0);

        bool GetPostBool(string name, bool defaultValue = false);

        T GetPostObject<T>(string name);

        void SetCookie(string name, string value, DateTime expires);

        string GetCookie(string name);

        bool IsCookieExists(string name);
    }
}
