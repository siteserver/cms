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

            LtlSiteName.Text = Site.SiteName;

            EBooleanUtils.AddListItems(RblIsSeparatedWeb, "Web独立部署", "Web与CMS部署在一起");
            ControlUtils.SelectSingleItem(RblIsSeparatedWeb, Site.Additional.IsSeparatedWeb.ToString());
            PhSeparatedWeb.Visible = Site.Additional.IsSeparatedWeb;
            TbSeparatedWebUrl.Text = Site.Additional.SeparatedWebUrl;
        }

        public void RblIsSeparatedWeb_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhSeparatedWeb.Visible = TranslateUtils.ToBool(RblIsSeparatedWeb.SelectedValue);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            Site.Additional.IsSeparatedWeb = TranslateUtils.ToBool(RblIsSeparatedWeb.SelectedValue);
            Site.Additional.SeparatedWebUrl = TbSeparatedWebUrl.Text;
            if (!string.IsNullOrEmpty(Site.Additional.SeparatedWebUrl) && !Site.Additional.SeparatedWebUrl.EndsWith("/"))
            {
                Site.Additional.SeparatedWebUrl = Site.Additional.SeparatedWebUrl + "/";
            }

            DataProvider.SiteDao.UpdateAsync(Site).GetAwaiter().GetResult();
            AuthRequest.AddSiteLogAsync(SiteId, "修改Web访问地址").GetAwaiter().GetResult();

            SuccessMessage("Web访问地址修改成功！");
            AddWaitAndRedirectScript(PageSiteUrlWeb.GetRedirectUrl());
        }

        public void Return_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(PageSiteUrlWeb.GetRedirectUrl());
        }
    }
}
