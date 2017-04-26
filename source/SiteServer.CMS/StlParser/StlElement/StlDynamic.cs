using System;
using System.Collections.Specialized;
using System.Text;
using System.Xml;
using BaiRong.Core;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    public class StlDynamic
    {
        private StlDynamic() { }
        public const string ElementName = "stl:dynamic";                    //显示动态内容

        public const string AttributeContext = "context";                   //所处上下文
        public const string AttributeIsPageRefresh = "ispagerefresh";       //翻页时是否刷新页面

        public static ListDictionary AttributeList => new ListDictionary
        {
            {AttributeContext, "所处上下文"},
            {AttributeIsPageRefresh, "翻页时是否刷新页面"}
        };

        internal static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfoRef)
        {
            string parsedContent;
            var contextInfo = contextInfoRef.Clone();
            try
            {
                var isPageRefresh = false;

                var ie = node.Attributes?.GetEnumerator();
                if (ie != null)
                {
                    while (ie.MoveNext())
                    {
                        var attr = (XmlAttribute)ie.Current;
                        var attributeName = attr.Name.ToLower();
                        if (attributeName.Equals(AttributeContext))
                        {
                            contextInfo.ContextType = EContextTypeUtils.GetEnumType(attr.Value);
                        }
                        else if (attributeName.Equals(AttributeIsPageRefresh))
                        {
                            isPageRefresh = TranslateUtils.ToBool(attr.Value);
                        }
                    }
                }

                parsedContent = ParseImpl(pageInfo, contextInfo, node.InnerXml, isPageRefresh);
            }
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

            return parsedContent;
        }

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, string templateContent, bool isPageRefresh)
        {
            pageInfo.AddPageScriptsIfNotExists(PageInfo.Components.StlClient);

            var ajaxDivId = StlParserUtility.GetAjaxDivId(pageInfo.UniqueId);

            var functionName = $"stlDynamic_{ajaxDivId}";

            if (string.IsNullOrEmpty(templateContent))
            {
                return string.Empty;
            }

            var apiUrl = ActionsDynamic.GetUrl(pageInfo.ApiUrl);
            var currentPageUrl = StlUtility.GetStlCurrentUrl(pageInfo, contextInfo.ChannelID, contextInfo.ContentID, contextInfo.ContentInfo);
            currentPageUrl = PageUtils.AddQuestionOrAndToUrl(currentPageUrl);
            var apiParameters = ActionsDynamic.GetParameters(pageInfo.PublishmentSystemId, contextInfo.ChannelID, contextInfo.ContentID, pageInfo.TemplateInfo.TemplateId, currentPageUrl, ajaxDivId, isPageRefresh, templateContent);

            var builder = new StringBuilder();
            builder.Append($@"<span id=""{ajaxDivId}""></span>");

            builder.Append($@"
<script type=""text/javascript"" language=""javascript"">
function {functionName}(pageNum)
{{
    var url = ""{apiUrl}?"" + StlClient.getQueryString();
    var data = {apiParameters};
    if (pageNum && pageNum > 0)
    {{
        data.pageNum = pageNum;
    }}

    stlClient.post(url, data, function (err, data, status) {{
        if (!err) document.getElementById(""{ajaxDivId}"").innerHTML = data.html;
    }});
}}
{functionName}(0);
</script>
");

            return builder.ToString();
        }

        internal static string ParseDynamicElement(string stlElement, PageInfo pageInfo, ContextInfo contextInfo)
        {
            stlElement = StringUtils.ReplaceIgnoreCase(stlElement, "isdynamic=\"true\"", string.Empty);
            return ParseImpl(pageInfo, contextInfo, stlElement, false);
        }
    }
}
