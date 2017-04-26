using System.Collections.Specialized;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.StlTemplates
{
    public class GovInteractApplyTemplate : InputTemplateBase
    {
        private readonly PublishmentSystemInfo _publishmentSystemInfo;
        private readonly int _nodeId;
        private readonly TagStyleInfo _tagStyleInfo;
        private TagStyleGovInteractApplyInfo _tagStyleApplyInfo;

        public GovInteractApplyTemplate(PublishmentSystemInfo publishmentSystemInfo, int nodeId, TagStyleInfo tagStyleInfo, TagStyleGovInteractApplyInfo tagStyleApplyInfo)
        {
            _publishmentSystemInfo = publishmentSystemInfo;
            _nodeId = nodeId;
            _tagStyleInfo = tagStyleInfo;
            _tagStyleApplyInfo = tagStyleApplyInfo;
        }

        public string GetTemplate(bool isTemplate, string inputTemplateString, string successTemplateString, string failureTemplateString)
        {
            var inputBuilder = new StringBuilder();

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
                if (isTemplate)
                {
                    successBuilder.Append(_tagStyleInfo.SuccessTemplate);
                }
                else
                {
                    successBuilder.Append(GetFileSuccessTemplate());
                }
            }

            var failureBuilder = new StringBuilder();
            if (!string.IsNullOrEmpty(failureTemplateString))
            {
                failureBuilder.Append(failureTemplateString);
            }
            else
            {
                if (isTemplate)
                {
                    failureBuilder.Append(_tagStyleInfo.FailureTemplate);
                }
                else
                {
                    failureBuilder.Append(GetFileFailureTemplate());
                }
            }

            return ReplacePlaceHolder(inputBuilder.ToString(), successBuilder.ToString(), failureBuilder.ToString());
        }

        public string GetScript()
        {
            var script = @"function submit_apply_[nodeID]()
{
	if (checkFormValueById('frmApply_[nodeID]'))
	{
		$('#frmApply_[nodeID]').showLoading();
		var frmApply_[nodeID] = document.getElementById('frmApply_[nodeID]');
		frmApply_[nodeID].action = '[actionUrl]';
		frmApply_[nodeID].target = 'iframeApply_[nodeID]';
		frmApply_[nodeID].submit();
	}
}
function stlApplyCallback_[nodeID](jsonString){
    $('#frmApply_[nodeID]').hideLoading();
	var obj = eval('(' + jsonString + ')');
	if (obj){
		$('#applySuccess_[nodeID]').hide();
		$('#applyFailure_[nodeID]').hide();
		if (obj.isSuccess == 'false'){
			$('#applyFailure_[nodeID]').show();
			$('#applyFailureMessage_[nodeID]').html(obj.failureMessage);
		}else{
			$('#applySuccess_[nodeID]').show();
			$('#applyQueryCode_[nodeID]').html(obj.queryCode);
			$('#applyContainer_[nodeID]').hide();
		}
	}
}
";
            script = script.Replace("[nodeID]", _nodeId.ToString());
            script = script.Replace("[actionUrl]", ActionsGovInteractApplyAdd.GetUrl(_publishmentSystemInfo.Additional.ApiUrl, _publishmentSystemInfo.PublishmentSystemId, _nodeId, _tagStyleInfo.StyleID));
            return script;
        }

        public string GetFileInputTemplate()
        {
            var content = FileUtils.ReadText(SiteFilesAssets.GetPath("govinteractapply/inputTemplate.html"), ECharset.utf_8);

            var regex = "<!--parameters:(?<params>[^\"]*)-->";
            var paramstring = RegexUtils.GetContent("params", regex, content);
            var parameters = TranslateUtils.ToNameValueCollection(paramstring);
            var tdNameClass = parameters["tdNameClass"];
            var tdInputClass = parameters["tdInputClass"];

            if (parameters.Count > 0)
            {
                content = content.Replace($"<!--parameters:{paramstring}-->\r\n", string.Empty);
            }

            content = $@"<link href=""{SiteFilesAssets.GovInteractApply.GetStyleUrl(_publishmentSystemInfo.Additional.ApiUrl)}"" type=""text/css"" rel=""stylesheet"" />
" + content;

            var builder = new StringBuilder();
            var styleInfoList = RelatedIdentities.GetTableStyleInfoList(_publishmentSystemInfo, ETableStyle.GovInteractContent, _nodeId);

            var pageScripts = new NameValueCollection();

            var isPreviousSingleLine = true;
            var isPreviousLeftColumn = false;
            foreach (var styleInfo in styleInfoList)
            {
                if (styleInfo.IsVisible)
                {
                    var value = InputTypeParser.Parse(_publishmentSystemInfo, _nodeId, styleInfo, ETableStyle.GovInteractContent, styleInfo.AttributeName, null, false, false, null, pageScripts, styleInfo.Additional.IsValidate);

                    if (builder.Length > 0)
                    {
                        if (isPreviousSingleLine)
                        {
                            builder.Append("</tr>");
                        }
                        else
                        {
                            if (!isPreviousLeftColumn)
                            {
                                builder.Append("</tr>");
                            }
                            else if (styleInfo.IsSingleLine)
                            {
                                builder.Append(
                                    $@"<td class=""{tdNameClass}""></td><td class=""{tdInputClass}""></td></tr>");
                            }
                        }
                    }

                    //this line

                    if (styleInfo.IsSingleLine || isPreviousSingleLine || !isPreviousLeftColumn)
                    {
                        builder.Append("<tr>");
                    }

                    builder.Append(
                        $@"<td class=""{tdNameClass}"">{styleInfo.DisplayName}</td><td {(styleInfo.IsSingleLine
                            ? @"colspan=""3"""
                            : string.Empty)} class=""{tdInputClass}"">{value}</td>");


                    if (styleInfo.IsSingleLine)
                    {
                        isPreviousSingleLine = true;
                        isPreviousLeftColumn = false;
                    }
                    else
                    {
                        isPreviousSingleLine = false;
                        isPreviousLeftColumn = !isPreviousLeftColumn;
                    }
                }
            }

            if (builder.Length > 0)
            {
                if (isPreviousSingleLine || !isPreviousLeftColumn)
                {
                    builder.Append("</tr>");
                }
                else
                {
                    builder.Append($@"<td class=""{tdNameClass}""></td><td class=""{tdInputClass}""></td></tr>");
                }
            }

            if (content.Contains("<!--提交表单循环-->"))
            {
                content = content.Replace("<!--提交表单循环-->", builder.ToString());
            }

            return content.Replace("[nodeID]", _nodeId.ToString());
        }

        public string GetFileSuccessTemplate()
        {
            var retval = FileUtils.ReadText(SiteFilesAssets.GetPath("govinteractapply/successTemplate.html"), ECharset.utf_8);
            return retval.Replace("[nodeID]", _nodeId.ToString());
        }

        public string GetFileFailureTemplate()
        {
            var retval = FileUtils.ReadText(SiteFilesAssets.GetPath("govinteractapply/failureTemplate.html"), ECharset.utf_8);
            return retval.Replace("[nodeID]", _nodeId.ToString());
        }

        public static string GetCallbackScript(PublishmentSystemInfo publishmentSystemInfo, int nodeId, bool isSuccess, string queryCode, string failureMessage)
        {
            var jsonAttributes = new NameValueCollection
            {
                {"isSuccess", isSuccess.ToString().ToLower()},
                {"queryCode", queryCode},
                {"failureMessage", failureMessage}
            };

            var jsonString = TranslateUtils.NameValueCollectionToJsonString(jsonAttributes);
            jsonString = StringUtils.ToJsString(jsonString);

            string retval = $"<script>window.parent.stlApplyCallback_[nodeID]('{jsonString}');</script>";
            return retval.Replace("[nodeID]", nodeId.ToString());
        }

        private string ReplacePlaceHolder(string fileInputTemplate, string fileSuccessTemplate, string fileFailureTemplate)
        {
            var parsedContent = new StringBuilder();
            parsedContent.AppendFormat(@"
<div id=""applySuccess_[nodeID]"" style=""display:none"">{0}</div>
<div id=""applyFailure_[nodeID]"" style=""display:none"">{1}</div>
<div id=""applyContainer_[nodeID]"" class=""applyContainer"">
  <form id=""frmApply_[nodeID]"" name=""frmApply_[nodeID]"" style=""margin:0;padding:0"" method=""post"" enctype=""multipart/form-data"">
  {2}
  </form>
  <iframe id=""iframeApply_[nodeID]"" name=""iframeApply_[nodeID]"" width=""0"" height=""0"" frameborder=""0""></iframe>
</div>
", fileSuccessTemplate, fileFailureTemplate, fileInputTemplate);

            parsedContent.Replace("[nodeID]", _nodeId.ToString());

            return parsedContent.ToString();
        }
    }
}
