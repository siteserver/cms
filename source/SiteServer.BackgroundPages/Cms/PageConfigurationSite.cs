using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageConfigurationSite : BasePageCms
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

        public DropDownList DdlCharset;
        public TextBox TbPageSize;
        public RadioButtonList RblIsCreateDoubleClick;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

			if (!IsPostBack)
			{
                BreadCrumb(AppManager.Cms.LeftMenu.IdConfigration, "站点配置管理", AppManager.Permissions.WebSite.Configration);

                TbPublishmentSystemUrl.Text = PublishmentSystemInfo.PublishmentSystemUrl;
                TbHomeUrl.Text = PublishmentSystemInfo.Additional.HomeUrl;
                EBooleanUtils.AddListItems(DdlIsMultiDeployment, "内外网分离部署", "默认部署");
                ControlUtils.SelectListItems(DdlIsMultiDeployment, PublishmentSystemInfo.Additional.IsMultiDeployment.ToString());

                TbSiteUrl.Text = PublishmentSystemInfo.Additional.SiteUrl;
                TbApiUrl.Text = PublishmentSystemInfo.Additional.ApiUrl;

                TbOuterSiteUrl.Text = PublishmentSystemInfo.Additional.OuterSiteUrl;
                TbInnerSiteUrl.Text = PublishmentSystemInfo.Additional.InnerSiteUrl;
                TbOuterApiUrl.Text = PublishmentSystemInfo.Additional.OuterApiUrl;
                TbInnerApiUrl.Text = PublishmentSystemInfo.Additional.InnerApiUrl;

                DdlIsMultiDeployment_SelectedIndexChanged(null, EventArgs.Empty);

                ECharsetUtils.AddListItems(DdlCharset);
                ControlUtils.SelectListItems(DdlCharset, PublishmentSystemInfo.Additional.Charset);

                TbPageSize.Text = PublishmentSystemInfo.Additional.PageSize.ToString();

                EBooleanUtils.AddListItems(RblIsCreateDoubleClick, "启用双击生成", "不启用");
                ControlUtils.SelectListItemsIgnoreCase(RblIsCreateDoubleClick, PublishmentSystemInfo.Additional.IsCreateDoubleClick.ToString());
            }
		}

        public void DdlIsMultiDeployment_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhMulti.Visible = TranslateUtils.ToBool(DdlIsMultiDeployment.SelectedValue);
            PhSingle.Visible = !PhMulti.Visible;
        }

        public override void Submit_OnClick(object sender, EventArgs e)
		{
		    if (!Page.IsPostBack || !Page.IsValid) return;

		    PublishmentSystemInfo.PublishmentSystemUrl = TbPublishmentSystemUrl.Text;
                
		    PublishmentSystemInfo.Additional.IsMultiDeployment = TranslateUtils.ToBool(DdlIsMultiDeployment.SelectedValue);
            PublishmentSystemInfo.Additional.SiteUrl = TbSiteUrl.Text;
            PublishmentSystemInfo.Additional.ApiUrl = TbApiUrl.Text;
            PublishmentSystemInfo.Additional.OuterSiteUrl = TbOuterSiteUrl.Text;
		    PublishmentSystemInfo.Additional.InnerSiteUrl = TbInnerSiteUrl.Text;
            PublishmentSystemInfo.Additional.OuterApiUrl = TbOuterApiUrl.Text;
            PublishmentSystemInfo.Additional.InnerApiUrl = TbInnerApiUrl.Text;

            PublishmentSystemInfo.Additional.HomeUrl = TbHomeUrl.Text;

		    if (PublishmentSystemInfo.Additional.Charset != DdlCharset.SelectedValue)
		    {
		        PublishmentSystemInfo.Additional.Charset = DdlCharset.SelectedValue;
		    }

		    PublishmentSystemInfo.Additional.PageSize = TranslateUtils.ToInt(TbPageSize.Text, PublishmentSystemInfo.Additional.PageSize);
		    PublishmentSystemInfo.Additional.IsCreateDoubleClick = TranslateUtils.ToBool(RblIsCreateDoubleClick.SelectedValue);
                
		    try
		    {
		        //修改所有模板编码
		        var templateInfoList = DataProvider.TemplateDao.GetTemplateInfoListByPublishmentSystemId(PublishmentSystemId);
		        var charset = ECharsetUtils.GetEnumType(PublishmentSystemInfo.Additional.Charset);
		        foreach (var templateInfo in templateInfoList)
		        {
		            if (templateInfo.Charset == charset) continue;

		            var templateContent = TemplateManager.GetTemplateContent(PublishmentSystemInfo, templateInfo);
		            templateInfo.Charset = charset;
		            DataProvider.TemplateDao.Update(PublishmentSystemInfo, templateInfo, templateContent, Body.AdministratorName);
		        }

		        DataProvider.PublishmentSystemDao.Update(PublishmentSystemInfo);

		        Body.AddSiteLog(PublishmentSystemId, "修改站点配置管理");

		        SuccessMessage("站点配置管理修改成功！");
		    }
		    catch(Exception ex)
		    {
		        FailMessage(ex, "站点配置管理修改失败！");
		    }
		}
	}
}
