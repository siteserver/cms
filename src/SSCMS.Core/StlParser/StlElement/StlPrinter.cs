using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using SSCMS.Core.StlParser.Attributes;
using SSCMS.Parse;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "打印", Description = "通过 stl:printer 标签在模板中实现打印功能")]
    public static class StlPrinter
	{
        public const string ElementName = "stl:printer";

        [StlAttribute(Title = "页面Html 中打印标题的 Id 属性")]
        private const string TitleId = nameof(TitleId);

        [StlAttribute(Title = "页面Html 中打印正文的 Id 属性")]
        private const string BodyId = nameof(BodyId);

        [StlAttribute(Title = "页面Logo 的 Id 属性")]
        private const string LogoId = nameof(LogoId);

        [StlAttribute(Title = "页面当前位置的 Id 属性")]
        private const string LocationId = nameof(LocationId);

        public static async Task<object> ParseAsync(IParseManager parseManager)
		{
		    var titleId = string.Empty;
            var bodyId = string.Empty;
            var logoId = string.Empty;
            var locationId = string.Empty;
            var attributes = new NameValueCollection();

            foreach (var name in parseManager.ContextInfo.Attributes.AllKeys)
            {
                var value = parseManager.ContextInfo.Attributes[name];

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
                    TranslateUtils.AddAttributeIfNotExists(attributes, name, value);
                }
            }

            return await ParseAsync(parseManager, attributes, titleId, bodyId, logoId, locationId);
		}

        private static async Task<string> ParseAsync(IParseManager parseManager, NameValueCollection attributes, string titleId, string bodyId, string logoId, string locationId)
        {
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;

            var jsUrl = parseManager.PathManager.GetSiteFilesUrl(pageInfo.Site, Resources.Print.Js);
            var iconUrl = parseManager.PathManager.GetSiteFilesUrl(pageInfo.Site, Resources.Print.IconUrl);

            if (!pageInfo.BodyCodes.ContainsKey(ParsePage.Const.JsAfStlPrinter))
            {
                pageInfo.BodyCodes.Add(ParsePage.Const.JsAfStlPrinter, $@"
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

            string innerHtml;
            if (string.IsNullOrEmpty(contextInfo.InnerHtml))
            {
                innerHtml = "打印";
            }
            else
            {
                var innerBuilder = new StringBuilder(contextInfo.InnerHtml);
                await parseManager.ParseInnerContentAsync(innerBuilder);
                innerHtml = innerBuilder.ToString();
            }


            attributes["href"] = "javascript:stlLoadPrintJs();";

            return $@"<a {TranslateUtils.ToAttributesString(attributes)}>{innerHtml}</a>";
        }
	}
}
