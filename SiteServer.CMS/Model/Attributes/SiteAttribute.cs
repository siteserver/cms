using SiteServer.Plugin;

namespace SiteServer.CMS.Model.Attributes
{
    public static class SiteAttribute
    {
        public const string Id = nameof(ISiteInfo.Id);
        public const string SiteName = nameof(ISiteInfo.SiteName);
        public const string SiteDir = nameof(ISiteInfo.SiteDir);
        public const string TableName = nameof(ISiteInfo.TableName);
        public const string IsRoot = nameof(ISiteInfo.IsRoot);
        public const string ParentId = nameof(ISiteInfo.ParentId);
        public const string Taxis = nameof(ISiteInfo.Taxis);
        public const string SettingsXml = nameof(SettingsXml);
    }
}
