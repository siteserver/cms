using System.Collections.Specialized;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.CMS.StlTemplates
{
	public class GovPublicQueryTemplate
	{
        private readonly PublishmentSystemInfo _publishmentSystemInfo;
        private readonly TagStyleInfo _tagStyleInfo;

        public const string HolderStyleId = "{StyleID}";

        public GovPublicQueryTemplate(PublishmentSystemInfo publishmentSystemInfo, TagStyleInfo tagStyleInfo)
        {
            _publishmentSystemInfo = publishmentSystemInfo;
            _tagStyleInfo = tagStyleInfo;
        }

        public string GetTemplate(bool isTemplate, string inputTemplateString, string successTemplateString, string failureTemplateString)
        {
            var inputBuilder = new StringBuilder();

            inputBuilder.Append($@"
<link href=""{SiteFilesAssets.GovPublicQuery.GetStyleUrl(_publishmentSystemInfo.Additional.ApiUrl)}"" type=""text/css"" rel=""stylesheet"" />
<script type=""text/javascript"">
var govPublicQueryActionUrl = '{ActionsGovPublicQueryAdd.GetUrl(_publishmentSystemInfo.Additional.ApiUrl,
                _publishmentSystemInfo.PublishmentSystemId, _tagStyleInfo.StyleID)}';
</script>
<script type=""text/javascript"" charset=""utf-8"" src=""{SiteFilesAssets.GovPublicQuery.GetScriptUrl(
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
function stlQueryCallback(jsonString){
    $('#frmQuery').hideLoading();
	var obj = eval('(' + jsonString + ')');
	if (obj){
		$('#querySuccess').hide();
		$('#queryFailure').hide();
		if (obj.isSuccess == 'false'){
			$('#queryFailure').show();
			$('#queryFailureMessage').html(obj.failureMessage);
		}else{
			$('#querySuccess').show();
			$('#querySuccess').setTemplateElement('successTemplate');
			$('#querySuccess').processTemplate(obj);
			$('#queryContainer').hide();
            if (obj.isOrg == 'true'){
                $('#dataContainer1').hide();
                $('#dataContainer2').show();
            }
		}
	}
}
";
        }

        public string GetFileInputTemplate()
        {
            return FileUtils.ReadText(SiteFilesAssets.GetPath("govpublicquery/inputTemplate.html"), ECharset.utf_8);
        }

        public string GetFileSuccessTemplate()
        {
            return FileUtils.ReadText(SiteFilesAssets.GetPath("govpublicquery/successTemplate.html"), ECharset.utf_8);
        }

        public string GetFileFailureTemplate()
        {
            return FileUtils.ReadText(SiteFilesAssets.GetPath("govpublicquery/failureTemplate.html"), ECharset.utf_8);
        }

        public static string GetCallbackScript(PublishmentSystemInfo publishmentSystemInfo, bool isSuccess, GovPublicApplyInfo applyInfo, string failureMessage)
        {
            var jsonAttributes = new NameValueCollection();
            jsonAttributes.Add("isSuccess", isSuccess.ToString().ToLower());
            if (isSuccess && applyInfo != null)
            {
                jsonAttributes.Add("isOrg", applyInfo.IsOrganization.ToString().ToLower());
                foreach (string attributeName in applyInfo.Attributes.Keys)
                {
                    jsonAttributes.Add(attributeName, applyInfo.GetExtendedAttribute(attributeName));
                }
                jsonAttributes.Add("replystate", EGovPublicApplyStateUtils.GetFrontText(applyInfo.State));
                if (applyInfo.State == EGovPublicApplyState.Checked || applyInfo.State == EGovPublicApplyState.Denied)
                {
                    var replyInfo = DataProvider.GovPublicApplyReplyDao.GetReplyInfoByApplyId(applyInfo.Id);
                    if (replyInfo != null)
                    {
                        jsonAttributes.Add("replycontent", replyInfo.Reply);
                        jsonAttributes.Add("replyfileurl", replyInfo.FileUrl);
                        jsonAttributes.Add("replydepartmentname", DepartmentManager.GetDepartmentName(replyInfo.DepartmentID));
                        jsonAttributes.Add("replyusername", replyInfo.UserName);
                        jsonAttributes.Add("replyadddate", DateUtils.GetDateAndTimeString(replyInfo.AddDate));
                    }
                }
            }
            jsonAttributes.Add("failureMessage", failureMessage);

            var jsonString = TranslateUtils.NameValueCollectionToJsonString(jsonAttributes);
            jsonString = StringUtils.ToJsString(jsonString);

            return $"<script>window.parent.stlQueryCallback('{jsonString}');</script>";
        }

        private static string ReplacePlaceHolder(string fileInputTemplate, string fileSuccessTemplate, string fileFailureTemplate)
        {
            return $@"
<div id=""querySuccess"" style=""display:none"">{fileSuccessTemplate}</div>
<div id=""queryFailure"" style=""display:none"">{fileFailureTemplate}</div>
<div id=""queryContainer"" class=""queryContainer"">
  <form id=""frmQuery"" name=""frmQuery"" style=""margin:0;padding:0"" method=""post"" enctype=""multipart/form-data"">
  {fileInputTemplate}
  </form>
  <iframe id=""iframeQuery"" name=""iframeQuery"" width=""0"" height=""0"" frameborder=""0""></iframe>
</div>
";
        }
	}
}
