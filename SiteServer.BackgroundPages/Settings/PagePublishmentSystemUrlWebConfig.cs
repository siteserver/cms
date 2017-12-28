using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Settings
{
	public class PagePublishmentSystemUrlWebConfig : BasePageCms
    {
        public Literal LtlPublishmentSystemName;

        public DropDownList DdlIsSeparatedWeb;
        public PlaceHolder PhSeparatedWeb;
        public TextBox TbSeparatedWebUrl;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetSettingsUrl(nameof(PagePublishmentSystemUrlWebConfig), new NameValueCollection
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

            VerifyAdministratorPermissions(AppManager.Permissions.Settings.SiteManagement);

            LtlPublishmentSystemName.Text = PublishmentSystemManager.GetPublishmentSystemName(PublishmentSystemInfo);

            EBooleanUtils.AddListItems(DdlIsSeparatedWeb, "Web独立部署", "Web与CMS部署在一起");
            ControlUtils.SelectSingleItem(DdlIsSeparatedWeb, PublishmentSystemInfo.Additional.IsSeparatedWeb.ToString());
            PhSeparatedWeb.Visible = PublishmentSystemInfo.Additional.IsSeparatedWeb;
            TbSeparatedWebUrl.Text = PublishmentSystemInfo.Additional.SeparatedWebUrl;
        }

        public void DdlIsSeparatedWeb_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhSeparatedWeb.Visible = TranslateUtils.ToBool(DdlIsSeparatedWeb.SelectedValue);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            PublishmentSystemInfo.Additional.IsSeparatedWeb = TranslateUtils.ToBool(DdlIsSeparatedWeb.SelectedValue);
            PublishmentSystemInfo.Additional.SeparatedWebUrl = TbSeparatedWebUrl.Text;

            DataProvider.PublishmentSystemDao.Update(PublishmentSystemInfo);
            Body.AddSiteLog(PublishmentSystemId, "修改Web访问地址");

            SuccessMessage("Web访问地址修改成功！");
            AddWaitAndRedirectScript(PagePublishmentSystemUrlWeb.GetRedirectUrl());
        }
    }
}
