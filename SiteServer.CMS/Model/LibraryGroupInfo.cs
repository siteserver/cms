using Datory;
using Newtonsoft.Json;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.Utils;

namespace SiteServer.CMS.Model
{
    [DataTable("siteserver_LibraryGroup")]
    public class LibraryGroup : Entity
    {
        [JsonIgnore]
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