using System;
using System.Collections.Generic;
using Datory;
using Datory.Annotations;
using Newtonsoft.Json;

namespace SiteServer.Abstractions
{
    [DataTable("siteserver_UserMenu")]
    public class UserMenu : Entity
    {
        [DataColumn]
        public string SystemId { get; set; }

        [DataColumn]
        [JsonIgnore]
        private string GroupIdCollection { get; set; }

        public List<int> GroupIds
        {
            get => StringUtils.GetIntList(GroupIdCollection);
            set => GroupIdCollection = StringUtils.Join(value);
        }

        [DataColumn]
        [DataIgnore]
        private string IsDisabled { get; set; }

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