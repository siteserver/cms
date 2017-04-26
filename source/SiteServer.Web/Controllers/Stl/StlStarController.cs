using System;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.Http;
using BaiRong.Core;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.API.Controllers.Stl
{
    [RoutePrefix("api")]
    public class StlStarController : ApiController
    {
        [HttpGet, Route(Star.Route)]
        public void Main(int publishmentSystemId, int channelId, int contentId)
        {
            var body = new RequestBody();

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);

            var updaterId = body.GetQueryInt("updaterId");
            var totalStar = body.GetQueryInt("totalStar");
            var initStar = body.GetQueryInt("initStar");
            var theme = PageUtils.FilterXss(body.GetQueryString("theme"));
            var isStar = body.GetQueryBool("isStar");
            var point = body.GetQueryInt("point");

            if (isStar)
            {
                DataProvider.StarDao.AddCount(publishmentSystemId, channelId, contentId, body.UserName, point, string.Empty, DateTime.Now);
            }

            var counts = DataProvider.StarDao.GetCount(publishmentSystemId, channelId, contentId);
            var totalCount = counts[0];
            var totalPoint = counts[1];

            var totalCountAndPointAverage = DataProvider.StarSettingDao.GetTotalCountAndPointAverage(publishmentSystemId, contentId);
            var settingTotalCount = (int)totalCountAndPointAverage[0];
            var settingPointAverage = (decimal)totalCountAndPointAverage[1];
            if (settingTotalCount > 0 || settingPointAverage > 0)
            {
                totalCount += settingTotalCount;
                totalPoint += Convert.ToInt32(settingPointAverage * settingTotalCount);
            }

            decimal num;
            if (totalCount > 0)
            {
                num = Convert.ToDecimal(totalPoint) / Convert.ToDecimal(totalCount);
            }
            else
            {
                num = initStar;
            }

            if (num > totalStar)
            {
                num = totalStar;
            }

            var numString = num.ToString(CultureInfo.InvariantCulture);
            if (numString.IndexOf('.') == -1)
            {
                numString += ".0";
            }
            var num1 = numString.Substring(0, numString.IndexOf('.'));
            var num2 = numString.Substring(numString.IndexOf('.') + 1, 1);

            point = (int)Math.Round(num);
            if (point > totalStar)
            {
                point = totalStar;
            }

            var builder = new StringBuilder();
            for (var i = 0; i < totalStar; i++)
            {
                builder.Append(GetItemHtml(publishmentSystemInfo, i, theme, point, initStar, updaterId, totalStar));
            }

            var retval = string.Empty;

            if (theme == "style1")
            {
                retval = string.Format($@"
<div style=""height:50px;"">
	<div class=""stlStar"">
		<span class=""shi"">{num1}</span><span class=""ge"">.{num2}</span>
		<div class=""stars"">
			{builder}
		</div>
		<span class=""scorer"">(<span>已有{totalCount}人评分</span>)</span>
	</div>
</div>");
            }
            else if (theme == "style2")
            {
                retval = string.Format($@"
<span class=""stlStar"">
	<span class=""stars"">
    	{builder}
    </span>
</span>
");
            }
            else if (theme == "style3")
            {
                retval = string.Format($@"
<div class=""stlStar"">
	<div class=""stars"">
    	{builder}
        <span class=""font-num"">{num1}.{num2}</span>
    </div>
</div>
");
            }

            HttpContext.Current.Response.Write(retval);
            HttpContext.Current.Response.End();
        }

        private static string GetItemHtml(PublishmentSystemInfo publishmentSystemInfo, int currentNum, string theme, int point, int initStar, int updaterId, int totalStar)
        {
            string imageName = $"{theme}_off.gif";
            if (currentNum <= point || (initStar > 0 && initStar >= currentNum))
            {
                imageName = $"{theme}_on.gif";
            }

            var clickString = $"stlStarPoint_{updaterId}({currentNum});return false;";
            var starUrl = SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, "star");

            return $@"<img id=""stl_star_item_{updaterId}_{currentNum}"" alt=""{currentNum}"" onmouseover=""stlStarDraw({currentNum}, {totalStar}, {updaterId}, '{theme}', '{starUrl}')"" onmouseout=""stlStarInit({totalStar}, {updaterId})"" oriSrc=""{starUrl}/{imageName}"" src=""{starUrl}/{imageName}"" onclick=""{clickString}""/>";
        }
    }
}
