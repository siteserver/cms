using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Wcm.GovInteract;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.BackgroundPages.Wcm
{
	public class PageGovInteractPageCheck : BasePageGovInteractPage
	{
        public TextBox tbRedoRemark;

        public void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Wcm.LeftMenu.IdGovInteract, "待审核办件", AppManager.Wcm.Permission.WebSite.GovInteract);
            }
        }

        public static string GetRedirectUrl(int publishmentSystemId, int nodeId, int contentId, string listPageUrl)
        {
            return PageUtils.GetWcmUrl(nameof(PageGovInteractPageCheck), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()},
                {"ContentID", contentId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(listPageUrl)}
            });
        }

        public void Redo_OnClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbRedoRemark.Text))
            {
                FailMessage("要求返工失败，必须填写意见");
                return;
            }
            try
            {
                var remarkInfo = new GovInteractRemarkInfo(0, PublishmentSystemId, contentInfo.NodeId, contentInfo.Id, EGovInteractRemarkType.Redo, tbRedoRemark.Text, Body.AdministratorInfo.DepartmentId, Body.AdministratorName, DateTime.Now);
                DataProvider.GovInteractRemarkDao.Insert(remarkInfo);

                GovInteractApplyManager.Log(PublishmentSystemId, contentInfo.NodeId, contentInfo.Id, EGovInteractLogType.Redo, Body.AdministratorName, Body.AdministratorInfo.DepartmentId);
                DataProvider.GovInteractContentDao.UpdateState(PublishmentSystemInfo, contentInfo.Id, EGovInteractState.Redo);

                SuccessMessage("要求返工成功");

                if (!PublishmentSystemInfo.Additional.GovInteractApplyIsOpenWindow)
                {
                    AddWaitAndRedirectScript(ListPageUrl);
                }
            }
            catch (Exception ex)
            {
                FailMessage(ex, ex.Message);
            }
        }

        public void Check_OnClick(object sender, EventArgs e)
        {
            try
            {
                GovInteractApplyManager.Log(PublishmentSystemId, contentInfo.NodeId, contentInfo.Id, EGovInteractLogType.Check, Body.AdministratorName, Body.AdministratorInfo.DepartmentId);
                DataProvider.GovInteractContentDao.UpdateState(PublishmentSystemInfo, contentInfo.Id, EGovInteractState.Checked);
                SuccessMessage("审核申请成功");

                if (!PublishmentSystemInfo.Additional.GovInteractApplyIsOpenWindow)
                {
                    AddWaitAndRedirectScript(ListPageUrl);
                }
            }
            catch (Exception ex)
            {
                FailMessage(ex, ex.Message);
            }
        }
	}
}
