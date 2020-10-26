using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SSCMS.Parse;
using SSCMS.Core.StlParser.Model;
using SSCMS.Core.StlParser.Utility;
using SSCMS.Core.Utils;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.StlEntity
{
    [StlElement(Title = "请求实体", Description = "通过 {request.} 实体在模板中显示地址栏请求参数")]
    public static class StlRequestEntities
	{
        internal static async Task<string> ParseAsync(string stlEntity, IParseManager parseManager)
        {
            var pageInfo = parseManager.PageInfo;

            var parsedContent = string.Empty;
            try
            {
                await pageInfo.AddPageHeadCodeIfNotExistsAsync(ParsePage.Const.Jquery);

                var entityName = StlParserUtility.GetNameFromEntity(stlEntity);
                var entityValue = StlParserUtility.GetValueFromEntity(stlEntity);
                var attributeName = entityName.Substring(9, entityName.Length - 10);

                var elementId = StringUtils.GetElementId();
                var functionName = $"stlRequest_{elementId}";
                parsedContent = $@"<span id=""{elementId}""></span>";

                var builder = new StringBuilder();
                builder.Append($@"
<script type=""text/javascript"" language=""javascript"">
$(function(){{
    try
    {{
        var queryString = document.location.search;
        if (queryString == null || queryString.length <= 1) return;
        var reg = new RegExp(""(^|&){attributeName}=([^&]*)(&|$)""); 
        var r = queryString.substring(1).match(reg);
        var v = decodeURI(decodeURI(r[2]));
        if (r) $(""#{elementId}"").text(v);");

                if (!string.IsNullOrEmpty(entityValue))
                {
                    builder.Append($@"
         if (r) $(""#{entityValue}"").val(v);");
                }

                builder.Append(@"
    }catch(e){}
});
</script>
");

                if (!pageInfo.FootCodes.ContainsKey(functionName))
                {
                    pageInfo.FootCodes.Add(functionName, builder.ToString());
                }
            }
            catch
            {
                // ignored
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
