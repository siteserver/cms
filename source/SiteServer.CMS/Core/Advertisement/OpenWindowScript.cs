using SiteServer.CMS.Model;
using BaiRong.Core;

namespace SiteServer.CMS.Core.Advertisement
{
	public class OpenWindowScript
	{
        private PublishmentSystemInfo publishmentSystemInfo;
        private int uniqueID;
        private AdvertisementOpenWindowInfo adOpenWindowInfo;

        public OpenWindowScript(PublishmentSystemInfo publishmentSystemInfo, int uniqueID, AdvertisementInfo adInfo)
		{
            this.publishmentSystemInfo = publishmentSystemInfo;
            this.uniqueID = uniqueID;
            adOpenWindowInfo = new AdvertisementOpenWindowInfo(adInfo.Settings);
		}

		public string GetScript()
		{
            var sizeString = (adOpenWindowInfo.Width > 0) ? $",width={adOpenWindowInfo.Width}"
                : string.Empty;
            sizeString += (adOpenWindowInfo.Height > 0) ? $",height={adOpenWindowInfo.Height} " : string.Empty;

            return $@"
<script language=""javascript"" type=""text/javascript"">
function ad_open_win_{uniqueID}() {{
	var popUpWin{uniqueID} = open(""{PageUtils.AddProtocolToUrl(
                PageUtility.ParseNavigationUrl(publishmentSystemInfo, adOpenWindowInfo.FileUrl))}"", (window.name!=""popUpWin{uniqueID}"")?""popUpWin{uniqueID}"":"""", ""toolbar=no,location=no,directories=no,resizable=no,copyhistory=yes{sizeString}"");
}}
try{{
	setTimeout(""ad_open_win_{uniqueID}();"",50);
}}catch(e){{}}
</script>
";
		}
	}
}
