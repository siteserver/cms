using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
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

            LtlSiteName.Text = Site.SiteName;

            EBooleanUtils.AddListItems(RblIsSeparatedAssets, "资源文件独立部署", "资源文件与Web部署在一起");
            ControlUtils.SelectSingleItem(RblIsSeparatedAssets, Site.Additional.IsSeparatedAssets.ToString());
            PhSeparatedAssets.Visible = Site.Additional.IsSeparatedAssets;
            TbSeparatedAssetsUrl.Text = Site.Additional.SeparatedAssetsUrl;
            TbAssetsDir.Text = Site.Additional.AssetsDir;
        }

        public void RblIsSeparatedAssets_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhSeparatedAssets.Visible = TranslateUtils.ToBool(RblIsSeparatedAssets.SelectedValue);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            Site.Additional.IsSeparatedAssets = TranslateUtils.ToBool(RblIsSeparatedAssets.SelectedValue);
            Site.Additional.SeparatedAssetsUrl = TbSeparatedAssetsUrl.Text;
            Site.Additional.AssetsDir = TbAssetsDir.Text;

            DataProvider.SiteDao.UpdateAsync(Site).GetAwaiter().GetResult();
            AuthRequest.AddSiteLogAsync(SiteId, "修改资源文件访问地址").GetAwaiter().GetResult();

            SuccessMessage("资源文件访问地址修改成功！");
            AddWaitAndRedirectScript(PageSiteUrlAssets.GetRedirectUrl());
        }

        public void Return_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(PageSiteUrlAssets.GetRedirectUrl());
        }
    }
}
