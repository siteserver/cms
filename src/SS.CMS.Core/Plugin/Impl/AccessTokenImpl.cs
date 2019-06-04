using System;
using SS.CMS.Plugin;

namespace SS.CMS.Core.Plugin.Impl
{
    public class AccessTokenImpl: IAccessToken
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public DateTime ExpiresAt { get; set; }
    }
}