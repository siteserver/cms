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
	public class PageGovPublicApplyToCheckDetail : BasePageGovPublicApplyToDetail
	{
        public TextBox tbRedoRemark;

        public void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Wcm.LeftMenu.IdGovPublic, AppManager.Wcm.LeftMenu.GovPublic.IdGovPublicApply, "待审核申请", AppManager.Wcm.Permission.WebSite.GovPublicApply);
            }
        }

        public static string GetRedirectUrl(int publishmentSystemId, int applyId, string listPageUrl)
        {
            return PageUtils.GetWcmUrl(nameof(PageGovPublicApplyToCheckDetail), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"ApplyID", applyId.ToString()},
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
                var remarkInfo = new GovPublicApplyRemarkInfo(0, PublishmentSystemId, applyInfo.Id, EGovPublicApplyRemarkType.Redo, tbRedoRemark.Text, Body.AdministratorInfo.DepartmentId, Body.AdministratorName, DateTime.Now);
                DataProvider.GovPublicApplyRemarkDao.Insert(remarkInfo);

                GovPublicApplyManager.Log(PublishmentSystemId, applyInfo.Id, EGovPublicApplyLogType.Redo, Body.AdministratorName, Body.AdministratorInfo.DepartmentId);
                DataProvider.GovPublicApplyDao.UpdateState(applyInfo.Id, EGovPublicApplyState.Redo);

                SuccessMessage("要求返工成功");

                AddWaitAndRedirectScript(ListPageUrl);
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
                GovPublicApplyManager.Log(PublishmentSystemId, applyInfo.Id, EGovPublicApplyLogType.Check, Body.AdministratorName, Body.AdministratorInfo.DepartmentId);
                DataProvider.GovPublicApplyDao.UpdateState(applyInfo.Id, EGovPublicApplyState.Checked);
                SuccessMessage("审核申请成功");
                AddWaitAndRedirectScript(ListPageUrl);
            }
            catch (Exception ex)
            {
                FailMessage(ex, ex.Message);
            }
        }
	}
}
