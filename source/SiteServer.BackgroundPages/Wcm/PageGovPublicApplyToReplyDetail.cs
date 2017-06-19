using System;
using System.Collections.Specialized;
using BaiRong.Core;

namespace SiteServer.BackgroundPages.Wcm
{
	public class PageGovPublicApplyToReplyDetail : BasePageGovPublicApplyToDetail
    {
        public static string GetRedirectUrl(int siteId, int applyId, string listPageUrl)
        {
            return PageUtils.GetWcmUrl(nameof(PageGovPublicApplyToReplyDetail), new NameValueCollection
            {
                {"siteId", siteId.ToString()},
                {"ApplyID", applyId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(listPageUrl)}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Wcm.LeftMenu.IdGovPublic, AppManager.Wcm.LeftMenu.GovPublic.IdGovPublicApply, "待办理申请", AppManager.Wcm.Permission.WebSite.GovPublicApply);
            }
        }
	}
}
