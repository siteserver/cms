using Datory;

namespace SiteServer.CMS.Model.Db
{
    [DataTable("siteserver_Site")]
    public class SiteInfo : Entity
    {
        [DataColumn]
        public string SiteDir { get; set; }

        [DataColumn]
        public string SiteName { get; set; }

        [DataColumn]
        public string TableName { get; set; }

        [DataColumn]
        public string IsRoot { get; set; }

        [DataColumn]
        public int ParentId { get; set; }

        [DataColumn]
        public int Taxis { get; set; }

        [DataColumn(Extend = true, Text = true)]
        public string SettingsXml { get; set; }
    }
}
