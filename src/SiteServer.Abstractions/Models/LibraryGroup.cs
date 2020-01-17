using System;
using Datory;
using Datory.Annotations;
using Newtonsoft.Json;

namespace SiteServer.Abstractions
{
    [DataTable("siteserver_LibraryGroup")]
    public class LibraryGroup : Entity
    {
        [DataColumn]
        public LibraryType Type { get; set; }

        [DataColumn]
        public string GroupName { get; set; }
    }
}
