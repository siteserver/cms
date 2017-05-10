using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "掘客", Description = "通过 stl:digg 标签在模板中实现赞同/不赞同、投个鲜花/扔个鸡蛋、顶一下/踩一下等功能")]
    public class StlDigg
	{
        private StlDigg() { }
        public const string ElementName = "stl:digg";

        public const string AttributeType = "type";
        public const string AttributeGoodText = "goodText";
        public const string AttributeBadText = "badText";
        public const string AttributeTheme = "theme";
        public const string AttributeIsNumber = "isNumber";
        public const string AttributeIsDynamic = "isDynamic";

	    public static SortedList<string, string> AttributeList => new SortedList<string, string>
        {
	        {AttributeType, StringUtils.SortedListToAttributeValueString("类型", EDiggTypeUtils.TypeList)},
	        {AttributeGoodText, "赞同文字"},
	        {AttributeBadText, "不赞同文字"},
	        {AttributeTheme, "主题样式"},
	        {AttributeIsNumber, "仅显示结果数字"},
	        {AttributeIsDynamic, "是否动态显示"}
	    };

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
		{
			string parsedContent;
			try
			{
                var diggType = EDiggType.All;
                var goodText = "顶一下";
                var badText = "踩一下";
                var theme = "style1";
                var isNumber = false;
                var isDynamic = false;

                var ie = node.Attributes?.GetEnumerator();
			    if (ie != null)
			    {
                    while (ie.MoveNext())
                    {
                        var attr = (XmlAttribute)ie.Current;

                        if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeType))
                        {
                            diggType = EDiggTypeUtils.GetEnumType(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeGoodText))
                        {
                            goodText = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeBadText))
                        {
                            badText = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeTheme))
                        {
                            theme = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsNumber))
                        {
                            isNumber = TranslateUtils.ToBool(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsDynamic))
                        {
                            isDynamic = TranslateUtils.ToBool(attr.Value);
                        }
                    }
                }

                pageInfo.AddPageScriptsIfNotExists(PageInfo.Components.Jquery);

                parsedContent = isDynamic ? StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo) : ParseImpl(pageInfo, contextInfo, diggType, goodText, badText, theme, isNumber);
			}
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

			return parsedContent;
		}

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, EDiggType diggType, string goodText, string badText, string theme, bool isNumber)
        {
            if (isNumber)
            {
                int count;

                var relatedIdentity = contextInfo.ContentId;
                if (relatedIdentity == 0 || contextInfo.ContextType == EContextType.Channel)
                {
                    relatedIdentity = contextInfo.ChannelId;
                }

                var counts = BaiRongDataProvider.DiggDao.GetCount(pageInfo.PublishmentSystemId, relatedIdentity);
                var goodNum = counts[0];
                var badNum = counts[1];

                if (diggType == EDiggType.Good)
                {
                    count = goodNum;
                }
                else if (diggType == EDiggType.Bad)
                {
                    count = badNum;
                }
                else
                {
                    count = goodNum + badNum;
                }

                return count.ToString();
            }
            else
            {
                var updaterId = pageInfo.UniqueId;
                var ajaxDivId = StlParserUtility.GetAjaxDivId(updaterId);

                pageInfo.AddPageScriptsIfNotExists(ElementName,
                    $@"<script language=""javascript"" src=""{SiteFilesAssets.Digg.GetScriptUrl(pageInfo.ApiUrl)}""></script>");

                var builder = new StringBuilder();
                builder.Append(
                    $@"<link rel=""stylesheet"" href=""{SiteFilesAssets.Digg.GetStyleUrl(pageInfo.ApiUrl, theme)}"" type=""text/css"" />");
                builder.Append($@"<div id=""{ajaxDivId}"">");

                var relatedIdentity = contextInfo.ContentId;
                if (relatedIdentity == 0 || contextInfo.ContextType == EContextType.Channel)
                {
                    relatedIdentity = contextInfo.ChannelId;
                }

                var innerPageUrl = Digg.GetUrl(pageInfo.ApiUrl, pageInfo.PublishmentSystemId, relatedIdentity, updaterId, diggType, goodText, badText, theme, false, false);
                var innerPageUrlWithGood = Digg.GetUrl(pageInfo.ApiUrl, pageInfo.PublishmentSystemId, relatedIdentity, updaterId, diggType, goodText, badText, theme, true, true);
                var innerPageUrlWithBad = Digg.GetUrl(pageInfo.ApiUrl, pageInfo.PublishmentSystemId, relatedIdentity, updaterId, diggType, goodText, badText, theme, true, false);

                string loadingHtml =
                    $@"<img src=""{SiteFilesAssets.GetUrl(pageInfo.ApiUrl, SiteFilesAssets.FileLoading)}"" />";

                builder.Append(loadingHtml);

                builder.Append("</div>");

                builder.Append($@"
<script type=""text/javascript"" language=""javascript"">
function stlDigg_{updaterId}(url)
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

stlDigg_{updaterId}('{innerPageUrl}');

function stlDiggSet_{updaterId}(isGood)
{{
    if (stlDiggCheck({pageInfo.PublishmentSystemId}, {relatedIdentity})){{
        jQuery('#{ajaxDivId}').html('{loadingHtml}');
        if (isGood)
        {{
            stlDigg_{updaterId}('{innerPageUrlWithGood}');
        }}else{{
            stlDigg_{updaterId}('{innerPageUrlWithBad}');
        }}
    }}
}}
</script>
");

                return builder.ToString();
            }
        }
	}
}
