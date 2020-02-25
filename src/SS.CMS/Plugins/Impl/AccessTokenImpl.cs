using System;

namespace SS.CMS.Plugins.Impl
{
    public class AccessTokenImpl
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public DateTime ExpiresAt { get; set; }
    }
}