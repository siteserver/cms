using System;
using SS.CMS.Abstractions;

namespace SS.CMS.Core.Components
{
    public class AccessTokenImpl: IAccessToken
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public DateTime ExpiresAt { get; set; }
    }
}