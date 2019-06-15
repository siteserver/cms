using SS.CMS.Abstractions.Models;

namespace SS.CMS.Core.Models.Attributes
{
    public static class SiteAttribute
    {
        public const string Id = nameof(SiteInfo.Id);
        public const string SiteName = nameof(SiteInfo.SiteName);
        public const string SiteDir = nameof(SiteInfo.SiteDir);
        public const string TableName = nameof(SiteInfo.TableName);
        public const string IsRoot = "IsRoot";
        public const string ParentId = nameof(SiteInfo.ParentId);
        public const string Taxis = nameof(SiteInfo.Taxis);
        public const string SettingsXml = nameof(SettingsXml);
    }
}
