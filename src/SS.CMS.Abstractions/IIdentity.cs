using SS.CMS.Abstractions.Repositories;

namespace SS.CMS.Abstractions
{
    public interface IIdentity
    {
        bool IsApiAuthenticated { get; set; }

        string ApiToken { get; }

        // api end

        // admin start

        string AdminToken { get; }

        bool IsAdminLoggin { get; }

        IPermissions AdminPermissions { get; }

        int AdminId { get; }

        string AdminName { get; }

        IAdministratorInfo AdminInfo { get; }

        string AdminLogin(string userName, bool isAutoLogin, IAccessTokenRepository accessTokenRepository);

        void AdminLogout();

        // admin end

        // user start

        bool IsUserLoggin { get; set; }

        string UserToken { get; }

        IPermissions UserPermissions { get; }

        int UserId { get; }

        string UserName { get; }

        IUserInfo UserInfo { get; }

        string UserLogin(string userName, bool isAutoLogin);

        void UserLogout();

        // user end

        // log start

        string IpAddress { get; }

        void AddSiteLog(int siteId, string action, string summary = "");

        void AddChannelLog(int siteId, int channelId, string action, string summary = "");

        void AddContentLog(int siteId, int channelId, int contentId, string action, string summary = "");

        void AddAdminLog(string action, string summary = "");

        void AddUserLog(string action, string summary = "");
    }
}
