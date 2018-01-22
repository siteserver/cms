using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.Utils.Model.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Settings
{
	public class PageSiteUrlAssetsConfig : BasePageCms
    {
        public Literal LtlPublishmentSystemName;

        public RadioButtonList RblIsSeparatedAssets;
        public PlaceHolder PhSeparatedAssets;
        public TextBox TbSeparatedAssetsUrl;
        public TextBox TbAssetsDir;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetSettingsUrl(nameof(PageSiteUrlAssetsConfig), new NameValueCollection
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

            EBooleanUtils.AddListItems(RblIsSeparatedAssets, "资源文件独立部署", "资源文件与Web部署在一起");
            ControlUtils.SelectSingleItem(RblIsSeparatedAssets, PublishmentSystemInfo.Additional.IsSeparatedAssets.ToString());
            PhSeparatedAssets.Visible = PublishmentSystemInfo.Additional.IsSeparatedAssets;
            TbSeparatedAssetsUrl.Text = PublishmentSystemInfo.Additional.SeparatedAssetsUrl;
            TbAssetsDir.Text = PublishmentSystemInfo.Additional.AssetsDir;
        }

        public void RblIsSeparatedAssets_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhSeparatedAssets.Visible = TranslateUtils.ToBool(RblIsSeparatedAssets.SelectedValue);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            PublishmentSystemInfo.Additional.IsSeparatedAssets = TranslateUtils.ToBool(RblIsSeparatedAssets.SelectedValue);
            PublishmentSystemInfo.Additional.SeparatedAssetsUrl = TbSeparatedAssetsUrl.Text;
            PublishmentSystemInfo.Additional.AssetsDir = TbAssetsDir.Text;

            DataProvider.PublishmentSystemDao.Update(PublishmentSystemInfo);
            Body.AddSiteLog(PublishmentSystemId, "修改资源文件访问地址");

            SuccessMessage("资源文件访问地址修改成功！");
            AddWaitAndRedirectScript(PageSiteUrlAssets.GetRedirectUrl());
        }

        public void Return_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(PageSiteUrlAssets.GetRedirectUrl());
        }
    }
}
