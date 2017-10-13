using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Settings
{
    public class ModalChangePublishmentSystemUrl : BasePageCms
    {
        public PlaceHolder PhUrlSettings;

        public DropDownList DdlIsSeparatedWeb;
        public PlaceHolder PhSeparatedWeb;
        public TextBox TbSeparatedWebUrl;

        public static string GetOpenWindowString(int publishmentSystemId)
        {
            return PageUtils.GetOpenWindowString("修改访问地址",
                PageUtils.GetSettingsUrl(nameof(ModalChangePublishmentSystemUrl), new NameValueCollection
                {
                    {
                        "PublishmentSystemID", publishmentSystemId.ToString()
                    }
                }), 600, 550);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            if (Page.IsPostBack) return;

            PhUrlSettings.Visible = !ConfigManager.SystemConfigInfo.IsUrlGlobalSetting;

            EBooleanUtils.AddListItems(DdlIsSeparatedWeb, "Web独立部署", "Web与CMS部署在一起");
            ControlUtils.SelectListItems(DdlIsSeparatedWeb, PublishmentSystemInfo.Additional.IsSeparatedWeb.ToString());
            PhSeparatedWeb.Visible = PublishmentSystemInfo.Additional.IsSeparatedWeb;
            TbSeparatedWebUrl.Text = PublishmentSystemInfo.Additional.SeparatedWebUrl;
        }

        public void DdlIsSeparatedWeb_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhSeparatedWeb.Visible = TranslateUtils.ToBool(DdlIsSeparatedWeb.SelectedValue);
        }

        public string GetSiteName()
        {
            return PublishmentSystemInfo.PublishmentSystemName;
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            try
            {
                PublishmentSystemInfo.Additional.IsSeparatedWeb = TranslateUtils.ToBool(DdlIsSeparatedWeb.SelectedValue);
                PublishmentSystemInfo.Additional.SeparatedWebUrl = TbSeparatedWebUrl.Text;

                //PublishmentSystemInfo.Additional.HomeUrl = TbHomeUrl.Text;

                DataProvider.PublishmentSystemDao.Update(PublishmentSystemInfo);
                Body.AddSiteLog(PublishmentSystemId, "修改网站访问设置");
            }
            catch (Exception ex)
            {
                PageUtils.RedirectToErrorPage($"修改失败：{ex.Message}");
                return;
            }

            PageUtils.CloseModalPage(Page);
        }
    }
}
