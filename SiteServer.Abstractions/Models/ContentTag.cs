using System;
using System.Collections.Generic;
using Datory;

namespace SiteServer.Abstractions
{
    [Serializable]
    [DataTable("siteserver_Tag")]
    public class ContentTag : Entity
    {
        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public string ContentIdCollection { get; set; }

        public List<int> ContentIds
        {
            get => StringUtils.GetIntList(ContentIdCollection);
            set => ContentIdCollection = StringUtils.Join(value);
        }

        [DataColumn(Text = true)]
        public string Tag { get; set; }

        [DataColumn]
        public int UseNum { get; set; }

        [DataColumn]
        public int Level { get; set; }
    }
}
