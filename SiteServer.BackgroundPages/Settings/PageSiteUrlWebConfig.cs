using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
	public class PageSiteUrlWebConfig : BasePageCms
    {
        public Literal LtlSiteName;

        public RadioButtonList RblIsSeparatedWeb;
        public PlaceHolder PhSeparatedWeb;
        public TextBox TbSeparatedWebUrl;

        public static string GetRedirectUrl(int siteId)
        {
            return PageUtils.GetSettingsUrl(nameof(PageSiteUrlWebConfig), new NameValueCollection
            {
                {
                    "SiteId", siteId.ToString()
                }
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            if (IsPostBack) return;

            VerifySystemPermissions(ConfigManager.SettingsPermissions.Site);

            LtlSiteName.Text = SiteInfo.SiteName;

            EBooleanUtils.AddListItems(RblIsSeparatedWeb, "Web独立部署", "Web与CMS部署在一起");
            ControlUtils.SelectSingleItem(RblIsSeparatedWeb, SiteInfo.Additional.IsSeparatedWeb.ToString());
            PhSeparatedWeb.Visible = SiteInfo.Additional.IsSeparatedWeb;
            TbSeparatedWebUrl.Text = SiteInfo.Additional.SeparatedWebUrl;
        }

        public void RblIsSeparatedWeb_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhSeparatedWeb.Visible = TranslateUtils.ToBool(RblIsSeparatedWeb.SelectedValue);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            SiteInfo.Additional.IsSeparatedWeb = TranslateUtils.ToBool(RblIsSeparatedWeb.SelectedValue);
            SiteInfo.Additional.SeparatedWebUrl = TbSeparatedWebUrl.Text;
            if (!string.IsNullOrEmpty(SiteInfo.Additional.SeparatedWebUrl) && !SiteInfo.Additional.SeparatedWebUrl.EndsWith("/"))
            {
                SiteInfo.Additional.SeparatedWebUrl = SiteInfo.Additional.SeparatedWebUrl + "/";
            }

            DataProvider.SiteDao.Update(SiteInfo);
            AuthRequest.AddSiteLog(SiteId, "修改Web访问地址");

            SuccessMessage("Web访问地址修改成功！");
            AddWaitAndRedirectScript(PageSiteUrlWeb.GetRedirectUrl());
        }

        public void Return_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(PageSiteUrlWeb.GetRedirectUrl());
        }
    }
}
