using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageConfigurationSite : BasePageCms
    {
        public PlaceHolder PhUrlSettings;

        public DropDownList DdlIsSeparatedWeb;
        public PlaceHolder PhSeparatedWeb;
        public TextBox TbSeparatedWebUrl;

        public DropDownList DdlCharset;
        public TextBox TbPageSize;
        public DropDownList DdlIsCreateDoubleClick;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

			if (!IsPostBack)
			{
                BreadCrumb(AppManager.Cms.LeftMenu.IdConfigration, "站点配置管理", AppManager.Permissions.WebSite.Configration);

			    PhUrlSettings.Visible = !ConfigManager.SystemConfigInfo.IsUrlGlobalSetting;

                EBooleanUtils.AddListItems(DdlIsSeparatedWeb, "Web独立部署", "Web与CMS部署在一起");
                ControlUtils.SelectListItems(DdlIsSeparatedWeb, PublishmentSystemInfo.Additional.IsSeparatedWeb.ToString());
                PhSeparatedWeb.Visible = PublishmentSystemInfo.Additional.IsSeparatedWeb;
                TbSeparatedWebUrl.Text = PublishmentSystemInfo.Additional.SeparatedWebUrl;

                ECharsetUtils.AddListItems(DdlCharset);
                ControlUtils.SelectListItems(DdlCharset, PublishmentSystemInfo.Additional.Charset);

                TbPageSize.Text = PublishmentSystemInfo.Additional.PageSize.ToString();

                EBooleanUtils.AddListItems(DdlIsCreateDoubleClick, "启用双击生成", "不启用");
                ControlUtils.SelectListItemsIgnoreCase(DdlIsCreateDoubleClick, PublishmentSystemInfo.Additional.IsCreateDoubleClick.ToString());
            }
		}

        public void DdlIsSeparatedWeb_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhSeparatedWeb.Visible = TranslateUtils.ToBool(DdlIsSeparatedWeb.SelectedValue);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
		{
		    if (!Page.IsPostBack || !Page.IsValid) return;

            PublishmentSystemInfo.Additional.IsSeparatedWeb = TranslateUtils.ToBool(DdlIsSeparatedWeb.SelectedValue);
            PublishmentSystemInfo.Additional.SeparatedWebUrl = TbSeparatedWebUrl.Text;

            if (PublishmentSystemInfo.Additional.Charset != DdlCharset.SelectedValue)
		    {
		        PublishmentSystemInfo.Additional.Charset = DdlCharset.SelectedValue;
		    }

		    PublishmentSystemInfo.Additional.PageSize = TranslateUtils.ToInt(TbPageSize.Text, PublishmentSystemInfo.Additional.PageSize);
		    PublishmentSystemInfo.Additional.IsCreateDoubleClick = TranslateUtils.ToBool(DdlIsCreateDoubleClick.SelectedValue);
                
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
		            DataProvider.TemplateDao.Update(PublishmentSystemInfo, templateInfo, templateContent, Body.AdminName);
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
