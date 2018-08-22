using System;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Model
{
    public class RoleInfo
    {
        public int Id { get; set; }

        public string RoleName  { get; set; }

        public string CreatorUserName { get; set; }

        public string Description { get; set; }
    }
}
