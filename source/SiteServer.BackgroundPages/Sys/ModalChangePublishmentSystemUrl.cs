using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Sys
{
    public class ModalChangePublishmentSystemUrl : BasePageCms
    {
        public TextBox tbPublishmentSystemUrl;
        public DropDownList ddlIsMultiDeployment;
        public PlaceHolder phIsMultiDeployment;
        public TextBox tbOuterUrl;
        public TextBox tbInnerUrl;
        public TextBox tbAPIUrl;
        public TextBox tbHomeUrl;

        public static string GetOpenWindowString(int publishmentSystemId)
        {
            return PageUtils.GetOpenWindowString("修改访问地址",
                PageUtils.GetSysUrl(nameof(ModalChangePublishmentSystemUrl), new NameValueCollection
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
                tbPublishmentSystemUrl.Text = PublishmentSystemInfo.PublishmentSystemUrl;
                EBooleanUtils.AddListItems(ddlIsMultiDeployment, "内外网分离部署", "默认部署");
                ControlUtils.SelectListItems(ddlIsMultiDeployment, PublishmentSystemInfo.Additional.IsMultiDeployment.ToString());
                tbOuterUrl.Text = PublishmentSystemInfo.Additional.OuterUrl;
                tbInnerUrl.Text = PublishmentSystemInfo.Additional.InnerUrl;
                tbAPIUrl.Text = PublishmentSystemInfo.Additional.ApiUrl;
                tbHomeUrl.Text = PublishmentSystemInfo.Additional.HomeUrl;

                ddlIsMultiDeployment_SelectedIndexChanged(null, EventArgs.Empty);
            }
        }

        public void ddlIsMultiDeployment_SelectedIndexChanged(object sender, EventArgs e)
        {
            phIsMultiDeployment.Visible = TranslateUtils.ToBool(ddlIsMultiDeployment.SelectedValue);
        }

        public string GetSiteName()
        {
            return PublishmentSystemInfo.PublishmentSystemName;
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;

            try
            {
                PublishmentSystemInfo.PublishmentSystemUrl = tbPublishmentSystemUrl.Text;
                PublishmentSystemInfo.Additional.IsMultiDeployment = TranslateUtils.ToBool(ddlIsMultiDeployment.SelectedValue);
                PublishmentSystemInfo.Additional.OuterUrl = tbOuterUrl.Text;
                PublishmentSystemInfo.Additional.InnerUrl = tbInnerUrl.Text;
                PublishmentSystemInfo.Additional.ApiUrl = tbAPIUrl.Text;
                PublishmentSystemInfo.Additional.HomeUrl = tbHomeUrl.Text;
                DataProvider.PublishmentSystemDao.Update(PublishmentSystemInfo);
                Body.AddSiteLog(PublishmentSystemId, "修改网站访问设置");

                isChanged = true;
            }
            catch (Exception ex)
            {
                PageUtils.RedirectToErrorPage($"修改失败：{ex.Message}");
                return;
            }

            if (isChanged)
            {
                PageUtils.CloseModalPage(Page);
            }
        }
    }
}
