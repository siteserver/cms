using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageConfigurationSite : BasePageCms
    {
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

                ECharsetUtils.AddListItems(DdlCharset);
                ControlUtils.SelectListItems(DdlCharset, PublishmentSystemInfo.Additional.Charset);

                TbPageSize.Text = PublishmentSystemInfo.Additional.PageSize.ToString();

                EBooleanUtils.AddListItems(DdlIsCreateDoubleClick, "启用双击生成", "不启用");
                ControlUtils.SelectListItemsIgnoreCase(DdlIsCreateDoubleClick, PublishmentSystemInfo.Additional.IsCreateDoubleClick.ToString());
            }
		}

        public override void Submit_OnClick(object sender, EventArgs e)
		{
		    if (!Page.IsPostBack || !Page.IsValid) return;

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
