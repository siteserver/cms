using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Wcm
{
    public class PageGovInteractConfigurationAlert : BasePageGovInteract
    {
        public TextBox tbGovInteractApplyDateLimit;
        public RadioButtonList rblGovInteractApplyAlertDateIsAfter;
        public TextBox tbGovInteractApplyAlertDate;
        public TextBox tbGovInteractApplyYellowAlertDate;
        public TextBox tbGovInteractApplyRedAlertDate;
        public RadioButtonList rblGovInteractApplyIsDeleteAllowed;

		public void Page_Load(object sender, EventArgs e)
		{
            PageUtils.CheckRequestParameter("PublishmentSystemID");

			if (!IsPostBack)
            {
                BreadCrumb(AppManager.Wcm.LeftMenu.IdGovInteract, AppManager.Wcm.LeftMenu.GovInteract.IdGovInteractConfiguration, "办件预警设置", AppManager.Wcm.Permission.WebSite.GovInteractConfiguration);

                tbGovInteractApplyDateLimit.Text = PublishmentSystemInfo.Additional.GovInteractApplyDateLimit.ToString();
                rblGovInteractApplyAlertDateIsAfter.SelectedValue = (PublishmentSystemInfo.Additional.GovInteractApplyAlertDate > 0).ToString();
                var alertDate = PublishmentSystemInfo.Additional.GovInteractApplyAlertDate;
                if (alertDate < 0) alertDate = -alertDate;
                tbGovInteractApplyAlertDate.Text = alertDate.ToString();
                tbGovInteractApplyYellowAlertDate.Text = PublishmentSystemInfo.Additional.GovInteractApplyYellowAlertDate.ToString();
                tbGovInteractApplyRedAlertDate.Text = PublishmentSystemInfo.Additional.GovInteractApplyRedAlertDate.ToString();
                rblGovInteractApplyIsDeleteAllowed.SelectedValue = PublishmentSystemInfo.Additional.GovInteractApplyIsDeleteAllowed.ToString();
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
                PublishmentSystemInfo.Additional.GovInteractApplyDateLimit = TranslateUtils.ToInt(tbGovInteractApplyDateLimit.Text);
                var alertDate = TranslateUtils.ToInt(tbGovInteractApplyAlertDate.Text);
                if (!TranslateUtils.ToBool(rblGovInteractApplyAlertDateIsAfter.SelectedValue))
                {
                    alertDate = -alertDate;
                }
                PublishmentSystemInfo.Additional.GovInteractApplyAlertDate = alertDate;
                PublishmentSystemInfo.Additional.GovInteractApplyYellowAlertDate = TranslateUtils.ToInt(tbGovInteractApplyYellowAlertDate.Text);
                PublishmentSystemInfo.Additional.GovInteractApplyRedAlertDate = TranslateUtils.ToInt(tbGovInteractApplyRedAlertDate.Text);
                PublishmentSystemInfo.Additional.GovInteractApplyIsDeleteAllowed = TranslateUtils.ToBool(rblGovInteractApplyIsDeleteAllowed.SelectedValue);
				
				try
				{
                    DataProvider.PublishmentSystemDao.Update(PublishmentSystemInfo);

                    Body.AddSiteLog(PublishmentSystemId, "修改互动交流设置");

                    SuccessMessage("互动交流设置修改成功！");
				}
				catch(Exception ex)
				{
                    FailMessage(ex, "互动交流设置修改失败！");
				}
			}
		}
	}
}
