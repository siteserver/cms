using SS.CMS.Core.Models.Attributes;

namespace SS.CMS.Core.StlParser.Models
{
    public partial class Container
    {
        public static class Site
        {
            public static readonly string SqlColumns = $"{SiteAttribute.Id}, {SiteAttribute.IsRoot}, {SiteAttribute.ParentId}, {SiteAttribute.Taxis}";
        }

    }
}