using System;
using SS.CMS.Data;

namespace SS.CMS.Models
{
    [Serializable]
    [DataTable("siteserver_Department")]
    public class Department : Entity
    {
        [DataColumn]
        public string DepartmentName { get; set; }

        [DataColumn]
        public string Code { get; set; }

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

        [DataColumn]
        public string Summary { get; set; }
    }
}
