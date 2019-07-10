using SS.CMS.Models;

namespace SS.CMS.Core.Models.Attributes
{
    public static class SiteAttribute
    {
        public const string Id = nameof(Site.Id);
        public const string SiteName = nameof(Site.SiteName);
        public const string SiteDir = nameof(Site.SiteDir);
        public const string TableName = nameof(Site.TableName);
        public const string IsRoot = "IsRoot";
        public const string ParentId = nameof(Site.ParentId);
        public const string Taxis = nameof(Site.Taxis);
        public const string ExtendValues = nameof(ExtendValues);
    }
}
