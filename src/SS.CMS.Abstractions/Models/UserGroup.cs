using System;
using SS.CMS.Data;

namespace SS.CMS.Models
{
    [Serializable]
    [DataTable("siteserver_UserGroup")]
    public class UserGroup : Entity
    {
        [DataColumn]
        public string GroupName { get; set; }

        [DataColumn]
        public string AdminName { get; set; }
    }
}
