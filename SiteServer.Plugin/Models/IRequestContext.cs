using System;
using System.Web;

namespace SiteServer.Plugin.Models
{
    /// <summary>
    /// Web.API request that plugin can use
    /// </summary>
    public interface IRequestContext
    {
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

        bool IsUserLoggin { get; }

        string UserName { get; }

        IUserInfo UserInfo { get; }

        void UserLogin(string userName);

        void UserLogout();

        bool IsAdminLoggin { get; }

        string AdminName { get; }

        void AdminLogin(string administratorName);

        void AdminLogout();

        string GetUserTokenByUserName(string userName);

        string GetUserNameByToken(string token);

        string GetAdminTokenByAdminName(string administratorName);

        string GetAdminNameByToken(string token);

        void SetCookie(string name, string value, DateTime expires);

        string GetCookie(string name);

        bool IsCookieExists(string name);
    }
}
