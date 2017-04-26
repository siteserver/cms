using System;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.BackgroundPages.Sys
{
    public class PageAppAdd : BasePageCms
    {
        protected override bool IsSinglePage => true;

        public static string GetRedirectUrl()
        {
            return PageUtils.GetSysUrl(nameof(PageAppAdd), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (!IsPostBack)
            {
                if (AppManager.IsWcm())
                {
                    PageUtils.RedirectToLoadingPage(PagePublishmentSystemAdd.GetRedirectUrl(EPublishmentSystemType.WCM));
                }
                else
                {
                    PageUtils.RedirectToLoadingPage(PagePublishmentSystemAdd.GetRedirectUrl(EPublishmentSystemType.CMS));
                }
            }
        }
    }
}
