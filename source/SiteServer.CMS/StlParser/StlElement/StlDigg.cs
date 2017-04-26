using System;
using System.Collections.Specialized;
using System.Text;
using System.Xml;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
	public class StlDigg
	{
        private StlDigg() { }
        public const string ElementName = "stl:digg";

        public const string AttributeType = "type";				        //类型
        public const string AttributeGoodText = "goodtext";				//赞同文字
        public const string AttributeBadText = "badtext";				    //不赞同文字
        public const string AttributeTheme = "theme";			            //主题样式
        public const string AttributeIsNumber = "isnumber";                //仅显示结果数字
        public const string AttributeIsDynamic = "isdynamic";              //是否动态显示

		public static ListDictionary AttributeList
		{
			get
			{
			    var attributes = new ListDictionary
			    {
			        {AttributeType, "类型"},
			        {AttributeGoodText, "赞同文字"},
			        {AttributeBadText, "不赞同文字"},
			        {AttributeTheme, "主题样式"},
			        {AttributeIsNumber, "仅显示结果数字"},
			        {AttributeIsDynamic, "是否动态显示"}
			    };
			    return attributes;
			}
		}

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
                        var attributeName = attr.Name.ToLower();
                        if (attributeName.Equals(AttributeType))
                        {
                            diggType = EDiggTypeUtils.GetEnumType(attr.Value);
                        }
                        else if (attributeName.Equals(AttributeGoodText))
                        {
                            goodText = attr.Value;
                        }
                        else if (attributeName.Equals(AttributeBadText))
                        {
                            badText = attr.Value;
                        }
                        else if (attributeName.Equals(AttributeTheme))
                        {
                            theme = attr.Value;
                        }
                        else if (attributeName.Equals(AttributeIsNumber))
                        {
                            isNumber = TranslateUtils.ToBool(attr.Value);
                        }
                        else if (attributeName.Equals(AttributeIsDynamic))
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

                var relatedIdentity = contextInfo.ContentID;
                if (relatedIdentity == 0 || contextInfo.ContextType == EContextType.Channel)
                {
                    relatedIdentity = contextInfo.ChannelID;
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

                var relatedIdentity = contextInfo.ContentID;
                if (relatedIdentity == 0 || contextInfo.ContextType == EContextType.Channel)
                {
                    relatedIdentity = contextInfo.ChannelID;
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
