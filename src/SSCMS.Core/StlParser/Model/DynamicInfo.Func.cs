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
<span id=""{ElementId}_loading"">{LoadingTemplate}</span>
<span id=""{ElementId}_success"" style=""display: none""></span>
<span id=""{ElementId}_failure"" style=""display: none""></span>
<script type=""text/javascript"" language=""javascript"">
function stlDynamic{ElementId}(page)
{{
    document.getElementById('{ElementId}_loading').style.display = '{display}';
    document.getElementById('{ElementId}_success').style.display = 'none';
    document.getElementById('{ElementId}_failure').style.display = 'none';
    {OnBeforeSend}
    stlClient.post('{apiUrl}?' + StlClient.getQueryString(), {{
        value: '{values}',
        page: page
    }}, function (err, data, status) {{
        if (!err) {{
            if (data.value) {{
                {OnSuccess}
                document.getElementById('{ElementId}_success').innerHTML = data.html;
                document.getElementById('{ElementId}_success').style.display = '{display}';
            }} else {{
                document.getElementById('{ElementId}_failure').innerHTML = data.html;
                document.getElementById('{ElementId}_failure').style.display = '{display}';
            }}
        }} else {{
            {OnError}
        }}
        {OnComplete}
    }});
}}

function stlGetPage{ElementId}(){{
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

stlDynamic{ElementId}(stlGetPage{ElementId}());

function stlRedirect{ElementId}(page)
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
