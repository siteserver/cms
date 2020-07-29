using System.Collections.Generic;
using Datory;
using Datory.Annotations;
using SSCMS.Enums;

namespace SSCMS.Models
{
    [DataTable("siteserver_MaterialMessage")]
    public class MaterialMessage : Entity
    {
        [DataColumn]
        public string MediaId { get; set; }

        [DataColumn]
        public int GroupId { get; set; }

        [DataIgnore]
        public List<MaterialMessageItem> Items { get; set; }
    }
}