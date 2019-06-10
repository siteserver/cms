using System.Collections.Specialized;
using AngleSharp.Common;
using Microsoft.AspNetCore.Http;
using SS.CMS.Abstractions;
using SS.CMS.Core.Settings;
using SS.CMS.Utils;

namespace SS.CMS.Core.StlParser.Models
{
    public partial class DynamicInfo
    {
        public static DynamicInfo GetDynamicInfo(NameValueCollection queryString, string value, int page, IUserInfo userInfo)
        {
            var dynamicInfo = TranslateUtils.JsonDeserialize<DynamicInfo>(AppContext.Decrypt(value));
            if (dynamicInfo.ChannelId == 0)
            {
                dynamicInfo.ChannelId = dynamicInfo.SiteId;
            }
            dynamicInfo.UserInfo = userInfo;
            dynamicInfo.QueryString = queryString;
            dynamicInfo.QueryString.Remove("siteId");

            dynamicInfo.Page = page;

            return dynamicInfo;
        }

        public string GetScript(string apiUrl)
        {
            if (string.IsNullOrEmpty(LoadingTemplate) &&
                string.IsNullOrEmpty(SuccessTemplate) &&
                string.IsNullOrEmpty(FailureTemplate))
            {
                return string.Empty;
            }

            var values = AppContext.Encrypt(TranslateUtils.JsonSerialize(this));

            return $@"
<div id=""{AjaxDivId}_loading"">{LoadingTemplate}</div>
<div id=""{AjaxDivId}_success"" style=""display: none""></div>
<div id=""{AjaxDivId}_failure"" style=""display: none""></div>
<script type=""text/javascript"" language=""javascript"">
function stlDynamic{AjaxDivId}(page)
{{
    document.getElementById('{AjaxDivId}_loading').style.display = 'block';
    document.getElementById('{AjaxDivId}_success').style.display = 'none';
    document.getElementById('{AjaxDivId}_failure').style.display = 'none';
    {OnBeforeSend}
    stlClient.post('{apiUrl}?' + StlClient.getQueryString(), {{
        value: '{values}',
        page: page
    }}, function (err, data, status) {{
        if (!err) {{
            if (data.value) {{
                {OnSuccess}
                document.getElementById('{AjaxDivId}_success').innerHTML = data.html;
                document.getElementById('{AjaxDivId}_success').style.display = 'block';
            }} else {{
                document.getElementById('{AjaxDivId}_failure').innerHTML = data.html;
                document.getElementById('{AjaxDivId}_failure').style.display = 'block';
            }}
        }} else {{
            {OnError}
        }}
        {OnComplete}
    }});
}}

function stlGetPage{AjaxDivId}(){{
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

stlDynamic{AjaxDivId}(stlGetPage{AjaxDivId}());

function stlRedirect{AjaxDivId}(page)
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
