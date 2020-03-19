using System.Collections.Generic;
using SSCMS;
using SSCMS.Dto.Request;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesChangeRootController
    {
        public class GetResult
        {
            public Site Site { get; set; }
            public List<string> Directories { get; set; }
            public List<string> Files { get; set; }
        }

        public class SubmitRequest : SiteRequest
        {
            public string SiteDir { get; set; }
            public IList<string> CheckedDirectories { get; set; }
            public IList<string> CheckedFiles { get; set; }
            public bool IsMoveFiles { get; set; }
        }
    }
}
