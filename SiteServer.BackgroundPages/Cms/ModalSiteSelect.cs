using System;
using System.Text;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalSiteSelect : BasePageCms
    {
        public Literal LtlHtml;

        public static string GetOpenLayerString(int siteId)
        {
            return LayerUtils.GetOpenScript("选择站点", PageUtils.GetCmsUrl(siteId, nameof(ModalSiteSelect), null));
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (IsPostBack) return;

            var builder = new StringBuilder();

            var siteIdList = AuthRequest.AdminPermissions.SiteIdList;
            foreach (var siteId in siteIdList)
            {
                var loadingUrl = PageUtils.GetLoadingUrl(PageMain.GetRedirectUrl(siteId));
                var siteInfo = SiteManager.GetSiteInfo(siteId);
                builder.Append($@"
<div class=""col-sm-6 col-lg-3"">
    <div class=""widget-simple text-center card-box"">
        <h3 class=""text-success counter"">
            <a href=""{loadingUrl}"" target=""_top"">
                {siteInfo.SiteName}
            </a>
        </h3>
        <p class=""text-muted"">{siteInfo.SiteDir}</p>
    </div>
</div>");
            }

            LtlHtml.Text = builder.ToString();
        }
    }
}
