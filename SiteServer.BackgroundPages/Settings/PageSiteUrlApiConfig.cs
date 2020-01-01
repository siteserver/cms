using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
	public class PageSiteUrlApiConfig : BasePageCms
    {
        public Literal LtlSiteName;

        public RadioButtonList RblIsSeparatedApi;
        public PlaceHolder PhSeparatedApi;
        public TextBox TbSeparatedApiUrl;

        public static string GetRedirectUrl(int siteId)
        {
            return PageUtils.GetSettingsUrl(nameof(PageSiteUrlApiConfig), new NameValueCollection
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

            VerifySystemPermissions(ConfigManager.AppPermissions.SettingsSiteUrl);

            LtlSiteName.Text = SiteInfo.SiteName;

            EBooleanUtils.AddListItems(RblIsSeparatedApi, "API独立部署", "API与CMS部署在一起");
            ControlUtils.SelectSingleItem(RblIsSeparatedApi, SiteInfo.Additional.IsSeparatedApi.ToString());
            PhSeparatedApi.Visible = SiteInfo.Additional.IsSeparatedApi;
            TbSeparatedApiUrl.Text = SiteInfo.Additional.SeparatedApiUrl;
        }

        public void RblIsSeparatedApi_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhSeparatedApi.Visible = TranslateUtils.ToBool(RblIsSeparatedApi.SelectedValue);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            SiteInfo.Additional.IsSeparatedApi = TranslateUtils.ToBool(RblIsSeparatedApi.SelectedValue);
            SiteInfo.Additional.SeparatedApiUrl = TbSeparatedApiUrl.Text;
            if (!string.IsNullOrEmpty(SiteInfo.Additional.SeparatedApiUrl) && SiteInfo.Additional.SeparatedApiUrl.EndsWith("/"))
            {
                SiteInfo.Additional.SeparatedApiUrl =
                    SiteInfo.Additional.SeparatedApiUrl.Substring(0, SiteInfo.Additional.SeparatedApiUrl.Length - 1);
            }

            DataProvider.SiteDao.Update(SiteInfo);
            AuthRequest.AddSiteLog(SiteId, "修改API访问地址");

            SuccessMessage("API访问地址修改成功！");
            AddWaitAndRedirectScript(PageSiteUrlApi.GetRedirectUrl());
        }
    }
}
