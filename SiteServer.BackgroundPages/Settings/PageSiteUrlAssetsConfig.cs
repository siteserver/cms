using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Database.Caches;
using SiteServer.CMS.Database.Core;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
	public class PageSiteUrlAssetsConfig : BasePageCms
    {
        public Literal LtlSiteName;

        public RadioButtonList RblIsSeparatedAssets;
        public PlaceHolder PhSeparatedAssets;
        public TextBox TbSeparatedAssetsUrl;
        public TextBox TbAssetsDir;

        public static string GetRedirectUrl(int siteId)
        {
            return PageUtils.GetSettingsUrl(nameof(PageSiteUrlAssetsConfig), new NameValueCollection
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

            EBooleanUtils.AddListItems(RblIsSeparatedAssets, "资源文件独立部署", "资源文件与Web部署在一起");
            ControlUtils.SelectSingleItem(RblIsSeparatedAssets, SiteInfo.Extend.IsSeparatedAssets.ToString());
            PhSeparatedAssets.Visible = SiteInfo.Extend.IsSeparatedAssets;
            TbSeparatedAssetsUrl.Text = SiteInfo.Extend.SeparatedAssetsUrl;
            TbAssetsDir.Text = SiteInfo.Extend.AssetsDir;
        }

        public void RblIsSeparatedAssets_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhSeparatedAssets.Visible = TranslateUtils.ToBool(RblIsSeparatedAssets.SelectedValue);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            SiteInfo.Extend.IsSeparatedAssets = TranslateUtils.ToBool(RblIsSeparatedAssets.SelectedValue);
            SiteInfo.Extend.SeparatedAssetsUrl = TbSeparatedAssetsUrl.Text;
            SiteInfo.Extend.AssetsDir = TbAssetsDir.Text;

            DataProvider.Site.Update(SiteInfo);
            AuthRequest.AddSiteLog(SiteId, "修改资源文件访问地址");

            SuccessMessage("资源文件访问地址修改成功！");
            AddWaitAndRedirectScript(PageSiteUrlAssets.GetRedirectUrl());
        }

        public void Return_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(PageSiteUrlAssets.GetRedirectUrl());
        }
    }
}
