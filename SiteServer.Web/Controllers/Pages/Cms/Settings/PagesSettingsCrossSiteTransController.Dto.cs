using System;
using SiteServer.CMS.Dto.Request;

namespace SiteServer.API.Controllers.Pages.Cms.Settings
{
    public partial class PagesSettingsCrossSiteTransController
    {
        public class SubmitRequest : SiteRequest
        {
            public bool IsCrossSiteTransChecked { get; set; }
        }
    }
}