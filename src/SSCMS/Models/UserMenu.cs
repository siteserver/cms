using System.Collections.Generic;
using Datory;
using Datory.Annotations;

namespace SSCMS
{
    [DataTable("siteserver_UserMenu")]
    public class UserMenu : Entity
    {
        [DataColumn]
        public string SystemId { get; set; }

        [DataColumn]
        public bool IsGroup { get; set; }

        [DataColumn]
        public List<int> GroupIds { get; set; }

        [DataColumn]
        public bool Disabled { get; set; }

        [DataColumn]
        public int ParentId { get; set; }

        [DataColumn]
        public int Taxis { get; set; }

        [DataColumn]
        public string Text { get; set; }

        [DataColumn]
        public string IconClass { get; set; }

        [DataColumn]
        public string Href { get; set; }

        [DataColumn]
        public string Target { get; set; }
    }
}