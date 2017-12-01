using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using BaiRong.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlEntity
{
    [Stl(Usage = "请求实体", Description = "通过 {request.} 实体在模板中显示地址栏请求参数")]
    public class StlRequestEntities
	{
        private StlRequestEntities()
		{
		}

	    public const string EntityName = "request";

        public static SortedList<string, string> AttributeList => null;

        internal static string Parse(string stlEntity, PageInfo pageInfo, ContextInfo contextInfo)
        {
            var parsedContent = string.Empty;
            try
            {
                var entityName = StlParserUtility.GetNameFromEntity(stlEntity);
                var entityValue = StlParserUtility.GetValueFromEntity(stlEntity);
                var attributeName = entityName.Substring(9, entityName.Length - 10);

                var ajaxDivId = StlParserUtility.GetAjaxDivId(pageInfo.UniqueId);
                string functionName = $"stlRequest_{ajaxDivId}";
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
        if (r) $(""#{ajaxDivId}"").html(v);");

                if (!string.IsNullOrEmpty(entityValue))
                {
                    builder.Append($@"
         if (r) $(""#{entityValue}"").val(v);");
                }

                builder.Append(@"
    }}catch(e){{}}
}});
</script>
");

                pageInfo.AddPageEndScriptsIfNotExists(functionName, builder.ToString());
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
                    templateContent = StringUtils.ReplaceIgnoreCase(templateContent, $"{{Request.{key}}}", queryString[key]);
                }
            }
            return RegexUtils.Replace("{Request.[^}]+}", templateContent, string.Empty);
        }
	}
}
