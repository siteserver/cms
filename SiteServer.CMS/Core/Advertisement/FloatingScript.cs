using SiteServer.CMS.Model;
using BaiRong.Core;
using System.Text;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Core.Advertisement
{
	public class FloatingScript
	{
        private int uniqueID;
        private PublishmentSystemInfo publishmentSystemInfo;
		
		private string floatDivIsCloseableHtml;

        private AdvertisementInfo adInfo;
        private AdvertisementFloatImageInfo adFloatImageInfo;

        public FloatingScript(PublishmentSystemInfo publishmentSystemInfo, int uniqueID, AdvertisementInfo adInfo)
		{
            this.publishmentSystemInfo = publishmentSystemInfo;
            this.adInfo = adInfo;
            adFloatImageInfo = new AdvertisementFloatImageInfo(this.adInfo.Settings);
            this.uniqueID = uniqueID;

            floatDivIsCloseableHtml = (adFloatImageInfo.IsCloseable) ?
                $@"<div style=""text-align:right; line-height:22px;""><a href=""javascript:;"" onclick=""document.getElementById('ad_{this
                    .uniqueID}').style.display='none'"" style=""text-decoration:underline"">关闭<a></div>"
                : string.Empty;
		}

		public string GetScript()
		{
            var builder = new StringBuilder();
            builder.Append($@"<div id=""ad_{uniqueID}"">");
            if (EFileSystemTypeUtils.IsFlash(PathUtils.GetExtension(adFloatImageInfo.ImageUrl)))
            {
                var height = (adFloatImageInfo.Height > 0) ? adFloatImageInfo.Height.ToString() : "100";
                var width = (adFloatImageInfo.Width > 0) ? adFloatImageInfo.Width.ToString() : "100";
                var value = PageUtility.ParseNavigationUrl(publishmentSystemInfo, adFloatImageInfo.ImageUrl);
                builder.Append(
                    $@"<object classid='clsid:D27CDB6E-AE6D-11cf-96B8-444553540000' codebase='http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=7,0,19,0' width='{width}' height='{height}'><param name='movie' value='{value}' /><param name='wmode' value='window' /><param name='scale' value='exactfit' /><param name='quality' value='high' /><embed src='{value}' quality='high' pluginspage='http://www.macromedia.com/go/getflashplayer' type='application/x-shockwave-flash' width='{width}' height='{height}'></embed></object>{floatDivIsCloseableHtml}</div>");
            }
            else
            {
                var floatDivSize = (adFloatImageInfo.Height > 0) ? $" HEIGHT={adFloatImageInfo.Height}" : "";
                floatDivSize += (adFloatImageInfo.Width > 0) ? $" WIDTH={adFloatImageInfo.Width} " : "";

                if (string.IsNullOrEmpty(adFloatImageInfo.NavigationUrl))
                {
                    builder.Append(
                        $@"<img src=""{PageUtility.ParseNavigationUrl(publishmentSystemInfo, adFloatImageInfo.ImageUrl)}"" {floatDivSize} border=""0""></a>{floatDivIsCloseableHtml}");
                }
                else
                {
                    builder.Append(
                        $@"<a href=""{PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(publishmentSystemInfo,
                            adFloatImageInfo.NavigationUrl))}"" target = ""_blank""><img src=""{PageUtility
                            .ParseNavigationUrl(publishmentSystemInfo, adFloatImageInfo.ImageUrl)}"" {floatDivSize} border=""0""></a>{floatDivIsCloseableHtml}");
                }
            }
            builder.Append("</div>");
            var type = 1;
            if (adFloatImageInfo.RollingType == ERollingType.FloatingInWindow)
            {
                type = 1;
            }
            else if (adFloatImageInfo.RollingType == ERollingType.FollowingScreen)
            {
                type = 2;
            }
            else if (adFloatImageInfo.RollingType == ERollingType.Static)
            {
                type = 3;
            }

            var positionX = string.Empty;
            var positionY = string.Empty;
            if (adFloatImageInfo.PositionType == EPositionType.LeftTop)
            {
                positionX = adFloatImageInfo.PositionX.ToString();
                positionY = adFloatImageInfo.PositionY.ToString();
            }
            else if (adFloatImageInfo.PositionType == EPositionType.LeftBottom)
            {
                positionX = adFloatImageInfo.PositionX.ToString();
                positionY =
                    $@"document.body.scrollTop+document.body.offsetHeight-{adFloatImageInfo.PositionY}-{adFloatImageInfo
                        .Height}";
            }
            else if (adFloatImageInfo.PositionType == EPositionType.RightTop)
            {
                positionX =
                    $@"document.body.scrollLeft+document.body.offsetWidth-{adFloatImageInfo.PositionX}-{adFloatImageInfo
                        .Width}";
                positionY = adFloatImageInfo.PositionY.ToString();
            }
            else if (adFloatImageInfo.PositionType == EPositionType.RightBottom)
            {
                positionX =
                    $@"document.body.scrollLeft+document.body.offsetWidth-{adFloatImageInfo.PositionX}-{adFloatImageInfo
                        .Width}";
                positionY =
                    $@"document.body.scrollTop+document.body.offsetHeight-{adFloatImageInfo.PositionY}-{adFloatImageInfo
                        .Height}";
            }

            var dateLimited = string.Empty;
            if (adInfo.IsDateLimited)
            {
                dateLimited = $@"
    var sDate{uniqueID} = new Date({adInfo.StartDate.Year}, {adInfo.StartDate.Month - 1}, {adInfo.StartDate.Day}, {adInfo
                    .StartDate.Hour}, {adInfo.StartDate.Minute});
    var eDate{uniqueID} = new Date({adInfo.EndDate.Year}, {adInfo.EndDate.Month - 1}, {adInfo.EndDate.Day}, {adInfo
                    .EndDate.Hour}, {adInfo.EndDate.Minute});
    ad{uniqueID}.SetDate(sDate{uniqueID}, eDate{uniqueID});
";
            }

            builder.Append($@"
<script type=""text/javascript"">
<!--
    var ad{uniqueID}=new Ad_Move(""ad_{uniqueID}"");
    ad{uniqueID}.SetLocation({positionX}, {positionY});
    ad{uniqueID}.SetType({type});{dateLimited}
    ad{uniqueID}.Run();
//-->
</script>
");

            return builder.ToString();
		}
	}
}
