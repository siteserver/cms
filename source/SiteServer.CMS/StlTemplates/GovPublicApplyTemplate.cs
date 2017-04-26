using System.Collections.Specialized;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.StlTemplates
{
	public class GovPublicApplyTemplate
	{
        private readonly PublishmentSystemInfo _publishmentSystemInfo;
        private readonly TagStyleInfo _tagStyleInfo;
        private TagStyleGovPublicApplyInfo _applyInfo;

        public const string HolderStyleId = "{StyleID}";
        public const string HolderActionUrl = "{ActionUrl}";

        public GovPublicApplyTemplate(PublishmentSystemInfo publishmentSystemInfo, TagStyleInfo tagStyleInfo, TagStyleGovPublicApplyInfo applyInfo)
        {
            _publishmentSystemInfo = publishmentSystemInfo;
            _tagStyleInfo = tagStyleInfo;
            _applyInfo = applyInfo;
        }

        public string GetTemplate(bool isTemplate, string inputTemplateString, string successTemplateString, string failureTemplateString)
        {
            var inputBuilder = new StringBuilder();

            inputBuilder.Append($@"
<link href=""{SiteFilesAssets.GovPublicApply.GetStyleUrl(_publishmentSystemInfo.Additional.ApiUrl)}"" type=""text/css"" rel=""stylesheet"" />
<script type=""text/javascript"">
var govPublicActionUrl = '{ActionsGovPublicApplyAdd.GetUrl(_publishmentSystemInfo.Additional.ApiUrl,
                _publishmentSystemInfo.PublishmentSystemId, _tagStyleInfo.StyleID)}';
var govPublicAjaxUploadUrl = '{ActionsUpload.GetUrl(_publishmentSystemInfo.Additional.ApiUrl,
                _publishmentSystemInfo.PublishmentSystemId, ActionsUpload.TypeGovPublicApply)}';
</script>
<script type=""text/javascript"" charset=""utf-8"" src=""{SiteFilesAssets.GovPublicApply.GetScriptUrl(
                _publishmentSystemInfo.Additional.ApiUrl)}""></script>
");

            if (!string.IsNullOrEmpty(inputTemplateString))
            {
                inputBuilder.Append($@"<script type=""text/javascript"">{GetScript()}</script>");
                inputBuilder.Append(inputTemplateString);
            }
            else
            {
                if (isTemplate)
                {
                    if (!string.IsNullOrEmpty(_tagStyleInfo.ScriptTemplate))
                    {
                        inputBuilder.Append($@"<script type=""text/javascript"">{_tagStyleInfo.ScriptTemplate}</script>");
                    }
                    inputBuilder.Append(_tagStyleInfo.ContentTemplate);
                }
                else
                {
                    inputBuilder.Append($@"<script type=""text/javascript"">{GetScript()}</script>");
                    inputBuilder.Append(GetFileInputTemplate());
                }
            }

            var successBuilder = new StringBuilder();
            if (!string.IsNullOrEmpty(successTemplateString))
            {
                successBuilder.Append(successTemplateString);
            }
            else
            {
                successBuilder.Append(GetFileSuccessTemplate());
            }

            var failureBuilder = new StringBuilder();
            if (!string.IsNullOrEmpty(failureTemplateString))
            {
                failureBuilder.Append(failureTemplateString);
            }
            else
            {
                failureBuilder.Append(GetFileFailureTemplate());
            }

            return ReplacePlaceHolder(inputBuilder.ToString(), successBuilder.ToString(), failureBuilder.ToString());
        }

        public string GetScript()
        {
            return @"
function stlApplyCallback(jsonString){
    $('#frmApply').hideLoading();
	var obj = eval('(' + jsonString + ')');
	if (obj){
		$('#applySuccess').hide();
		$('#applyFailure').hide();
		if (obj.isSuccess == 'false'){
			$('#applyFailure').show();
			$('#applyFailureMessage').html(obj.failureMessage);
		}else{
			$('#applySuccess').show();
			$('#applyQueryCode').html(obj.queryCode);
			$('#applyContainer').hide();
		}
	}
}
";
        }

        public string GetFileInputTemplate()
        {
            return FileUtils.ReadText(SiteFilesAssets.GetPath("govpublicapply/inputTemplate.html"), ECharset.utf_8);
        }

        public string GetFileSuccessTemplate()
        {
            return FileUtils.ReadText(SiteFilesAssets.GetPath("govpublicapply/successTemplate.html"), ECharset.utf_8);
        }

        public string GetFileFailureTemplate()
        {
            return FileUtils.ReadText(SiteFilesAssets.GetPath("govpublicapply/failureTemplate.html"), ECharset.utf_8);
        }

        public static string GetCallbackScript(PublishmentSystemInfo publishmentSystemInfo, bool isSuccess, string queryCode, string failureMessage)
        {
            var jsonAttributes = new NameValueCollection();
            jsonAttributes.Add("isSuccess", isSuccess.ToString().ToLower());
            jsonAttributes.Add("queryCode", queryCode);
            jsonAttributes.Add("failureMessage", failureMessage);

            var jsonString = TranslateUtils.NameValueCollectionToJsonString(jsonAttributes);
            jsonString = StringUtils.ToJsString(jsonString);

            return $"<script>window.parent.stlApplyCallback('{jsonString}');</script>";
        }

        private static string ReplacePlaceHolder(string fileInputTemplate, string fileSuccessTemplate, string fileFailureTemplate)
        {
            return $@"
<div id=""applySuccess"" style=""display:none"">{fileSuccessTemplate}</div>
<div id=""applyFailure"" style=""display:none"">{fileFailureTemplate}</div>
<div id=""applyContainer"" class=""applyContainer"">
  <form id=""frmApply"" name=""frmApply"" style=""margin:0;padding:0"" method=""post"" enctype=""multipart/form-data"">
  {fileInputTemplate}
  </form>
  <iframe id=""iframeApply"" name=""iframeApply"" width=""0"" height=""0"" frameborder=""0""></iframe>
</div>
";
        }
	}
}
