using System.Collections.Generic;
using Datory;
using SiteServer.Utils;

namespace SiteServer.CMS.Model
{
    [DataTable("siteserver_Tag")]
    public class ContentTag : Entity
    {
        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public string ContentIdCollection { get; set; }

        public List<int> ContentIds
        {
            get => TranslateUtils.StringCollectionToIntList(ContentIdCollection);
            set => ContentIdCollection = string.Join(",", value);
        }

        [DataColumn(Text = true)]
        public string Tag { get; set; }

        [DataColumn]
        public int UseNum { get; set; }

        [DataColumn]
        public int Level { get; set; }
    }
}
