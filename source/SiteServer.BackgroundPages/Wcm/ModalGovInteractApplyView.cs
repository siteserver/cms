using System;
using System.Collections.Specialized;
using BaiRong.Core;

namespace SiteServer.BackgroundPages.Wcm
{
    public class ModalGovInteractApplyView : BasePageGovInteractPage
	{
        public static string GetOpenWindowString(int publishmentSystemId, int nodeId, int contentId)
        {
            return PageUtils.GetOpenWindowString("快速查看", PageUtils.GetWcmUrl(nameof(ModalGovInteractApplyView), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()},
                {"ContentID", contentId.ToString()},
                {"ReturnUrl", string.Empty}
            }), 750, 600, true);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            
        }
	}
}
