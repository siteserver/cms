using System;

namespace SS.CMS.Core.PluginImpls
{
    public class AccessTokenImpl
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public DateTime ExpiresAt { get; set; }
    }
}