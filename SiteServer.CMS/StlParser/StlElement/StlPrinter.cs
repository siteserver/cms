using System.Text;
using System.Web.UI.HtmlControls;
using SiteServer.Utils;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlElement(Title = "打印", Description = "通过 stl:printer 标签在模板中实现打印功能")]
    public class StlPrinter
	{
        private StlPrinter() { }
        public const string ElementName = "stl:printer";

        [StlAttribute(Title = "页面Html 中打印标题的 Id 属性")]
        private const string TitleId = nameof(TitleId);

        [StlAttribute(Title = "页面Html 中打印正文的 Id 属性")]
        private const string BodyId = nameof(BodyId);

        [StlAttribute(Title = "页面Logo 的 Id 属性")]
        private const string LogoId = nameof(LogoId);

        [StlAttribute(Title = "页面当前位置的 Id 属性")]
        private const string LocationId = nameof(LocationId);

        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
		{
		    var titleId = string.Empty;
            var bodyId = string.Empty;
            var logoId = string.Empty;
            var locationId = string.Empty;
            var stlAnchor = new HtmlAnchor();

            foreach (var name in contextInfo.Attributes.AllKeys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, TitleId))
                {
                    titleId = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, BodyId))
                {
                    bodyId = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, LogoId))
                {
                    logoId = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, LocationId))
                {
                    locationId = value;
                }
                else
                {
                    ControlUtils.AddAttributeIfNotExists(stlAnchor, name, value);
                }
            }

            return ParseImpl(pageInfo, contextInfo, stlAnchor, titleId, bodyId, logoId, locationId);
		}

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, HtmlAnchor stlAnchor, string titleId, string bodyId, string logoId, string locationId)
        {
            var jsUrl = SiteFilesAssets.GetUrl(pageInfo.ApiUrl, pageInfo.TemplateInfo.Charset == ECharset.gb2312 ? SiteFilesAssets.Print.JsGb2312 : SiteFilesAssets.Print.JsUtf8);

            var iconUrl = SiteFilesAssets.GetUrl(pageInfo.ApiUrl, SiteFilesAssets.Print.IconUrl);
            if (!pageInfo.BodyCodes.ContainsKey(PageInfo.Const.JsAfStlPrinter))
            {
                pageInfo.BodyCodes.Add(PageInfo.Const.JsAfStlPrinter, $@"
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
            }

            if (string.IsNullOrEmpty(contextInfo.InnerHtml))
            {
                stlAnchor.InnerHtml = "打印";
            }
            else
            {
                var innerBuilder = new StringBuilder(contextInfo.InnerHtml);
                StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                stlAnchor.InnerHtml = innerBuilder.ToString();
            }
            stlAnchor.Attributes["href"] = "javascript:stlLoadPrintJs();";

            var parsedContent = ControlUtils.GetControlRenderHtml(stlAnchor);

            return parsedContent;
        }
	}
}
