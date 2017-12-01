using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Settings
{
	public class PagePublishmentSystemUrlConfig : BasePageCms
    {
        public Literal LtlPublishmentSystemName;

        public DropDownList DdlIsSeparatedWeb;
        public PlaceHolder PhSeparatedWeb;
        public TextBox TbSeparatedWebUrl;

        public DropDownList DdlIsSeparatedAssets;
        public PlaceHolder PhSeparatedAssets;
        public TextBox TbSeparatedAssetsUrl;

        public TextBox TbAssetsDir;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetSettingsUrl(nameof(PagePublishmentSystemUrlConfig), new NameValueCollection
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

            BreadCrumbSettings("访问地址管理", AppManager.Permissions.Settings.SiteManagement);

            LtlPublishmentSystemName.Text = PublishmentSystemManager.GetPublishmentSystemName(PublishmentSystemInfo);

            EBooleanUtils.AddListItems(DdlIsSeparatedWeb, "Web独立部署", "Web与CMS部署在一起");
            ControlUtils.SelectListItems(DdlIsSeparatedWeb, PublishmentSystemInfo.Additional.IsSeparatedWeb.ToString());
            PhSeparatedWeb.Visible = PublishmentSystemInfo.Additional.IsSeparatedWeb;
            TbSeparatedWebUrl.Text = PublishmentSystemInfo.Additional.SeparatedWebUrl;

            EBooleanUtils.AddListItems(DdlIsSeparatedAssets, "资源文件独立部署", "资源文件与Web部署在一起");
            ControlUtils.SelectListItems(DdlIsSeparatedAssets, PublishmentSystemInfo.Additional.IsSeparatedAssets.ToString());
            PhSeparatedAssets.Visible = PublishmentSystemInfo.Additional.IsSeparatedAssets;
            TbSeparatedAssetsUrl.Text = PublishmentSystemInfo.Additional.SeparatedAssetsUrl;

            TbAssetsDir.Text = PublishmentSystemInfo.Additional.AssetsDir;
        }

        public void DdlIsSeparatedWeb_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhSeparatedWeb.Visible = TranslateUtils.ToBool(DdlIsSeparatedWeb.SelectedValue);
        }

        public void DdlIsSeparatedAssets_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhSeparatedAssets.Visible = TranslateUtils.ToBool(DdlIsSeparatedAssets.SelectedValue);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            try
            {
                PublishmentSystemInfo.Additional.IsSeparatedWeb = TranslateUtils.ToBool(DdlIsSeparatedWeb.SelectedValue);
                PublishmentSystemInfo.Additional.SeparatedWebUrl = TbSeparatedWebUrl.Text;
                PublishmentSystemInfo.Additional.IsSeparatedAssets = TranslateUtils.ToBool(DdlIsSeparatedAssets.SelectedValue);
                PublishmentSystemInfo.Additional.SeparatedAssetsUrl = TbSeparatedAssetsUrl.Text;

                PublishmentSystemInfo.Additional.AssetsDir = TbAssetsDir.Text;

                DataProvider.PublishmentSystemDao.Update(PublishmentSystemInfo);
                Body.AddSiteLog(PublishmentSystemId, "修改网站访问设置");
            }
            catch (Exception ex)
            {
                PageUtils.RedirectToErrorPage($"修改失败：{ex.Message}");
            }
        }
    }
}
