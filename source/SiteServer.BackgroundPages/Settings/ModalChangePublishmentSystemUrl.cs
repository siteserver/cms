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
        public TextBox TbPublishmentSystemUrl;
        public TextBox TbHomeUrl;

        public DropDownList DdlIsMultiDeployment;
        public PlaceHolder PhSingle;
        public TextBox TbSiteUrl;
        public TextBox TbApiUrl;
        public PlaceHolder PhMulti;
        public TextBox TbOuterSiteUrl;
        public TextBox TbInnerSiteUrl;
        public TextBox TbOuterApiUrl;
        public TextBox TbInnerApiUrl;

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

            if (!Page.IsPostBack)
            {
                TbPublishmentSystemUrl.Text = PublishmentSystemInfo.PublishmentSystemUrl;
                EBooleanUtils.AddListItems(DdlIsMultiDeployment, "内外网分离部署", "默认部署");
                ControlUtils.SelectListItems(DdlIsMultiDeployment, PublishmentSystemInfo.Additional.IsMultiDeployment.ToString());

                TbSiteUrl.Text = PublishmentSystemInfo.Additional.SiteUrl;
                TbApiUrl.Text = PublishmentSystemInfo.Additional.ApiUrl;

                TbOuterSiteUrl.Text = PublishmentSystemInfo.Additional.OuterSiteUrl;
                TbInnerSiteUrl.Text = PublishmentSystemInfo.Additional.InnerSiteUrl;
                TbOuterApiUrl.Text = PublishmentSystemInfo.Additional.OuterApiUrl;
                TbInnerApiUrl.Text = PublishmentSystemInfo.Additional.InnerApiUrl;

                TbHomeUrl.Text = PublishmentSystemInfo.Additional.HomeUrl;

                DdlIsMultiDeployment_SelectedIndexChanged(null, EventArgs.Empty);
            }
        }

        public void DdlIsMultiDeployment_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhMulti.Visible = TranslateUtils.ToBool(DdlIsMultiDeployment.SelectedValue);
            PhSingle.Visible = !PhMulti.Visible;
        }

        public string GetSiteName()
        {
            return PublishmentSystemInfo.PublishmentSystemName;
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            try
            {
                PublishmentSystemInfo.PublishmentSystemUrl = TbPublishmentSystemUrl.Text;

                PublishmentSystemInfo.Additional.IsMultiDeployment = TranslateUtils.ToBool(DdlIsMultiDeployment.SelectedValue);
                PublishmentSystemInfo.Additional.SiteUrl = TbSiteUrl.Text;
                PublishmentSystemInfo.Additional.ApiUrl = TbApiUrl.Text;
                PublishmentSystemInfo.Additional.OuterSiteUrl = TbOuterSiteUrl.Text;
                PublishmentSystemInfo.Additional.InnerSiteUrl = TbInnerSiteUrl.Text;
                PublishmentSystemInfo.Additional.OuterApiUrl = TbOuterApiUrl.Text;
                PublishmentSystemInfo.Additional.InnerApiUrl = TbInnerApiUrl.Text;

                PublishmentSystemInfo.Additional.HomeUrl = TbHomeUrl.Text;

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
