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
    public class PageGovInteractPageAccept : BasePageGovInteractPage
	{
        public TextBox tbAcceptRemark;
        public TextBox tbDenyReply;

        public void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Wcm.LeftMenu.IdGovInteract, "待受理办件", AppManager.Wcm.Permission.WebSite.GovInteract);
            }
        }

        public static string GetRedirectUrl(int publishmentSystemId, int nodeId, int contentId, string listPageUrl)
        {
            return PageUtils.GetWcmUrl(nameof(PageGovInteractPageAccept), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()},
                {"ContentID", contentId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(listPageUrl)}
            });
        }

        public void Accept_OnClick(object sender, EventArgs e)
        {
            try
            {
                var remarkInfo = new GovInteractRemarkInfo(0, PublishmentSystemId, contentInfo.NodeId, contentInfo.Id, EGovInteractRemarkType.Accept, tbAcceptRemark.Text, Body.AdministratorInfo.DepartmentId, Body.AdministratorName, DateTime.Now);
                DataProvider.GovInteractRemarkDao.Insert(remarkInfo);

                GovInteractApplyManager.Log(PublishmentSystemId, contentInfo.NodeId, contentInfo.Id, EGovInteractLogType.Accept, Body.AdministratorName, Body.AdministratorInfo.DepartmentId);
                DataProvider.GovInteractContentDao.UpdateState(PublishmentSystemInfo, contentInfo.Id, EGovInteractState.Accepted);
                SuccessMessage("申请受理成功");

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

        public void Deny_OnClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbDenyReply.Text))
            {
                FailMessage("拒绝失败，必须填写拒绝理由");
                return;
            }
            try
            {
                DataProvider.GovInteractReplyDao.DeleteByContentId(PublishmentSystemId, contentInfo.Id);
                var replyInfo = new GovInteractReplyInfo(0, PublishmentSystemId, contentInfo.NodeId, contentInfo.Id, tbDenyReply.Text, string.Empty, Body.AdministratorInfo.DepartmentId, Body.AdministratorName, DateTime.Now);
                DataProvider.GovInteractReplyDao.Insert(replyInfo);

                GovInteractApplyManager.Log(PublishmentSystemId, contentInfo.NodeId, contentInfo.Id, EGovInteractLogType.Deny, Body.AdministratorName, Body.AdministratorInfo.DepartmentId);
                DataProvider.GovInteractContentDao.UpdateState(PublishmentSystemInfo, contentInfo.Id, EGovInteractState.Denied);

                SuccessMessage("拒绝申请成功");

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
