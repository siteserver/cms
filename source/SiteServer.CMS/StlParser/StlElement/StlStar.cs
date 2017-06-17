using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;
using BaiRong.Core;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "评分", Description = "通过 stl:star 标签在模板中显示评分功能")]
    public class StlStar
	{
        private StlStar() { }
        public const string ElementName = "stl:star";

        public const string AttributeTotalStar = "totalStar";
        public const string AttributeInitStar = "initStar";
        public const string AttributeSuccessMessage = "successMessage";
        public const string AttributeFailureMessage = "failureMessage";
        public const string AttributeTheme = "theme";
        public const string AttributeIsTextOnly = "isTextOnly";
        public const string AttributeIsDynamic = "isDynamic";

	    public static SortedList<string, string> AttributeList => new SortedList<string, string>
	    {
	        {AttributeTotalStar, "最高评分"},
	        {AttributeInitStar, "初始评分"},
	        {AttributeSuccessMessage, "评分成功提示信息"},
	        {AttributeFailureMessage, "评分失败提示信息"},
	        {AttributeTheme, "主题样式"},
	        {AttributeIsTextOnly, "仅显示评分数"},
	        {AttributeIsDynamic, "是否动态显示"}
	    };

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
		{
			string parsedContent;
			try
			{
                var totalStar = 10;
                var initStar = 0;
                var successMessage = string.Empty;
                var failureMessage = "对不起，不能重复操作!";
                var theme = "style1";
                var isTextOnly = false;
                var isDynamic = false;

                var ie = node.Attributes?.GetEnumerator();
			    if (ie != null)
			    {
			        while (ie.MoveNext())
			        {
			            var attr = (XmlAttribute) ie.Current;

			            if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeTotalStar))
			            {
			                totalStar = TranslateUtils.ToInt(attr.Value, totalStar);
			            }
			            else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeInitStar))
			            {
			                initStar = TranslateUtils.ToInt(attr.Value, 0);
			            }
			            else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeSuccessMessage))
			            {
			                successMessage = attr.Value;
			            }
			            else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeFailureMessage))
			            {
			                failureMessage = attr.Value;
			            }
			            else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeTheme))
			            {
			                theme = attr.Value;
			            }
			            else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsTextOnly))
			            {
			                isTextOnly = TranslateUtils.ToBool(attr.Value);
			            }
			            else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsDynamic))
			            {
			                isDynamic = TranslateUtils.ToBool(attr.Value);
			            }
			        }
			    }

			    pageInfo.AddPageScriptsIfNotExists(PageInfo.Components.Jquery);

                parsedContent = isDynamic ? StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo) : ParseImpl(pageInfo, contextInfo, totalStar, initStar, successMessage, failureMessage, theme, isTextOnly);
			}
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

			return parsedContent;
		}

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, int totalStar, int initStar, string successMessage, string failureMessage, string theme, bool isTextOnly)
        {
            var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, contextInfo.ChannelId);
            var tableStyle = NodeManager.GetTableStyle(pageInfo.PublishmentSystemInfo, contextInfo.ChannelId);
            var contentId = ContentUtility.GetRealContentId(tableStyle, tableName, contextInfo.ContentId);
            var channelId = BaiRongDataProvider.ContentDao.GetNodeId(tableName, contextInfo.ContentId);

            if (isTextOnly)
            {
                var counts = DataProvider.StarDao.GetCount(pageInfo.PublishmentSystemId, channelId, contentId);
                var totalCount = counts[0];
                var totalPoint = counts[1];

                var totalCountAndPointAverage = DataProvider.StarSettingDao.GetTotalCountAndPointAverage(pageInfo.PublishmentSystemId, contentId);
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
                    return numString + ".0";
                }
                var first = numString.Substring(0, numString.IndexOf('.'));
                var second = numString.Substring(numString.IndexOf('.') + 1, 1);
                return first + "." + second;
            }
            else
            {
                var updaterId = pageInfo.UniqueId;
                var ajaxDivId = StlParserUtility.GetAjaxDivId(updaterId);

                pageInfo.AddPageScriptsIfNotExists(ElementName,
                    $@"<script language=""javascript"" src=""{SiteFilesAssets.Star.GetScriptUrl(pageInfo.ApiUrl)}""></script>");

                var builder = new StringBuilder();
                builder.Append(
                    $@"<link rel=""stylesheet"" href=""{SiteFilesAssets.Star.GetStyleUrl(pageInfo.ApiUrl, theme)}"" type=""text/css"" />");
                builder.Append($@"<div id=""{ajaxDivId}"">");

                var innerPageUrl = Star.GetUrl(pageInfo.ApiUrl, pageInfo.PublishmentSystemId, channelId, contentId, updaterId, totalStar, initStar, theme, false);
                var innerPageUrlWithAction = Star.GetUrl(pageInfo.ApiUrl, pageInfo.PublishmentSystemId, channelId, contentId, updaterId, totalStar, initStar, theme, true);

                string loadingHtml =
                    $@"<img src=""{SiteFilesAssets.GetUrl(pageInfo.ApiUrl, SiteFilesAssets.FileLoading)}"" />";

                builder.Append(loadingHtml);

                builder.Append("</div>");

                var successAlert = string.Empty;
                if (!string.IsNullOrEmpty(successMessage))
                {
                    successAlert = $"stlSuccessAlert('{successMessage}');";
                }

                builder.Append($@"
<script type=""text/javascript"" language=""javascript"">
function stlStar_{updaterId}(url)
{{
    try
    {{
        var cnum=Math.ceil(Math.random()*1000);
        url = url + '&r=' + cnum;

        jQuery.get(url, '', function(data, textStatus){{
            jQuery('#{ajaxDivId}').html(data);
        }});

    }}catch(e){{}}
}}

stlStar_{updaterId}('{innerPageUrl}');

function stlStarPoint_{updaterId}(point)
{{
    if (stlStarCheck({pageInfo.PublishmentSystemId}, {channelId}, {contentId}, '{failureMessage}')){{
        jQuery('#{ajaxDivId}').innerHTML = '{loadingHtml}';
        stlStar_{updaterId}('{innerPageUrlWithAction}' + point);
        {successAlert}
    }}
}}
</script>
");

                return builder.ToString();
            }
        }
	}
}
