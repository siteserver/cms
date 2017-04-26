using SiteServer.CMS.Model;
using BaiRong.Core;

namespace SiteServer.CMS.Core.Advertisement
{
	public class ScreenDownScript
	{
        private PublishmentSystemInfo publishmentSystemInfo;
        private int uniqueID;

        private readonly AdvertisementScreenDownInfo adScreenDownInfo;

        public ScreenDownScript(PublishmentSystemInfo publishmentSystemInfo, int uniqueID, AdvertisementInfo adInfo)
		{
            this.publishmentSystemInfo = publishmentSystemInfo;
            this.uniqueID = uniqueID;

            adScreenDownInfo = new AdvertisementScreenDownInfo(adInfo.Settings);
		}

		public string GetScript()
		{
            var sizeString = (adScreenDownInfo.Width > 0) ? $"width={adScreenDownInfo.Width} "
                : string.Empty;
            sizeString += (adScreenDownInfo.Height > 0) ? $"height={adScreenDownInfo.Height}" : string.Empty;

            return $@"
<script language=""javascript"" type=""text/javascript"">
function ad_changediv(){{
    jQuery('#ad_hiddenLayer_{uniqueID}').slideDown();
    setTimeout(""ad_hidediv()"",{adScreenDownInfo.Delay}000);
}}
function ad_hidediv(){{
    jQuery('#ad_hiddenLayer_{uniqueID}').slideUp();
}}
jQuery(document).ready(function(){{
    jQuery('body').prepend('<div id=""ad_hiddenLayer_{uniqueID}"" style=""display: none;""><center><a href=""{PageUtils
                .AddProtocolToUrl(PageUtility.ParseNavigationUrl(publishmentSystemInfo,
                    adScreenDownInfo.NavigationUrl))}"" target=""_blank""><img src=""{PageUtility
                .ParseNavigationUrl(publishmentSystemInfo, adScreenDownInfo.ImageUrl)}"" {sizeString} border=""0"" /></a></center></div>');
    setTimeout(""ad_changediv()"",2000);
}});
</script>
";
		}
	}
}
