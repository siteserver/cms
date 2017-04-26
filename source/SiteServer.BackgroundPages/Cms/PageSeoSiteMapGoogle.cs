using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageSeoSiteMapGoogle : BasePageCms
    {
		public TextBox SiteMapGooglePath;
		public DropDownList SiteMapGoogleChangeFrequency;
		public RadioButtonList SiteMapGoogleIsShowLastModified;
        public TextBox SiteMapGooglePageCount;

        public Literal ltlGoogleSiteMapUrl;
        public Literal ltlYahooSiteMapUrl;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

			if (!IsPostBack)
			{
                BreadCrumb(AppManager.Cms.LeftMenu.IdFunction, AppManager.Cms.LeftMenu.Function.IdSeo, "站点地图(SiteMap) / 谷歌站点地图", AppManager.Cms.Permission.WebSite.Seo);

                SiteMapGooglePath.Text = PublishmentSystemInfo.Additional.SiteMapGooglePath;

                SiteMapGoogleChangeFrequency.Items.Add(new ListItem("每天", "daily"));
                SiteMapGoogleChangeFrequency.Items.Add(new ListItem("每周", "weekly"));
                SiteMapGoogleChangeFrequency.Items.Add(new ListItem("每月", "monthly"));
                SiteMapGoogleChangeFrequency.Items.Add(new ListItem("每年", "yearly"));
                SiteMapGoogleChangeFrequency.Items.Add(new ListItem("永不改变", "never"));
                ControlUtils.SelectListItemsIgnoreCase(SiteMapGoogleChangeFrequency, PublishmentSystemInfo.Additional.SiteMapGoogleChangeFrequency);

                EBooleanUtils.AddListItems(SiteMapGoogleIsShowLastModified, "显示", "不显示");
                ControlUtils.SelectListItemsIgnoreCase(SiteMapGoogleIsShowLastModified, PublishmentSystemInfo.Additional.SiteMapGoogleIsShowLastModified.ToString());

                SiteMapGooglePageCount.Text = PublishmentSystemInfo.Additional.SiteMapGooglePageCount.ToString();

			    var path =
			        PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(PublishmentSystemInfo,
			            PublishmentSystemInfo.Additional.SiteMapGooglePath));
                ltlGoogleSiteMapUrl.Text = ltlYahooSiteMapUrl.Text =
                    $@"<a href=""{path}"" target=""_blank"">{path}</a>";
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
                PublishmentSystemInfo.Additional.SiteMapGooglePath = SiteMapGooglePath.Text;
                PublishmentSystemInfo.Additional.SiteMapGoogleChangeFrequency = SiteMapGoogleChangeFrequency.SelectedValue;
                PublishmentSystemInfo.Additional.SiteMapGoogleIsShowLastModified = TranslateUtils.ToBool(SiteMapGoogleIsShowLastModified.SelectedValue);
                PublishmentSystemInfo.Additional.SiteMapGooglePageCount = TranslateUtils.ToInt(SiteMapGooglePageCount.Text);
				
				try
				{
                    DataProvider.PublishmentSystemDao.Update(PublishmentSystemInfo);
                    SeoManager.CreateSiteMapGoogle(PublishmentSystemInfo);
                    Body.AddSiteLog(PublishmentSystemId, "生成谷歌站点地图");
					SuccessMessage("谷歌站点地图生成成功！");
				}
				catch(Exception ex)
				{
                    FailMessage(ex, "谷歌站点地图生成失败！");
				}
			}
		}
	}
}
