using Datory;
using Datory.Annotations;

namespace SSCMS.Models
{
    [DataTable("siteserver_StorageFile")]
    public class StorageFile : Entity
    {
        [DataColumn]
        public string Key { get; set; }

        [DataColumn]
        public string ETag { get; set; }

        [DataColumn]
        public string Md5 { get; set; }
    }
}
