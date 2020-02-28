using System.Collections.Generic;
using SS.CMS.Core;

namespace SS.CMS.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesTemplatesController
    {
        public class ListResult
        {
            public List<SiteTemplateInfo> SiteTemplateInfoList { get; set; }
            public List<string> FileNameList { get; set; }
            public string SiteTemplateUrl { get; set; }
            public bool SiteAddPermission { get; set; }
        }

        public class ZipRequest
        {
            public string DirectoryName { get; set; }
        }

        public class UnZipRequest
        {
            public string FileName { get; set; }
        }

        public class DeleteRequest
        {
            public string DirectoryName { get; set; }
            public string FileName { get; set; }
        }
    }
}
