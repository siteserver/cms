using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageConfigurationSite : BasePageCms
    {
        public TextBox tbPublishmentSystemUrl;
        public TextBox tbHomeUrl;
        public DropDownList ddlIsMultiDeployment;
        public PlaceHolder phIsMultiDeployment;
        public TextBox tbOuterUrl;
        public TextBox tbInnerUrl;
        public TextBox tbAPIUrl;

        public DropDownList Charset;
        public TextBox PageSize;
        public RadioButtonList IsCountHits;
        public PlaceHolder phIsCountHitsByDay;
        public RadioButtonList IsCountHitsByDay;
        public RadioButtonList IsCountDownload;
        public RadioButtonList IsCreateDoubleClick;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

			if (!IsPostBack)
			{
                BreadCrumb(AppManager.Cms.LeftMenu.IdConfigration, "站点配置管理", AppManager.Cms.Permission.WebSite.Configration);

                tbPublishmentSystemUrl.Text = PublishmentSystemInfo.PublishmentSystemUrl;
                tbHomeUrl.Text = PublishmentSystemInfo.Additional.HomeUrl;
                EBooleanUtils.AddListItems(ddlIsMultiDeployment, "内外网分离部署", "默认部署");
                ControlUtils.SelectListItems(ddlIsMultiDeployment, PublishmentSystemInfo.Additional.IsMultiDeployment.ToString());
                tbOuterUrl.Text = PublishmentSystemInfo.Additional.OuterUrl;
                tbInnerUrl.Text = PublishmentSystemInfo.Additional.InnerUrl;

                tbAPIUrl.Text = PublishmentSystemInfo.Additional.ApiUrl;

                ddlIsMultiDeployment_SelectedIndexChanged(null, EventArgs.Empty);

                ECharsetUtils.AddListItems(Charset);
                ControlUtils.SelectListItems(Charset, PublishmentSystemInfo.Additional.Charset);

                PageSize.Text = PublishmentSystemInfo.Additional.PageSize.ToString();

                EBooleanUtils.AddListItems(IsCountHits, "统计", "不统计");
                ControlUtils.SelectListItemsIgnoreCase(IsCountHits, PublishmentSystemInfo.Additional.IsCountHits.ToString());

                EBooleanUtils.AddListItems(IsCountHitsByDay, "统计", "不统计");
                ControlUtils.SelectListItemsIgnoreCase(IsCountHitsByDay, PublishmentSystemInfo.Additional.IsCountHitsByDay.ToString());

                EBooleanUtils.AddListItems(IsCountDownload, "统计", "不统计");
                ControlUtils.SelectListItemsIgnoreCase(IsCountDownload, PublishmentSystemInfo.Additional.IsCountDownload.ToString());

                EBooleanUtils.AddListItems(IsCreateDoubleClick, "启用双击生成", "不启用");
                ControlUtils.SelectListItemsIgnoreCase(IsCreateDoubleClick, PublishmentSystemInfo.Additional.IsCreateDoubleClick.ToString());
                
                IsCountHits_SelectedIndexChanged(null, EventArgs.Empty);
            }
		}

        public void ddlIsMultiDeployment_SelectedIndexChanged(object sender, EventArgs e)
        {
            phIsMultiDeployment.Visible = TranslateUtils.ToBool(ddlIsMultiDeployment.SelectedValue);
        }

        public void IsCountHits_SelectedIndexChanged(object sender, EventArgs e)
        {
            phIsCountHitsByDay.Visible = TranslateUtils.ToBool(IsCountHits.SelectedValue);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
                PublishmentSystemInfo.PublishmentSystemUrl = tbPublishmentSystemUrl.Text;
                
                PublishmentSystemInfo.Additional.IsMultiDeployment = TranslateUtils.ToBool(ddlIsMultiDeployment.SelectedValue);
                PublishmentSystemInfo.Additional.OuterUrl = tbOuterUrl.Text;
                PublishmentSystemInfo.Additional.InnerUrl = tbInnerUrl.Text;
                PublishmentSystemInfo.Additional.ApiUrl = tbAPIUrl.Text;
                PublishmentSystemInfo.Additional.HomeUrl = tbHomeUrl.Text;

                if (PublishmentSystemInfo.Additional.Charset != Charset.SelectedValue)
                {
                    PublishmentSystemInfo.Additional.Charset = Charset.SelectedValue;
                }

                PublishmentSystemInfo.Additional.PageSize = TranslateUtils.ToInt(PageSize.Text, PublishmentSystemInfo.Additional.PageSize);
                PublishmentSystemInfo.Additional.IsCountHits = TranslateUtils.ToBool(IsCountHits.SelectedValue);
                PublishmentSystemInfo.Additional.IsCountHitsByDay = TranslateUtils.ToBool(IsCountHitsByDay.SelectedValue);
                PublishmentSystemInfo.Additional.IsCountDownload = TranslateUtils.ToBool(IsCountDownload.SelectedValue);

                PublishmentSystemInfo.Additional.IsCreateDoubleClick = TranslateUtils.ToBool(IsCreateDoubleClick.SelectedValue);
                
				try
				{
                    //修改所有模板编码
                    var templateInfoArrayList = DataProvider.TemplateDao.GetTemplateInfoArrayListByPublishmentSystemId(PublishmentSystemId);
                    var charset = ECharsetUtils.GetEnumType(PublishmentSystemInfo.Additional.Charset);
                    foreach (TemplateInfo templateInfo in templateInfoArrayList)
                    {
                        if (templateInfo.Charset != charset)
                        {
                            var templateContent = StlCacheManager.FileContent.GetTemplateContent(PublishmentSystemInfo, templateInfo);
                            templateInfo.Charset = charset;
                            DataProvider.TemplateDao.Update(PublishmentSystemInfo, templateInfo, templateContent, Body.AdministratorName);
                        }
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
}
