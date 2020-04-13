using Datory;
using Datory.Annotations;
using SSCMS.Enums;

namespace SSCMS.Models
{
    [DataTable("siteserver_LibraryGroup")]
    public class LibraryGroup : Entity
    {
        [DataColumn]
        public LibraryType LibraryType { get; set; }

        [DataColumn]
        public string GroupName { get; set; }
    }
}
