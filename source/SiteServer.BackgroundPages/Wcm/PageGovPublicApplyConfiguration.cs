using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Wcm
{
    public class PageGovPublicApplyConfiguration : BasePageGovPublic
    {
        public TextBox tbGovPublicApplyDateLimit;
        public RadioButtonList rblGovPublicApplyAlertDateIsAfter;
        public TextBox tbGovPublicApplyAlertDate;
        public TextBox tbGovPublicApplyYellowAlertDate;
        public TextBox tbGovPublicApplyRedAlertDate;
        public RadioButtonList rblGovPublicApplyIsDeleteAllowed;

		public void Page_Load(object sender, EventArgs e)
		{
            PageUtils.CheckRequestParameter("PublishmentSystemID");

			if (!IsPostBack)
            {
                BreadCrumb(AppManager.Wcm.LeftMenu.IdGovPublic, AppManager.Wcm.LeftMenu.GovPublic.IdGovPublicApplyConfiguration, "依申请公开设置", AppManager.Wcm.Permission.WebSite.GovPublicApplyConfiguration);

                tbGovPublicApplyDateLimit.Text = PublishmentSystemInfo.Additional.GovPublicApplyDateLimit.ToString();
                rblGovPublicApplyAlertDateIsAfter.SelectedValue = (PublishmentSystemInfo.Additional.GovPublicApplyAlertDate > 0).ToString();
                var alertDate = PublishmentSystemInfo.Additional.GovPublicApplyAlertDate;
                if (alertDate < 0) alertDate = -alertDate;
                tbGovPublicApplyAlertDate.Text = alertDate.ToString();
                tbGovPublicApplyYellowAlertDate.Text = PublishmentSystemInfo.Additional.GovPublicApplyYellowAlertDate.ToString();
                tbGovPublicApplyRedAlertDate.Text = PublishmentSystemInfo.Additional.GovPublicApplyRedAlertDate.ToString();
                rblGovPublicApplyIsDeleteAllowed.SelectedValue = PublishmentSystemInfo.Additional.GovPublicApplyIsDeleteAllowed.ToString();
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
                PublishmentSystemInfo.Additional.GovPublicApplyDateLimit = TranslateUtils.ToInt(tbGovPublicApplyDateLimit.Text);
                var alertDate = TranslateUtils.ToInt(tbGovPublicApplyAlertDate.Text);
                if (!TranslateUtils.ToBool(rblGovPublicApplyAlertDateIsAfter.SelectedValue))
                {
                    alertDate = -alertDate;
                }
                PublishmentSystemInfo.Additional.GovPublicApplyAlertDate = alertDate;
                PublishmentSystemInfo.Additional.GovPublicApplyYellowAlertDate = TranslateUtils.ToInt(tbGovPublicApplyYellowAlertDate.Text);
                PublishmentSystemInfo.Additional.GovPublicApplyRedAlertDate = TranslateUtils.ToInt(tbGovPublicApplyRedAlertDate.Text);
                PublishmentSystemInfo.Additional.GovPublicApplyIsDeleteAllowed = TranslateUtils.ToBool(rblGovPublicApplyIsDeleteAllowed.SelectedValue);
				
				try
				{
                    DataProvider.PublishmentSystemDao.Update(PublishmentSystemInfo);

                    Body.AddSiteLog(PublishmentSystemId, "修改依申请公开设置");

                    SuccessMessage("依申请公开设置修改成功！");
				}
				catch(Exception ex)
				{
                    FailMessage(ex, "依申请公开设置修改失败！");
				}
			}
		}
	}
}
