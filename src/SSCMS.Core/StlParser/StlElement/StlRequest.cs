using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SSCMS.Core.StlParser.Attributes;
using SSCMS.Core.Utils;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "URL地址请求", Description = "通过 stl:request 实体在模板中显示地址栏请求参数")]
    public static class StlRequest
    {
        public const string ElementName = "stl:request";

        [StlAttribute(Title = "地址参数名称")]
        private const string Type = nameof(Type);

        [StlAttribute(Title = "指定需要显示地址参数值的HTML元素Id")]
        private const string ElementId = nameof(ElementId);

        [StlAttribute(Title = "回调函数名称")]
        private const string Callback = nameof(Callback);

        public static async Task<object> ParseAsync(IParseManager parseManager)
        {
            var type = string.Empty;
            var elementId = string.Empty;
            var callback = string.Empty;
            
            var attributes = new NameValueCollection();

            foreach (var name in parseManager.ContextInfo.Attributes.AllKeys)
            {
                var value = parseManager.ContextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, Type))
                {
                    type = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, ElementId))
                {
                    elementId = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Callback))
                {
                    callback = value;
                }
                else
                {
                    attributes[name] = value;
                }
            }

            return await ParseAsync(parseManager, type, elementId, callback, attributes);
        }

        private static async Task<string> ParseAsync(IParseManager parseManager, string type, string elementId, string callback, NameValueCollection attributes)
        {
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;

            var parsedContent = string.Empty;
            if (string.IsNullOrEmpty(elementId))
            {
                elementId = StringUtils.GetElementId();
                attributes["id"] = elementId;

                var html = string.Empty;
                if (!string.IsNullOrEmpty(contextInfo.InnerHtml))
                {
                    var innerBuilder = new StringBuilder(contextInfo.InnerHtml);
                    await parseManager.ParseInnerContentAsync(innerBuilder);
                    html = innerBuilder.ToString();
                }

                parsedContent = $@"<span {TranslateUtils.ToAttributesString(attributes)}>{html}</span>";
            }

            var builder = new StringBuilder();
            builder.Append($@"
<script type=""text/javascript"" language=""javascript"">
$(document).ready(function(){{
    try
    {{
        var queryString = document.location.search;
        if (queryString == null || queryString.length <= 1) return;
        var reg = new RegExp(""(^|&){type}=([^&]*)(&|$)""); 
        var r = queryString.substring(1).match(reg);
        var v = decodeURI(decodeURI(r[2]));
        if (r) $(""#{elementId}"").text(v);");

            if (!string.IsNullOrEmpty(callback))
            {
                builder.Append($@"{callback}(v);");
            }

            builder.Append(@"
    }catch(e){}
});
</script>
");

            if (!pageInfo.FootCodes.ContainsKey($"{ElementName}_{elementId}"))
            {
                pageInfo.FootCodes.Add($"{ElementName}_{elementId}", builder.ToString());
            }

            return parsedContent;
        }

        public static string ParseRequestEntities(NameValueCollection queryString, string templateContent)
        {
            if (queryString != null && queryString.Count > 0)
            {
                foreach (string key in queryString.Keys)
                {
                    var value = queryString[key];
                    value = WebUtility.UrlDecode(value);
                    value = AttackUtils.FilterSqlAndXss(value);

                    templateContent = StringUtils.ReplaceIgnoreCase(templateContent, $"{{Request.{key}}}", value);
                }
            }
            return RegexUtils.Replace("{Request.[^}]+}", templateContent, string.Empty);
        }
    }
}
