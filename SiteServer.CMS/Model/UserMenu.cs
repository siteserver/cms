using System.Collections.Generic;
using Datory;
using SiteServer.Utils;

namespace SiteServer.CMS.Model
{
    [DataTable("siteserver_UserMenu")]
    public class UserMenu : Entity
    {
        [DataColumn]
        public string SystemId { get; set; }

        [DataColumn]
        public string GroupIdCollection { get; set; }

        public List<int> GroupIds
        {
            get => TranslateUtils.StringCollectionToIntList(GroupIdCollection);
            set => GroupIdCollection = string.Join(",", value);
        }

        [DataColumn]
        public string IsDisabled { get; set; }

        public bool Disabled
        {
            get => TranslateUtils.ToBool(IsDisabled);
            set => IsDisabled = value.ToString();
        }

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