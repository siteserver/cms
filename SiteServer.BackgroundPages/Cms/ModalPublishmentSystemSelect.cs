using System;
using System.Text;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalPublishmentSystemSelect : BasePageCms
    {
        public Literal LtlHtml;

        public static string GetOpenLayerString()
        {
            return LayerUtils.GetOpenScript("选择站点", PageUtils.GetCmsUrl(nameof(ModalPublishmentSystemSelect), null));
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (IsPostBack) return;

            var builder = new StringBuilder();

            var publishmentSystemIdList = ProductPermissionsManager.Current.PublishmentSystemIdList;
            foreach (var publishmentSystemId in publishmentSystemIdList)
            {
                var loadingUrl = PageUtils.GetLoadingUrl(PageMain.GetRedirectUrl(publishmentSystemId));
                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                builder.Append($@"
<div class=""col-sm-6 col-lg-3"">
    <div class=""widget-simple text-center card-box"">
        <h3 class=""text-success counter"">
            <a href=""{loadingUrl}"" target=""_top"">
                {publishmentSystemInfo.PublishmentSystemName}
            </a>
        </h3>
        <p class=""text-muted"">{publishmentSystemInfo.PublishmentSystemDir}</p>
    </div>
</div>");
            }

            LtlHtml.Text = builder.ToString();
        }
    }
}
