using System.Collections.Generic;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;

namespace SS.CMS.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesSaveController
    {
        public class GetResult
        {
            public Site Site { get; set; }
            public string TemplateDir { get; set; }
        }

        public class SaveSettingsResult
        {
            public List<string> Directories { get; set; }
            public List<string> Files { get; set; }
        }

        public class SaveFilesResult
        {
            public Channel Channel { get; set; }
        }

        public class SaveRequest : SiteRequest
        {
            public string TemplateName { get; set; }
            public string TemplateDir { get; set; }
            public string WebSiteUrl { get; set; }
            public string Description { get; set; }
            public bool IsAllFiles { get; set; }
            public IList<string> CheckedDirectories { get; set; }
            public IList<string> CheckedFiles { get; set; }
            public bool IsSaveContents { get; set; }
            public bool IsSaveAllChannels { get; set; }
            public IList<int> CheckedChannelIds { get; set; }
        }
    }
}
