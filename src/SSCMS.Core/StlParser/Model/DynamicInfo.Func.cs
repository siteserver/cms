using SSCMS;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.Model
{
    public partial class DynamicInfo
    {
        public string GetScript(string apiUrl, bool inline)
        {
            if (string.IsNullOrEmpty(LoadingTemplate) && 
                string.IsNullOrEmpty(SuccessTemplate) &&
                string.IsNullOrEmpty(FailureTemplate))
            {
                return string.Empty;
            }

            var values = _settingsManager.Encrypt(TranslateUtils.JsonSerialize(this));
            var display = inline ? "inline-block" : "block";

            return $@"
<span id=""{AjaxDivId}_loading"">{LoadingTemplate}</span>
<span id=""{AjaxDivId}_success"" style=""display: none""></span>
<span id=""{AjaxDivId}_failure"" style=""display: none""></span>
<script type=""text/javascript"" language=""javascript"">
function stlDynamic{AjaxDivId}(page)
{{
    document.getElementById('{AjaxDivId}_loading').style.display = '{display}';
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
                document.getElementById('{AjaxDivId}_success').style.display = '{display}';
            }} else {{
                document.getElementById('{AjaxDivId}_failure').innerHTML = data.html;
                document.getElementById('{AjaxDivId}_failure').style.display = '{display}';
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
