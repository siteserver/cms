using System;
using Datory;
using Datory.Annotations;
using Newtonsoft.Json;

namespace SiteServer.Abstractions
{
    [DataTable("siteserver_LibraryGroup")]
    public class LibraryGroup : Entity
    {
        [JsonIgnore]
        [DataColumn]
        private string Type { get; set; }

        public LibraryType LibraryType
        {
            get => TranslateUtils.ToEnum(Type, LibraryType.Document);
            set => Type = value.GetValue();
        }

        [DataColumn]
        public string GroupName { get; set; }
    }
}
