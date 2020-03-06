using SS.CMS.Abstractions.Dto.Request;

namespace SS.CMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsCrossSiteTransController
    {
        public class SubmitRequest : SiteRequest
        {
            public bool IsCrossSiteTransChecked { get; set; }
        }
    }
}