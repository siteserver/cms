using System.Collections.Generic;
using Datory;
using SSCMS.Configuration;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsSiteController
    {
        public class GetResult
        {
            public Site Site { get; set; }
            public IEnumerable<InputStyle> Styles { get; set; }
        }

        public class SubmitRequest : Entity
        {
            public int SiteId { get; set; }
            public string SiteName { get; set; }
            public string ImageUrl { get; set; }
            public string Keywords { get; set; }
            public string Description { get; set; }
        }
    }
}