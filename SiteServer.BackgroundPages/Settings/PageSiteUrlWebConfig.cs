using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.Utils.Model.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Settings
{
	public class PageSiteUrlWebConfig : BasePageCms
    {
        public Literal LtlPublishmentSystemName;

        public RadioButtonList RblIsSeparatedWeb;
        public PlaceHolder PhSeparatedWeb;
        public TextBox TbSeparatedWebUrl;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetSettingsUrl(nameof(PageSiteUrlWebConfig), new NameValueCollection
            {
                {
                    "PublishmentSystemID", publishmentSystemId.ToString()
                }
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            if (IsPostBack) return;

            VerifyAdministratorPermissions(AppManager.Permissions.Settings.Site);

            LtlPublishmentSystemName.Text = PublishmentSystemInfo.PublishmentSystemName;

            EBooleanUtils.AddListItems(RblIsSeparatedWeb, "Web独立部署", "Web与CMS部署在一起");
            ControlUtils.SelectSingleItem(RblIsSeparatedWeb, PublishmentSystemInfo.Additional.IsSeparatedWeb.ToString());
            PhSeparatedWeb.Visible = PublishmentSystemInfo.Additional.IsSeparatedWeb;
            TbSeparatedWebUrl.Text = PublishmentSystemInfo.Additional.SeparatedWebUrl;
        }

        public void RblIsSeparatedWeb_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhSeparatedWeb.Visible = TranslateUtils.ToBool(RblIsSeparatedWeb.SelectedValue);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            PublishmentSystemInfo.Additional.IsSeparatedWeb = TranslateUtils.ToBool(RblIsSeparatedWeb.SelectedValue);
            PublishmentSystemInfo.Additional.SeparatedWebUrl = TbSeparatedWebUrl.Text;

            DataProvider.PublishmentSystemDao.Update(PublishmentSystemInfo);
            Body.AddSiteLog(PublishmentSystemId, "修改Web访问地址");

            SuccessMessage("Web访问地址修改成功！");
            AddWaitAndRedirectScript(PageSiteUrlWeb.GetRedirectUrl());
        }

        public void Return_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(PageSiteUrlWeb.GetRedirectUrl());
        }
    }
}
