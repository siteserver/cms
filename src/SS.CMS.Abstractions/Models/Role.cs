using System;
using SS.CMS.Data;

namespace SS.CMS.Models
{
    [Serializable]
    [DataTable("siteserver_Role")]
    public class Role : Entity
    {
        [DataColumn]
        public string RoleName { get; set; }

        [DataColumn]
        public int UserId { get; set; }

        [DataColumn]
        public string Description { get; set; }
    }
}
