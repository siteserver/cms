using System;
using System.Collections.Specialized;
using BaiRong.Core;

namespace SiteServer.BackgroundPages.Wcm
{
    public class ModalGovPublicApplyView : BasePageGovPublicApplyToDetail
	{
	    public static string GetOpenWindowString(int publishmentSystemId, int applyId)
	    {
	        return PageUtils.GetOpenWindowString("快速查看",
	            PageUtils.GetWcmUrl(nameof(ModalGovPublicApplyView), new NameValueCollection
	            {
	                {"PublishmentSystemID", publishmentSystemId.ToString()},
	                {"ApplyID", applyId.ToString()},
	                {"ReturnUrl", string.Empty}
	            }), 750, 600, true);
	    }

	    public void Page_Load(object sender, EventArgs e)
        {
            
        }
	}
}
