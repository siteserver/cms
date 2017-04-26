using System.Collections.Specialized;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.StlTemplates
{
    public class ResumeTemplate
    {
        private readonly PublishmentSystemInfo _publishmentSystemInfo;

        public const string HolderStyleId = "{StyleID}";
        public const string HolderActionUrl = "{ActionUrl}";

        public ResumeTemplate(PublishmentSystemInfo publishmentSystemInfo)
        {
            _publishmentSystemInfo = publishmentSystemInfo;
        }

        public string GetTemplate(string successTemplateString, string failureTemplateString)
        {
            var builder = new StringBuilder();

            builder.Append($@"
<link href=""{SiteFilesAssets.Resume.GetStyleUrl(_publishmentSystemInfo.Additional.ApiUrl)}"" type=""text/css"" rel=""stylesheet"" />
<script type=""text/javascript"">
var resumeActionUrl = '{ActionsResumeAdd.GetUrl(_publishmentSystemInfo.Additional.ApiUrl, _publishmentSystemInfo.PublishmentSystemId)}';
var resumeAjaxUploadUrl = '{ActionsUpload.GetUrl(_publishmentSystemInfo.Additional.ApiUrl, _publishmentSystemInfo.PublishmentSystemId, ActionsUpload.TypeResume)}';
</script>
<script type=""text/javascript"" charset=""utf-8"" src=""{SiteFilesAssets.Resume.GetScriptUrl(_publishmentSystemInfo.Additional.ApiUrl)}""></script>
");

            builder.Append($@"<script type=""text/javascript"">{GetScript()}</script>");

            builder.Append(GetContent());

            return ReplacePlaceHolder(_publishmentSystemInfo.Additional.ApiUrl, builder.ToString(), successTemplateString, failureTemplateString);
        }

        public string GetScript()
        {
            return @"
function stlResumeCallback(jsonString){
	var obj = eval('(' + jsonString + ')');
	if (obj){
		document.getElementById('resumeSuccess').style.display = 'none';
		document.getElementById('resumeFailure').style.display = 'none';
		if (obj.isSuccess == 'false'){
			document.getElementById('resumeFailure').style.display = '';
			document.getElementById('resumeFailure').innerHTML = obj.message;
		}else{
			document.getElementById('resumeSuccess').style.display = '';
			document.getElementById('resumeSuccess').innerHTML = obj.message;
			document.getElementById('resumeContainer').style.display = 'none';
		}
	}
}
";
        }

        public string GetContent()
        {
            return FileUtils.ReadText(SiteFilesAssets.GetPath("resume/template.html"), ECharset.utf_8);
        }

        public static string GetCallbackScript(PublishmentSystemInfo publishmentSystemInfo, bool isSuccess, string message)
        {
            var jsonAttributes = new NameValueCollection
            {
                {"isSuccess", isSuccess.ToString().ToLower()},
                {"message", message}
            };

            var jsonString = TranslateUtils.NameValueCollectionToJsonString(jsonAttributes);
            jsonString = StringUtils.ToJsString(jsonString);

            return $"<script>window.parent.stlResumeCallback('{jsonString}');</script>";
        }

        public static string ReplacePlaceHolder(string apiUrl, string template, string successTemplateString, string failureTemplateString)
        {
            var parsedContent = new StringBuilder();

            parsedContent.Append(@"
<div id=""resumeSuccess"" class=""successContainer"" style=""display:none""></div>
<div id=""resumeFailure"" class=""failureContainer"" style=""display:none""></div>
<div id=""resumeContainer"" class=""resumeContainer"">
  <form id=""frmResume"" name=""frmResume"" style=""margin:0;padding:0"" method=""post"" enctype=""multipart/form-data"">
  <input type=""hidden"" id=""JobContentID"" name=""JobContentID"" value="""" />
");
            //添加遮罩层
            parsedContent.Append($@"	
<div id=""resumeModal"" times=""2"" id=""xubox_shade2"" class=""xubox_shade"" style=""z-index:19891016; background-color: #FFF; opacity: 0.5; filter:alpha(opacity=10);top: 0;left: 0;width: 100%;height: 100%;position: fixed;display:none;""></div>
<div id=""resumeModalMsg"" times=""2"" showtime=""0"" style=""z-index: 19891016; left: 50%; top: 206px; width: 500px; height: 360px; margin-left: -250px;position: fixed;text-align: center;display:none;"" id=""xubox_layer2"" class=""xubox_layer"" type=""iframe""><img src = ""{SiteFilesAssets.GetUrl(apiUrl, SiteFilesAssets.FileWaiting)}"" style="""">
<br>
<span style=""font-size:10px;font-family:Microsoft Yahei"">正在提交...</span>
</div>
<script>
		function openModal()
        {{
			document.getElementById(""resumeModal"").style.display = '';
            document.getElementById(""resumeModalMsg"").style.display = '';
        }}
        function closeModal()
        {{
			document.getElementById(""resumeModal"").style.display = 'none';
            document.getElementById(""resumeModalMsg"").style.display = 'none';
        }}
</script>");

            if (!string.IsNullOrEmpty(successTemplateString))
            {
                parsedContent.AppendFormat(@"<input type=""hidden"" id=""successTemplateString"" value=""{0}"" />", TranslateUtils.EncryptStringBySecretKey(successTemplateString));
            }
            if (!string.IsNullOrEmpty(failureTemplateString))
            {
                parsedContent.AppendFormat(@"<input type=""hidden"" id=""failureTemplateString"" value=""{0}"" />", TranslateUtils.EncryptStringBySecretKey(failureTemplateString));
            }

            parsedContent.Append(template);

            parsedContent.Append(@"
</form>
<iframe id=""iframeResume"" name=""iframeResume"" width=""0"" height=""0"" frameborder=""0""></iframe>
</div>");

            return parsedContent.ToString();
        }
    }
}