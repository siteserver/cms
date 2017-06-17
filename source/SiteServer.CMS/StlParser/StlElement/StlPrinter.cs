using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Xml;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "打印", Description = "通过 stl:printer 标签在模板中实现打印功能")]
    public class StlPrinter
	{
        private StlPrinter() { }
        public const string ElementName = "stl:printer";

        public const string AttributeTitleId = "titleId";
        public const string AttributeBodyId = "bodyId";
        public const string AttributeLogoId = "logoId";
        public const string AttributeLocationId = "locationId";
        public const string AttributeIsDynamic = "isDynamic";

	    public static SortedList<string, string> AttributeList => new SortedList<string, string>
	    {
	        {AttributeTitleId, "页面HTML中打印标题的ID属性"},
	        {AttributeBodyId, "页面HTML中打印正文的ID属性"},
	        {AttributeLogoId, "页面LOGO的ID属性"},
	        {AttributeLocationId, "页面当前位置的ID属性"},
	        {AttributeIsDynamic, "是否动态显示"}
	    };

        //对“打印”（stl:printer）元素进行解析
        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
		{
			string parsedContent;
			try
			{
                var titleId = string.Empty;
                var bodyId = string.Empty;
                var logoId = string.Empty;
                var locationId = string.Empty;
                var isDynamic = false;
                var stlAnchor = new HtmlAnchor();

                var ie = node.Attributes?.GetEnumerator();
			    if (ie != null)
			    {
                    while (ie.MoveNext())
                    {
                        var attr = (XmlAttribute)ie.Current;

                        if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeTitleId))
                        {
                            titleId = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeBodyId))
                        {
                            bodyId = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeLogoId))
                        {
                            logoId = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeLocationId))
                        {
                            locationId = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsDynamic))
                        {
                            isDynamic = TranslateUtils.ToBool(attr.Value);
                        }
                        else
                        {
                            ControlUtils.AddAttributeIfNotExists(stlAnchor, attr.Name, attr.Value);
                        }
                    }
                }

                parsedContent = isDynamic ? StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo) : ParseImpl(node, pageInfo, contextInfo, stlAnchor, titleId, bodyId, logoId, locationId);
			}
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

			return parsedContent;
		}

        private static string ParseImpl(XmlNode node, PageInfo pageInfo, ContextInfo contextInfo, HtmlAnchor stlAnchor, string titleId, string bodyId, string logoId, string locationId)
        {
            var jsUrl = SiteFilesAssets.GetUrl(pageInfo.ApiUrl, pageInfo.TemplateInfo.Charset == ECharset.gb2312 ? SiteFilesAssets.Print.JsGb2312 : SiteFilesAssets.Print.JsUtf8);

            var iconUrl = SiteFilesAssets.GetUrl(pageInfo.ApiUrl, SiteFilesAssets.Print.IconUrl);
            pageInfo.AddPageScriptsIfNotExists(PageInfo.JsAfStlPrinter, $@"
<script language=""JavaScript"" type=""text/javascript"">
function stlLoadPrintJsCallBack()
{{
    if(typeof forSPrint == ""object"" && forSPrint.Print)
    {{
        forSPrint.data.titleId = ""{titleId}"";
        forSPrint.data.artiBodyId = ""{bodyId}"";
        forSPrint.data.pageLogoId = ""{logoId}"";
        forSPrint.data.pageWayId = ""{locationId}"";
        forSPrint.data.iconUrl = ""{iconUrl}"";
        forSPrint.Print();
    }}
}}

function stlPrintGetBrowser()
{{
    if (navigator.userAgent.indexOf(""MSIE"") != -1)
    {{
        return 1; 
    }}
    else if (navigator.userAgent.indexOf(""Firefox"") != -1)
    {{
        return 2; 
    }}
    else if (navigator.userAgent.indexOf(""Navigator"") != -1)
    {{
        return 3;
    }}
    else if (navigator.userAgent.indexOf(""Opera"") != -1 )
    {{
        return 4;
    }}
    else
    {{
        return 5;
    }}
}}

function stlLoadPrintJs()
{{
    var myBrowser = stlPrintGetBrowser();
    if(myBrowser == 1)
    {{
        var js_url = ""{jsUrl}"";
        var js = document.createElement( ""script"" ); 
        js.setAttribute( ""type"", ""text/javascript"" );
        js.setAttribute( ""src"", js_url);
        js.setAttribute( ""id"", ""printJsUrl"");
        document.body.insertBefore( js, null);
        document.getElementById(""printJsUrl"").onreadystatechange = stlLoadPrintJsCallBack;
    }}
    else
    {{
        var js_url = ""{jsUrl}"";
        var js = document.createElement( ""script"" ); 
        js.setAttribute( ""type"", ""text/javascript"" );
        js.setAttribute( ""src"", js_url);
        js.setAttribute( ""id"", ""printJsUrl"");
        js.setAttribute( ""onload"", ""stlLoadPrintJsCallBack()"");
        document.body.insertBefore( js, null);					
    }}
}}	
</script>
");

            if (node.InnerXml.Trim().Length == 0)
            {
                stlAnchor.InnerHtml = "打印";
            }
            else
            {
                var innerBuilder = new StringBuilder(node.InnerXml);
                StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                stlAnchor.InnerHtml = innerBuilder.ToString();
            }
            stlAnchor.Attributes["href"] = "javascript:stlLoadPrintJs();";

            var parsedContent = ControlUtils.GetControlRenderHtml(stlAnchor);

            return parsedContent;
        }
	}
}
