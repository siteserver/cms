using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Wcm.GovPublic;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.BackgroundPages.Wcm
{
    public class PageGovPublicApplyToAcceptDetail : BasePageGovPublicApplyToDetail
    {
        public TextBox tbAcceptRemark;
        public TextBox tbDenyReply;

        public void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Wcm.LeftMenu.IdGovPublic, AppManager.Wcm.LeftMenu.GovPublic.IdGovPublicApply, "待受理申请", AppManager.Wcm.Permission.WebSite.GovPublicApply);
            }
        }

        public static string GetRedirectUrl(int publishmentSystemId, int applyId, string listPageUrl)
        {
            return PageUtils.GetWcmUrl(nameof(PageGovPublicApplyToAcceptDetail), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"ApplyID", applyId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(listPageUrl)}
            });
        }

        public void Accept_OnClick(object sender, EventArgs e)
        {
            try
            {
                var remarkInfo = new GovPublicApplyRemarkInfo(0, PublishmentSystemId, applyInfo.Id, EGovPublicApplyRemarkType.Accept, tbAcceptRemark.Text, Body.AdministratorInfo.DepartmentId, Body.AdministratorName, DateTime.Now);
                DataProvider.GovPublicApplyRemarkDao.Insert(remarkInfo);

                GovPublicApplyManager.Log(PublishmentSystemId, applyInfo.Id, EGovPublicApplyLogType.Accept, Body.AdministratorName, Body.AdministratorInfo.DepartmentId);
                DataProvider.GovPublicApplyDao.UpdateState(applyInfo.Id, EGovPublicApplyState.Accepted);
                SuccessMessage("申请受理成功");

                AddWaitAndRedirectScript(ListPageUrl);
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
                DataProvider.GovPublicApplyReplyDao.DeleteByApplyId(applyInfo.Id);
                var replyInfo = new GovPublicApplyReplyInfo(0, PublishmentSystemId, applyInfo.Id, tbDenyReply.Text, string.Empty, Body.AdministratorInfo.DepartmentId, Body.AdministratorName, DateTime.Now);
                DataProvider.GovPublicApplyReplyDao.Insert(replyInfo);

                GovPublicApplyManager.Log(PublishmentSystemId, applyInfo.Id, EGovPublicApplyLogType.Deny, Body.AdministratorName, Body.AdministratorInfo.DepartmentId);
                DataProvider.GovPublicApplyDao.UpdateState(applyInfo.Id, EGovPublicApplyState.Denied);

                SuccessMessage("拒绝申请成功");

                AddWaitAndRedirectScript(ListPageUrl);
            }
            catch (Exception ex)
            {
                FailMessage(ex, ex.Message);
            }
        }
	}
}
