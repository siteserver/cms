using System.Collections.Specialized;
using System.Text;
using SiteServer.Utils;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlEntity
{
    [StlElement(Title = "请求实体", Description = "通过 {request.} 实体在模板中显示地址栏请求参数")]
    public static class StlRequestEntities
	{
        internal static string Parse(string stlEntity, PageInfo pageInfo)
        {
            var parsedContent = string.Empty;
            try
            {
                pageInfo.AddPageBodyCodeIfNotExists(PageInfo.Const.Jquery);

                var entityName = StlParserUtility.GetNameFromEntity(stlEntity);
                var entityValue = StlParserUtility.GetValueFromEntity(stlEntity);
                var attributeName = entityName.Substring(9, entityName.Length - 10);

                var ajaxDivId = StlParserUtility.GetAjaxDivId(pageInfo.UniqueId);
                var functionName = $"stlRequest_{ajaxDivId}";
                parsedContent = $@"<span id=""{ajaxDivId}""></span>";

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
        if (r) $(""#{ajaxDivId}"").text(v);");

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
                    templateContent = StringUtils.ReplaceIgnoreCase(templateContent, $"{{Request.{key}}}", AttackUtils.FilterSqlAndXss(queryString[key]));
                }
            }
            return RegexUtils.Replace("{Request.[^}]+}", templateContent, string.Empty);
        }
	}
}
