using Datory;
using Datory.Annotations;
using SSCMS.Enums;

namespace SSCMS.Models
{
    [DataTable("siteserver_WxMenu")]
    public class WxMenu : Entity
    {
        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public int ParentId { get; set; }

        [DataColumn]
        public int Taxis { get; set; }

        [DataColumn]
        public string Text { get; set; }

        [DataColumn]
        public WxMenuType MenuType { get; set; }

        [DataColumn]
        public string Key { get; set; }

        [DataColumn]
        public string Url { get; set; }

        [DataColumn]
        public string AppId { get; set; }

        [DataColumn]
        public string PagePath { get; set; }

        [DataColumn]
        public string MediaId { get; set; }
    }
}