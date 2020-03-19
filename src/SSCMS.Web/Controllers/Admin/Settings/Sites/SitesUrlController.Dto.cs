using System.Collections.Generic;
using SSCMS;
using SSCMS.Dto.Request;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesUrlController
    {
        public class GetResult
        {
            public List<Site> Sites { get; set; }
            public bool IsSeparatedApi { get; set; }
            public string SeparatedApiUrl { get; set; }
        }

        public class EditWebRequest : SiteRequest
        {
            public bool IsSeparatedWeb { get; set; }
            public string SeparatedWebUrl { get; set; }
            public bool IsSeparatedAssets { get; set; }
            public string AssetsDir { get; set; }
            public string SeparatedAssetsUrl { get; set; }
        }

        public class EditWebResult
        {
            public List<Site> Sites { get; set; }
        }

        public class EditApiRequest
        {
            public bool IsSeparatedApi { get; set; }
            public string SeparatedApiUrl { get; set; }
        }
    }
}
