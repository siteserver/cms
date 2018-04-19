using System;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Settings;
using SiteServer.CMS.Core.Create;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageCreateAll : BasePageCms
    {
        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");

            if (IsPostBack) return;

            CreateManager.CreateByAll(SiteId);
            PageCreateStatus.Redirect(SiteId);
        }
    }
}
