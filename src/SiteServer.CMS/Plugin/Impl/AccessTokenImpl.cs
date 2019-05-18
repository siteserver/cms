using System;
using SiteServer.Plugin;

namespace SiteServer.CMS.Plugin.Impl
{
    public class AccessTokenImpl: IAccessToken
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public DateTime ExpiresAt { get; set; }
    }
}