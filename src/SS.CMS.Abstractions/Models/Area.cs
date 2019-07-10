using System;
using SS.CMS.Data;

namespace SS.CMS.Models
{
    [Serializable]
    [DataTable("siteserver_Area")]
    public class Area : Entity
    {
        [DataColumn]
        public string AreaName { get; set; }

        [DataColumn]
        public int ParentId { get; set; }

        [DataColumn]
        public string ParentsPath { get; set; }

        [DataColumn]
        public int ParentsCount { get; set; }

        [DataColumn]
        public int ChildrenCount { get; set; }

        [DataColumn]
        public bool IsLastNode { get; set; }

        [DataColumn]
        public int Taxis { get; set; }
    }
}
