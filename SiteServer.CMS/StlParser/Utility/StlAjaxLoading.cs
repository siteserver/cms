using BaiRong.Core;

namespace SiteServer.CMS.StlParser.Utility
{
	public class StlAjaxLoading
	{
        private string containerID;
        private string ajaxUrl;
        private string scriptName;
        private string updater;
        private string theEvent;

        public string ContainerID
        {
            get { return containerID; }
            set { containerID = value; }
        }

        public string AjaxUrl
        {
            get { return ajaxUrl; }
            set { ajaxUrl = value; }
        }

        public string ScriptName
        {
            get { return scriptName; }
            set { scriptName = value; }
        }

        public string Updater
        {
            get { return updater; }
            set { updater = value; }
        }

        public string Event
        {
            get { return theEvent; }
            set { theEvent = value; }
        }

        public StlAjaxLoading(string containerID, string ajaxUrl, string scriptName)
        {
            this.containerID = containerID;
            this.ajaxUrl = ajaxUrl;
            this.scriptName = scriptName;
        }

        private StlAjaxLoading()
        {

        }

        public string GetScript()
        {
            if (!string.IsNullOrEmpty(AjaxUrl))
            {
                var ajaxUrl = PageUtils.ParseNavigationUrl(AjaxUrl);

                string updateScript = $"{ScriptName}();";
                if (!string.IsNullOrEmpty(Updater) && !string.IsNullOrEmpty(Event))
                {
                    updateScript = $@"
<script type=""text/javascript"" language=""javascript"">
	Event.observe({Updater}, '{Event}', function(){{{ScriptName}()}}, false);
</script>
";
                }

                return $@"
<script type=""text/javascript"" language=""javascript"">
function {ScriptName}(ajaxUrl) {{
    if (!ajaxUrl){{
        ajaxUrl = '{ajaxUrl}';
        ajaxUrl += stlGetQueryString(true);
    }}
	var option = {{
		method:'get',
		evalScripts:true,
		onSuccess:function(){{

		}},
		onFailure:function(){{
			$('{containerID}').innerHTML = ""网络繁忙，请稍后再试...."";
		}}
	}};
	new Ajax.Updater ({{success:'{containerID}'}}, ajaxUrl, option);
}}
{updateScript}
</script>
	";
            }

            return string.Empty;
        }

        public string GetRenderHtml(bool isScript)
        {
            if (!string.IsNullOrEmpty(AjaxUrl))
            {
                return $@"
<div id=""{ContainerID}""></div>{((isScript) ? GetScript() : string.Empty)}
";
            }

            return string.Empty;
        }
	}
}
