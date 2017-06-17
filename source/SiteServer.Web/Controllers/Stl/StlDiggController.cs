using System;
using System.Web;
using System.Web.Http;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Controllers.Stl;

namespace SiteServer.API.Controllers.Stl
{
    [RoutePrefix("api")]
    public class StlDiggController : ApiController
    {
        [HttpGet]
        [Route(Digg.Route)]
        public void Main(int publishmentSystemId)
        {
            var queryString = HttpContext.Current.Request.QueryString;

            var updaterId = int.Parse(queryString["updaterId"]);
            var relatedIdentity = int.Parse(queryString["relatedIdentity"]);
            var diggType = EDiggTypeUtils.GetEnumType(queryString["diggType"]);
            var goodText = TranslateUtils.DecryptStringBySecretKey(queryString["goodText"]);
            var badText = TranslateUtils.DecryptStringBySecretKey(queryString["badText"]);
            var theme = queryString["theme"];
            var isDigg = TranslateUtils.ToBool(queryString["isDigg"]);
            var isGood = TranslateUtils.ToBool(queryString["isGood"]);

            if (isDigg)
            {
                BaiRongDataProvider.DiggDao.AddCount(publishmentSystemId, relatedIdentity, isGood);
            }

            var counts = BaiRongDataProvider.DiggDao.GetCount(publishmentSystemId, relatedIdentity);
            var goodNum = counts[0];
            var badNum = counts[1];

            var goodDisplay = diggType != EDiggType.Bad ? "" : "display: none";
            var badDisplay = diggType != EDiggType.Good ? "" : "display: none";

            var clickStringOfGood = $"stlDiggSet_{updaterId}(true);return false;";
            var clickStringOfBad = $"stlDiggSet_{updaterId}(false);return false;";

            decimal goodPercentage;
            decimal badPercentage;

            if (goodNum == 0 && badNum == 0)
            {
                goodPercentage = 0;
                badPercentage = 0;
            }
            else if (goodNum > 0 && badNum == 0)
            {
                goodPercentage = 100;
                badPercentage = 0;
            }
            else if (goodNum == 0 && badNum > 0)
            {
                goodPercentage = 0;
                badPercentage = 100;
            }
            else
            {
                goodPercentage = Math.Round((Convert.ToDecimal(goodNum) / Convert.ToDecimal(goodNum + badNum)) * Convert.ToDecimal(100));
                badPercentage = 100 - goodPercentage;
            }

            var retval = string.Empty;

            if (theme == "style1")
            {
                retval = string.Format($@"
<div class=""newdigg"" id=""newdigg"">
	<div class=""diggbox digg_good"" onmousemove=""this.style.backgroundPosition='left bottom';"" onmouseout=""this.style.backgroundPosition='left top';"" onclick=""{clickStringOfGood}"" style=""display:{goodDisplay}"">
        <div class=""digg_act"">{goodText}</div>
		<div class=""digg_num"">({goodNum})</div>
		<div class=""digg_percent"">
			<div class=""digg_percent_bar""><span style=""width:{goodPercentage}%""></span></div>
			<div class=""digg_percent_num"">{goodPercentage}%</div>
		</div>
	</div>
    <div class=""diggbox digg_bad"" onmousemove=""this.style.backgroundPosition='right bottom';"" onmouseout=""this.style.backgroundPosition='right top';"" onclick=""{clickStringOfBad}"" style=""{badDisplay}"">
		<div class=""digg_act"">{badText}</div>
		<div class=""digg_num"">({badNum})</div>
		<div class=""digg_percent"">
			<div class=""digg_percent_bar""><span style=""width:{badPercentage}%""></span></div>
			<div class=""digg_percent_num"">{badPercentage}%</div>
		</div>
	</div>
</div>");
            }
            else if (theme == "style2")
            {
                retval = string.Format($@"
<table border=""0"" cellpadding=""0"" cellspacing=""8"" class=""newdigg"">
  <tr>
    <td style=""{goodDisplay}"">
      <table border=""0"" align=""center"" cellpadding=""0"" cellspacing=""0"" class=""digg"">
        <tr>
          <td class=""diggnum"" id=""diggnum"">
            <strong>{goodNum}</strong>
          </td>
        </tr>
        <tr>
          <td class=""diggit"">
            <a href=""javascript:;"" onclick=""{clickStringOfGood}"">{goodText}</a>
          </td>
        </tr>
      </table>
    </td>
    <td style=""{badDisplay}"">
      <table border=""0"" align=""center"" cellpadding=""0"" cellspacing=""0"" class=""digg"">
        <tr>
          <td class=""diggnum"" id=""diggnum"">
            <strong>{badNum}</strong>
          </td>
        </tr>
        <tr>
          <td class=""diggit"">
            <a href=""javascript:;"" onclick=""{clickStringOfBad}"">{badText}</a>
          </td>
        </tr>
      </table>
    </td>
  </tr>
</table>
");
            }
            else if (theme == "style3")
            {
                retval = string.Format($@"
<table>
    <tr>
        <td align=""center"" style=""{goodDisplay}"">
            <a class=""diggLink"" href=""javascript:;"" onclick=""{clickStringOfGood}"">{goodText}</a>
            <span class=""diggNum"">{goodNum}票</span>
        </td>
        <td align=""center"" style=""{badDisplay}"">
            <a class=""diggLink"" href=""javascript:;"" onclick=""{clickStringOfBad}"">{badText}</a>
            <span class=""diggNum"">{badNum}票</span>
        </td>
    </tr>
</table>
");
            }
            else if (theme == "style4")
            {
                retval = string.Format($@"
<div>
	<div class=""diggArea"" style=""{goodDisplay}"">
        <div class=""diggNum"">{goodNum}</div>
        <div class=""diggLink""><a href=""javascript:;"" onclick=""{clickStringOfGood}"">{goodText}</a></div>
    </div>
    <div class=""diggArea"" style=""{badDisplay}"">
        <div class=""diggNum"">{badNum}</div>
        <div class=""diggLink""><a href=""javascript:;"" onclick=""{clickStringOfBad}"">{badText}</a></div>
    </div>
</div>
");
            }

            HttpContext.Current.Response.Write(retval);
            HttpContext.Current.Response.End();
        }
    }
}
