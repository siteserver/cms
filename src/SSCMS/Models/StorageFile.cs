using Datory;
using Datory.Annotations;
using SSCMS.Enums;

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

        [DataColumn]
        public FileType FileType { get; set; }
    }
}
