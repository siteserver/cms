using System;
using BaiRong.Core;
using SiteServer.BackgroundPages.Service;
using SiteServer.CMS.Core.Create;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageCreateAll : BasePageCms
    {
        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            if (!IsPostBack)
            {
                CreateManager.CreateAll(PublishmentSystemId);
                PageCreateStatus.Redirect(PublishmentSystemId);
            }
        }
    }
}
