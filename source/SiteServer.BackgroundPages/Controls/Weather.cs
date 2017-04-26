using System;
using System.Web.UI;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Net;

namespace SiteServer.BackgroundPages.Controls
{
	public class Weather : LiteralControl 
	{
		public virtual string City
		{
			get 
			{
				var state = ViewState["City"];
				if ( state != null ) 
				{
					return (string)state;
				}
				return "北京";
			}
			set 
			{
				ViewState["City"] = value;
			}
		}

		public virtual string Provider
		{
			get 
			{
				var provider = yahoo;
				var state = ViewState["Provider"];
				if ( state != null ) 
				{
					provider = ((string)state).Trim().ToLower();
					if (provider != yahoo && provider != sina)
					{
						provider = yahoo;
					}
				}
				return provider;
			}
			set 
			{
				ViewState["Provider"] = value;
			}
		}

		private const string sina = "sina";
		private const string yahoo = "yahoo";

		protected override void Render(HtmlTextWriter writer)
		{
			var weatherHtml = GetWeatherHtml(Provider);
			if (string.IsNullOrEmpty(weatherHtml))
			{
				var otherProvider = (Provider == yahoo) ? sina : yahoo;
				weatherHtml = GetWeatherHtml(otherProvider);
			}
			writer.Write(weatherHtml);
		}

		protected string GetWeatherHtml(string provider)
		{
			var result = string.Empty;

			try
			{
				var group = "title";
				if (provider == yahoo)
				{
					var pinyinCity = TranslateUtils.ToPinYin(City);
					string weatherUrl = $"http://weather.cn.yahoo.com/weather.html?city={pinyinCity}&s=1";
					string regex =
					    $@"<\!--today\s+-->\s+<div\s+class=""dt_c"">\s+<div\s+class=""tn"">{DateTime.Now.Year}-{TranslateUtils
					        .ToTwoCharString(DateTime.Now.Month)}-{TranslateUtils.ToTwoCharString(DateTime.Now.Day)}</div>\s*(?<title>[\s\S]+?)\s*</div>\s+<\!--//today\s+-->";

                    var content = WebClientUtils.GetRemoteFileSource(weatherUrl, ECharset.utf_8);

					if (!string.IsNullOrEmpty(content))
					{
						content = RegexUtils.GetContent(group, regex, content);

						var flashRegex = @"<p>\s*(?<title>[\s\S]+?)\s*</p>";
						var weatherStringRegex = @"<span\s+class=""ft1"">\s*(?<title>[\s\S]+?)\s*</span>";
						var highDegreeRegex = @"<span\s+class=""hitp"">\s*(?<title>[\s\S]+?)\s*</span>";
						var lowDegreeRegex = @"<span\s+class=""lotp"">\s*(?<title>[\s\S]+?)\s*</span>";

						var flashHtml = RegexUtils.GetContent(group, flashRegex, content);
						flashHtml = flashHtml.Replace("width=\"64\" height=\"64\"", "width=\"20\" height=\"20\"");
						var weatherString = RegexUtils.GetContent(group, weatherStringRegex, content);
						var highDegree = RegexUtils.GetContent(group, highDegreeRegex, content);
						var lowDegree = RegexUtils.GetContent(group, lowDegreeRegex, content);

						if (!string.IsNullOrEmpty(highDegree))
						{
							result = flashHtml + weatherString + $"&nbsp;{highDegree} ~ {lowDegree}";
						}
					}
				}
				else if (provider == sina)
				{
					string weatherUrl = $"http://php.weather.sina.com.cn/search.php?city={PageUtils.UrlEncode(City)}";
					var regex = "<td\\s+bgcolor=\\#FEFEFF\\s+height=25\\s+style=\"padding-left:20px;\"\\s+align=center>24小时</td>\\s*(?<title>[\\s\\S]+?)\\s*</tr>";

                    var content = WebClientUtils.GetRemoteFileSource(weatherUrl, ECharset.gb2312);

					if (!string.IsNullOrEmpty(content))
					{
						content = RegexUtils.GetContent(group, regex, content);

						var contentArrayList = RegexUtils.GetTagInnerContents("td", content);

						var weatherString = (string)contentArrayList[0];
						var highDegree = (string)contentArrayList[1];
						var lowDegree = (string)contentArrayList[2];

						if (!string.IsNullOrEmpty(highDegree))
						{
							result = weatherString + $"&nbsp;{highDegree} ~ {lowDegree}";
						}
					}
				}
			}
			catch{}
			
			return result;
		}
	}
}
