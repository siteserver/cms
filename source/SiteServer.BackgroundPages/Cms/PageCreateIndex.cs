using System;
using BaiRong.Core;
using SiteServer.BackgroundPages.Service;
using SiteServer.CMS.Core.Create;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageCreateIndex : BasePageCms
    {
        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            if (!IsPostBack)
            {
                CreateManager.CreateIndex(PublishmentSystemId);
                PageCreateStatus.Redirect(PublishmentSystemId);
            }
        }
    }
}
