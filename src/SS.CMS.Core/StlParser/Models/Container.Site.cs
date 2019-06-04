using SS.CMS.Core.Models.Attributes;

namespace SS.CMS.Core.StlParser.Models
{
    public partial class Container
    {
        public class Site
        {
            public static readonly string SqlColumns = $"{SiteAttribute.Id}, {SiteAttribute.IsRoot}, {SiteAttribute.ParentId}, {SiteAttribute.Taxis}";

            public int ItemIndex { get; set; }

            public int Id { get; set; }
        }

    }
}