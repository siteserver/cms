using System;
using SS.CMS.Data;

namespace SS.CMS.Models
{
    [Serializable]
    [DataTable("siteserver_Log")]
    public class Log : Entity
    {
        [DataColumn]
        public int UserId { get; set; }

        [DataColumn]
        public string IpAddress { get; set; }

        [DataColumn]
        public string Action { get; set; }

        [DataColumn]
        public string Summary { get; set; }
    }
}
