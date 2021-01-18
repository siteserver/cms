using System.Text;
using System.Threading.Tasks;
using SSCMS.Models;
using SSCMS.Parse;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.StlElement
{
    public static partial class StlDynamic
    {
        public static Dynamic GetDynamicInfo(ISettingsManager settingsManager, string value, int page, User user, string pathAndQuery)
        {
            var dynamicInfo = TranslateUtils.JsonDeserialize<Dynamic>(settingsManager.Decrypt(value));
            if (dynamicInfo.ChannelId == 0)
            {
                dynamicInfo.ChannelId = dynamicInfo.SiteId;
            }
            dynamicInfo.User = user;
            dynamicInfo.QueryString = PageUtils.GetQueryStringFilterSqlAndXss(pathAndQuery);
            dynamicInfo.QueryString.Remove("siteId");

            dynamicInfo.Page = page;

            return dynamicInfo;
        }

        public static async Task<string> GetScriptAsync(IParseManager parseManager, string dynamicApiUrl, Dynamic dynamicInfo)
        {
            if (string.IsNullOrEmpty(dynamicInfo.LoadingTemplate) &&
                string.IsNullOrEmpty(dynamicInfo.YesTemplate) &&
                string.IsNullOrEmpty(dynamicInfo.NoTemplate))
            {
                return string.Empty;
            }

            //运行解析以便为页面生成所需JS引用
            await parseManager.ParseInnerContentAsync(new StringBuilder(dynamicInfo.LoadingTemplate + dynamicInfo.YesTemplate + dynamicInfo.NoTemplate));

            var values = parseManager.SettingsManager.Encrypt(TranslateUtils.JsonSerialize(dynamicInfo));
            var display = dynamicInfo.IsInline ? "inline-block" : "block";
            var elementId = dynamicInfo.ElementId;

            return $@"
<span id=""{elementId}_loading"">{dynamicInfo.LoadingTemplate}</span>
<span id=""{elementId}_success"" style=""display: none""></span>
<span id=""{elementId}_failure"" style=""display: none""></span>
<script type=""text/javascript"" language=""javascript"">
function stlDynamic{elementId}(page)
{{
    document.getElementById('{elementId}_loading').style.display = '{display}';
    document.getElementById('{elementId}_success').style.display = 'none';
    document.getElementById('{elementId}_failure').style.display = 'none';
    {dynamicInfo.OnBeforeSend}
    stlClient.post('{dynamicApiUrl}?' + StlClient.getQueryString(), {{
        value: '{values}',
        page: page
    }}, function (err, data, status) {{
        if (!err) {{
            if (data.value) {{
                {dynamicInfo.OnSuccess}
                document.getElementById('{elementId}_success').innerHTML = data.html;
                document.getElementById('{elementId}_success').style.display = '{display}';
            }} else {{
                document.getElementById('{elementId}_failure').innerHTML = data.html;
                document.getElementById('{elementId}_failure').style.display = '{display}';
            }}
        }} else {{
            {dynamicInfo.OnError}
        }}
        {dynamicInfo.OnComplete}
    }});
}}

function stlGetPage{elementId}(){{
    var page = 1;
    var queryString = document.location.search;
    if (queryString && queryString.length > 1) {{
        queryString = queryString.substring(1);
        var arr = queryString.split('&');
        for(var i=0; i < arr.length; i++) {{
            var item = arr[i];
            var arr2 = item.split('=');
            if (arr2 && arr2.length == 2) {{
                if (arr2[0] === 'page') {{
                    page = parseInt(arr2[1]);
                }}
            }}
        }}
    }}
    return page;
}}

stlDynamic{elementId}(stlGetPage{elementId}());

function stlRedirect{elementId}(page)
{{
    var queryString = document.location.search;
    var parameters = '';
    if (queryString && queryString.length > 1) {{
        queryString = queryString.substring(1);
        
        var arr = queryString.split('&');
        for(var i=0; i < arr.length; i++) {{
            var item = arr[i];
            var arr2 = item.split('=');
            if (arr2 && arr2.length == 2) {{
                if (arr2[0] !== 'page') {{
                    parameters += item + '&';
                }}
            }}
        }}
    }}
    parameters += 'page=' + page;
    location.href = location.protocol + '//' + location.host + location.pathname + location.hash + '?' + parameters;
}}
</script>
";
        }
    }
}
