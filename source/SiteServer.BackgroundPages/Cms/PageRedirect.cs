using System;
using BaiRong.Core;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageRedirect : BasePageCms
    {
        public void Page_Load(object sender, EventArgs e)
        {
            var type = Request["type"];

            if (type == "DynamicPage")
            {
                var redirectUrl = PagePreview.GetRedirectUrl(PublishmentSystemId, 0, 0, 0, 0);
                
                PageUtils.Redirect(redirectUrl);
            }
        }
    }
}
