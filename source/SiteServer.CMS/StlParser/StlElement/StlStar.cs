using System;
using System.Collections.Specialized;
using System.Text;
using System.Xml;
using BaiRong.Core;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
	public class StlStar
	{
        private StlStar() { }
        public const string ElementName = "stl:star";

        public const string Attribute_TotalStar = "totalstar";              //最高评分
        public const string Attribute_InitStar = "initstar";                //初始评分
        public const string Attribute_SuccessMessage = "successmessage";    //评分成功提示信息
        public const string Attribute_FailureMessage = "failuremessage";    //评分失败提示信息
        public const string Attribute_Theme = "theme";			            //主题样式
        public const string Attribute_IsTextOnly = "istextonly";            //仅显示评分数
        public const string Attribute_IsDynamic = "isdynamic";              //是否动态显示

		public static ListDictionary AttributeList
		{
			get
			{
                var attributes = new ListDictionary();
                attributes.Add(Attribute_TotalStar, "最高评分");
                attributes.Add(Attribute_InitStar, "初始评分");
                attributes.Add(Attribute_SuccessMessage, "评分成功提示信息");
                attributes.Add(Attribute_FailureMessage, "评分失败提示信息");
                attributes.Add(Attribute_Theme, "主题样式");
                attributes.Add(Attribute_IsTextOnly, "仅显示评分数");
                attributes.Add(Attribute_IsDynamic, "是否动态显示");
                return attributes;
			}
		}

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
		{
			var parsedContent = string.Empty;
			try
			{
                var totalStar = 10;
                var initStar = 0;
                var successMessage = string.Empty;
                var failureMessage = "对不起，不能重复操作!";
                var theme = "style1";
                var isTextOnly = false;
                var isDynamic = false;

                var ie = node.Attributes.GetEnumerator();

                while (ie.MoveNext())
                {
                    var attr = (XmlAttribute)ie.Current;
                    var attributeName = attr.Name.ToLower();
                    if (attributeName.Equals(Attribute_TotalStar))
                    {
                        totalStar = TranslateUtils.ToInt(attr.Value, totalStar);
                    }
                    else if (attributeName.Equals(Attribute_InitStar))
                    {
                        initStar = TranslateUtils.ToInt(attr.Value, 0);
                    }
                    else if (attributeName.Equals(Attribute_SuccessMessage))
                    {
                        successMessage = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_FailureMessage))
                    {
                        failureMessage = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_Theme))
                    {
                        theme = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_IsTextOnly))
                    {
                        isTextOnly = TranslateUtils.ToBool(attr.Value);
                    }
                    else if (attributeName.Equals(Attribute_IsDynamic))
                    {
                        isDynamic = TranslateUtils.ToBool(attr.Value);
                    }
                }

                pageInfo.AddPageScriptsIfNotExists(PageInfo.Components.Jquery);

                if (isDynamic)
                {
                    parsedContent = StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo);
                }
                else
                {
                    parsedContent = ParseImpl(pageInfo, contextInfo, totalStar, initStar, successMessage, failureMessage, theme, isTextOnly);
                }
			}
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

			return parsedContent;
		}

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, int totalStar, int initStar, string successMessage, string failureMessage, string theme, bool isTextOnly)
        {
            var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, contextInfo.ChannelID);
            var tableStyle = NodeManager.GetTableStyle(pageInfo.PublishmentSystemInfo, contextInfo.ChannelID);
            var contentID = ContentUtility.GetRealContentId(tableStyle, tableName, contextInfo.ContentID);
            var channelID = BaiRongDataProvider.ContentDao.GetNodeId(tableName, contextInfo.ContentID);

            if (isTextOnly)
            {
                var counts = DataProvider.StarDao.GetCount(pageInfo.PublishmentSystemId, channelID, contentID);
                var totalCount = counts[0];
                var totalPoint = counts[1];

                var totalCountAndPointAverage = DataProvider.StarSettingDao.GetTotalCountAndPointAverage(pageInfo.PublishmentSystemId, contentID);
                var settingTotalCount = (int)totalCountAndPointAverage[0];
                var settingPointAverage = (decimal)totalCountAndPointAverage[1];
                if (settingTotalCount > 0 || settingPointAverage > 0)
                {
                    totalCount += settingTotalCount;
                    totalPoint += Convert.ToInt32(settingPointAverage * settingTotalCount);
                }

                decimal num = 0;
                if (totalCount > 0)
                {
                    num = Convert.ToDecimal(totalPoint) / Convert.ToDecimal(totalCount);
                    initStar = 0;
                }
                else
                {
                    num = initStar;
                }

                if (num > totalStar)
                {
                    num = totalStar;
                }

                var numString = num.ToString();
                if (numString.IndexOf('.') == -1)
                {
                    return numString + ".0";
                }
                else
                {
                    var first = numString.Substring(0, numString.IndexOf('.'));
                    var second = numString.Substring(numString.IndexOf('.') + 1, 1);
                    return first + "." + second;
                }
            }
            else
            {
                var updaterID = pageInfo.UniqueId;
                var ajaxDivID = StlParserUtility.GetAjaxDivId(updaterID);

                pageInfo.AddPageScriptsIfNotExists(ElementName,
                    $@"<script language=""javascript"" src=""{SiteFilesAssets.Star.GetScriptUrl(pageInfo.ApiUrl)}""></script>");

                var builder = new StringBuilder();
                builder.Append(
                    $@"<link rel=""stylesheet"" href=""{SiteFilesAssets.Star.GetStyleUrl(pageInfo.ApiUrl, theme)}"" type=""text/css"" />");
                builder.Append($@"<div id=""{ajaxDivID}"">");

                var innerPageUrl = Star.GetUrl(pageInfo.ApiUrl, pageInfo.PublishmentSystemId, channelID, contentID, updaterID, totalStar, initStar, theme, false);
                var innerPageUrlWithAction = Star.GetUrl(pageInfo.ApiUrl, pageInfo.PublishmentSystemId, channelID, contentID, updaterID, totalStar, initStar, theme, true);

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
function stlStar_{updaterID}(url)
{{
    try
    {{
        var cnum=Math.ceil(Math.random()*1000);
        url = url + '&r=' + cnum;

        jQuery.get(url, '', function(data, textStatus){{
            jQuery('#{ajaxDivID}').html(data);
        }});

    }}catch(e){{}}
}}

stlStar_{updaterID}('{innerPageUrl}');

function stlStarPoint_{updaterID}(point)
{{
    if (stlStarCheck({pageInfo.PublishmentSystemId}, {channelID}, {contentID}, '{failureMessage}')){{
        jQuery('#{ajaxDivID}').innerHTML = '{loadingHtml}';
        stlStar_{updaterID}('{innerPageUrlWithAction}' + point);
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
