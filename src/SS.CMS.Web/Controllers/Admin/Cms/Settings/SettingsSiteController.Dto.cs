using System.Collections.Generic;
using Datory;
using SS.CMS.Abstractions;

namespace SS.CMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsSiteController
    {
        public class Style
        {
            public int Id { get; set; }
            public string AttributeName { get; set; }
            public string DisplayName { get; set; }
            public string InputType { get; set; }
            public IEnumerable<TableStyleRule> Rules { get; set; }
            public IEnumerable<TableStyleItem> Items { get; set; }
        }

        public class GetResult
        {
            public Site Site { get; set; }
            public IEnumerable<Style> Styles { get; set; }
        }

        public class SubmitRequest : Entity
        {
            public int SiteId { get; set; }
            public string SiteName { get; set; }
            public int PageSize { get; set; }
            public bool IsCreateDoubleClick { get; set; }
        }
    }
}