using System.Collections.Generic;
using Datory;
using Datory.Annotations;

namespace SSCMS.Models
{
    [DataTable("siteserver_Department")]
    public class Department : Entity
    {
        [DataColumn]
        public string Name { get; set; }

        [DataColumn]
        public int ParentId { get; set; }

        [DataColumn]
        public List<int> ParentsPath { get; set; }

        [DataColumn]
        public int ParentsCount { get; set; }

        [DataColumn]
        public int ChildrenCount { get; set; }

        [DataColumn]
        public int Count { get; set; }

        [DataColumn]
        public int Taxis { get; set; }

        [DataColumn]
        public string Description { get; set; }
        
        public string FullName { get; set; }

        public IList<Department> Children { get; set; }
    }
}