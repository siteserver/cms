using System;
using System.Collections.Specialized;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Wcm
{
	public class PageGovPublicApplyToReply : BasePageGovPublicApplyTo
    {
        public void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Wcm.LeftMenu.IdGovPublic, AppManager.Wcm.LeftMenu.GovPublic.IdGovPublicApply, "待办理申请", AppManager.Wcm.Permission.WebSite.GovPublicApply);
            }
        }

        protected override string GetSelectString()
        {
            return DataProvider.GovPublicApplyDao.GetSelectStringByState(PublishmentSystemId, EGovPublicApplyState.Accepted, EGovPublicApplyState.Redo);
        }

        private string _pageUrl;
        protected override string PageUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_pageUrl))
                {
                    _pageUrl = PageUtils.GetWcmUrl(nameof(PageGovPublicApplyToReply), new NameValueCollection
                    {
                        {"PublishmentSystemID", PublishmentSystemId.ToString()}
                    });
                }
                return _pageUrl;
            }
        }
	}
}
