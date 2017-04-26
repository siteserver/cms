using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageSeoSiteMapBaidu : BasePageCms
    {
        public TextBox SiteMapBaiduPath;
        public TextBox SiteMapBaiduUpdatePeri;
        public TextBox SiteMapBaiduWebMaster;

        public Literal ltlBaiduSiteMapUrl;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdFunction, AppManager.Cms.LeftMenu.Function.IdSeo, "站点地图(SiteMap) / 百度新闻地图", AppManager.Cms.Permission.WebSite.Seo);

                SiteMapBaiduPath.Text = PublishmentSystemInfo.Additional.SiteMapBaiduPath;
                SiteMapBaiduUpdatePeri.Text = PublishmentSystemInfo.Additional.SiteMapBaiduUpdatePeri;
                SiteMapBaiduWebMaster.Text = PublishmentSystemInfo.Additional.SiteMapBaiduWebMaster;

                var path =
                    PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(PublishmentSystemInfo,
                        PublishmentSystemInfo.Additional.SiteMapBaiduPath));
                ltlBaiduSiteMapUrl.Text = $@"<a href=""{path}"" target=""_blank"">{path}</a>";
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                PublishmentSystemInfo.Additional.SiteMapBaiduPath = SiteMapBaiduPath.Text;
                PublishmentSystemInfo.Additional.SiteMapBaiduUpdatePeri = SiteMapBaiduUpdatePeri.Text;
                PublishmentSystemInfo.Additional.SiteMapBaiduWebMaster = SiteMapBaiduWebMaster.Text;

                try
                {
                    DataProvider.PublishmentSystemDao.Update(PublishmentSystemInfo);
                    SeoManager.CreateSiteMapBaidu(PublishmentSystemInfo);
                    Body.AddSiteLog(PublishmentSystemId, "生成百度新闻地图");
                    SuccessMessage("百度新闻地图生成成功！");
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "百度新闻地图生成失败！");
                }
            }
        }
    }
}
