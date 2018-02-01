using System;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Settings;
using SiteServer.CMS.Core.Create;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageCreateIndex : BasePageCms
    {
        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");

            if (!IsPostBack)
            {
                CreateManager.CreateChannel(SiteId, SiteId); // 创建任务
                PageCreateStatus.Redirect(SiteId); // 转到查询任务进度页面
            }
        }
    }
}
