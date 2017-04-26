using System.Collections.Specialized;

namespace SiteServer.BackgroundPages.Core
{
    public class AjaxManager
    {
        public static string RegisterWaitingTaskScript(string url, string parameters)
        {
            return $@"
<script type=""text/javascript"" language=""javascript"">
function WebServiceUtility_Task()
{{
	var url = '{url}';
	var pars = '{parameters}';

    jQuery.post(url, pars, function(data, textStatus)
    {{
        writeResult(data.resultMessage, data.errorMessage);
        if (data.executeCode)
		{{
			setTimeout(function()
			{{
				eval(data.executeCode);
			}}, 1000);
		}}
    }}, 'json');
}}

$(document).ready(function(){{
    WebServiceUtility_Task();
}});
</script>
";
        }

        public static string RegisterProgressTaskScript(string url, string pars, string userKeyPrefix, string countArrayUrl)
        {
            return $@"
<script type=""text/javascript"" language=""javascript"">
var intervalID;
function WebServiceUtility_Task()
{{
	var url = '{url}';
	var pars = '{pars}';

    jQuery.post(url, pars, function(data, textStatus)
    {{
        clearInterval(intervalID);
        if (data.isPoweredBy == 'false')
        {{
            $('#poweredby').show();
        }}
        writeResult(data.resultMessage, data.errorMessage);
    }}, 'json');
}}

function WebServiceUtility_GetCountArray()
{{
	var url = '{countArrayUrl}';
	var pars = 'userKeyPrefix={userKeyPrefix}';

    jQuery.post(url, pars, function(data, textStatus)
    {{
        if (data.totalCount)
        {{
            var totalCount = parseInt(data.totalCount);
            var currentCount = parseInt(data.currentCount);
            if (data.isPoweredBy == 'false')
            {{
                $('poweredby').show();
            }}
            writeProgressBar(totalCount, currentCount, data.message);
        }}
    }}, 'json');
}}

function WebServiceUtility_Initialize()
{{
	WebServiceUtility_Task();
    intervalID = setInterval(WebServiceUtility_GetCountArray, 3000);
}}

$(document).ready(function(){{
    WebServiceUtility_Initialize();
}});
</script>
";
        }

        public static string RegisterProgressTaskScriptForService(string url, string pars, string userKeyPrefix, string countArrayUrl)
        {
            return $@"
<script type=""text/javascript"" language=""javascript"">
var intervalID;
function WebServiceUtility_Task()
{{
	var url = '{url}';
	var pars = '{pars}';

    jQuery.post(url, pars, function(data, textStatus)
    {{
        var totalCount = parseInt(data.totalCount);
        var currentCount = parseInt(data.currentCount);
        var queuingCount = parseInt(data.queuingCount);
        var errorCount = parseInt(data.errorCount);
        if (data.isPoweredBy == 'false')
        {{
            $('#poweredby').show();
        }}
        if(data.resultMessage!=''&&data.resultMessage!=undefined)
        {{
              clearInterval(intervalID);
              writeResult(data.resultMessage, data.errorMessage);
        }}
        else{{
              writeProgressBar(totalCount, currentCount, data.message);
        }}
    }}, 'json');
}}

function WebServiceUtility_GetCountArray()
{{
	var url = '{countArrayUrl}';
	var pars = 'userKeyPrefix={userKeyPrefix}';

    jQuery.post(url, pars, function(data, textStatus)
    {{
        if(data.resultMessage!=''&&data.resultMessage!=undefined)
        {{
            clearInterval(intervalID);
            writeResult(data.resultMessage, data.errorMessage);
        }}
        if (data.totalCount)
        {{
            var totalCount = parseInt(data.totalCount);
            var currentCount = parseInt(data.currentCount);
            var queuingCount = parseInt(data.queuingCount);
            var errorCount = parseInt(data.errorCount);
            if (data.isPoweredBy == 'false')
            {{
                $('poweredby').show();
            }}
            writeProgressBar(totalCount, currentCount, data.message);
        }}
    }}, 'json');
}}

function WebServiceUtility_Initialize()
{{
	WebServiceUtility_Task();
    intervalID = setInterval(WebServiceUtility_GetCountArray, 3000);
}}

$(document).ready(function(){{
    WebServiceUtility_Initialize();
}});
</script>
";
        }

        public static string RegisterProgressTaskScript(string url, string pars, string userKeyPrefix, string countArrayUrl, bool isRedirect)
        {
            if (!isRedirect)
            {
                return RegisterProgressTaskScript(url, pars, userKeyPrefix, countArrayUrl);
            }
            return $@"
<script type=""text/javascript"" language=""javascript"">
var intervalID;
function WebServiceUtility_Task()
{{
	var url = '{url}';
	var pars = '{pars}';

    jQuery.post(url, pars, function(data, textStatus)
    {{
        clearInterval(intervalID);
        if (data.isPoweredBy == 'false')
        {{
            $('#poweredby').show();
        }}
        writeResult(data.resultMessage, data.errorMessage);
        if (data.executeCode)
		{{
			setTimeout(function()
			{{
				eval(data.executeCode);
			}}, 1000);
		}}
    }}, 'json');
}}

function WebServiceUtility_GetCountArray()
{{
	var url = '{countArrayUrl}';
	var pars = 'userKeyPrefix={userKeyPrefix}';

    jQuery.post(url, pars, function(data, textStatus)
    {{
        if (data.totalCount)
        {{
            var totalCount = parseInt(data.totalCount);
            var currentCount = parseInt(data.currentCount);
            if (data.isPoweredBy == 'false')
            {{
                $('poweredby').show();
            }}
            writeProgressBar(totalCount, currentCount, data.message);
        }}
    }}, 'json');
}}

function WebServiceUtility_Initialize()
{{
	WebServiceUtility_Task();
    intervalID = setInterval(WebServiceUtility_GetCountArray, 3000);
}}

$(document).ready(function(){{
    WebServiceUtility_Initialize();
}});
</script>
";
        }

        public static NameValueCollection GetWaitingTaskNameValueCollection(string resultMessage, string errorMessage, string executeCode)
        {
            return new NameValueCollection
            {
                {"resultMessage", resultMessage},
                {"errorMessage", errorMessage},
                {"executeCode", executeCode}
            };
        }

        public static NameValueCollection GetProgressTaskNameValueCollection(string resultMessage, string errorMessage)
        {
            return new NameValueCollection
            {
                {"resultMessage", resultMessage},
                {"errorMessage", errorMessage}
            };
        }

        public static NameValueCollection GetCountArrayNameValueCollection(int totalCount, int currentCount, string message)
        {
            return new NameValueCollection
            {
                {"totalCount", totalCount.ToString()},
                {"currentCount", currentCount.ToString()},
                {"message", message}
            };
        }

        public static NameValueCollection GetCountArrayNameValueCollection(int totalCount, int currentCount, int errorCount, int queuingCount, string message)
        {
            return new NameValueCollection
            {
                {"totalCount", totalCount.ToString()},
                {"currentCount", currentCount.ToString()},
                {"errorCount", errorCount.ToString()},
                {"queuingCount", queuingCount.ToString()},
                {"message", message}
            };
        }
    }
}
