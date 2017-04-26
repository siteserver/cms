using System.Collections.Specialized;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.CMS.StlTemplates
{
	public class GovInteractQueryTemplate
	{
        private readonly PublishmentSystemInfo _publishmentSystemInfo;
        private readonly int _nodeId;
        private readonly TagStyleInfo _tagStyleInfo;

        public GovInteractQueryTemplate(PublishmentSystemInfo publishmentSystemInfo, int nodeId, TagStyleInfo tagStyleInfo)
        {
            _publishmentSystemInfo = publishmentSystemInfo;
            _nodeId = nodeId;
            _tagStyleInfo = tagStyleInfo;
        }

        public string GetTemplate(bool isTemplate, string inputTemplateString, string successTemplateString, string failureTemplateString)
        {
            var inputBuilder = new StringBuilder();

            inputBuilder.Append($@"
<link href=""{SiteFilesAssets.GovInteractQuery.GetStyleUrl(_publishmentSystemInfo.Additional.ApiUrl)}"" type=""text/css"" rel=""stylesheet"" />
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
                string content =
                    $@"<textarea id=""successTemplate"" style=""display:none"">{GetFileSuccessTemplate()}</textarea>";
                successBuilder.Append(content);
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
            var script = @"function submit_query()
{
	if (checkFormValueById('frmQuery'))
	{
		$('#frmQuery').showLoading();
		var frmQuery = document.getElementById('frmQuery');
		frmQuery.action = '[actionUrl]';
		frmQuery.target = 'iframeQuery';
		frmQuery.submit();
	}
}
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
		}
	}
}
";
            script = script.Replace("[actionUrl]", ActionsGovInteractQueryAdd.GetUrl(_publishmentSystemInfo.Additional.ApiUrl, _publishmentSystemInfo.PublishmentSystemId, _nodeId));

            return script;
        }

        public string GetFileInputTemplate()
        {
            return FileUtils.ReadText(SiteFilesAssets.GetPath("govinteractquery/inputTemplate.html"), ECharset.utf_8);
        }

        public string GetFileSuccessTemplate()
        {
            var content = FileUtils.ReadText(SiteFilesAssets.GetPath("govinteractquery/successTemplate.html"), ECharset.utf_8);

            var regex = "<!--parameters:(?<params>[^\"]*)-->";
            var paramstring = RegexUtils.GetContent("params", regex, content);
            var parameters = TranslateUtils.ToNameValueCollection(paramstring);
            var tdNameClass = parameters["tdNameClass"];
            var tdInputClass = parameters["tdInputClass"];

            if (parameters.Count > 0)
            {
                content = content.Replace($"<!--parameters:{paramstring}-->\r\n", string.Empty);
            }

            var builder = new StringBuilder();
            var styleInfoList = TableStyleManager.GetTableStyleInfoList(ETableStyle.GovInteractContent, _publishmentSystemInfo.AuxiliaryTableForGovInteract, RelatedIdentities.GetChannelRelatedIdentities(_publishmentSystemInfo.PublishmentSystemId, _nodeId));

            var isPreviousSingleLine = true;
            var isPreviousLeftColumn = false;
            foreach (var styleInfo in styleInfoList)
            {
                if (styleInfo.IsVisible)
                {
                    if (StringUtils.EqualsIgnoreCase(GovInteractContentAttribute.IsPublic, styleInfo.AttributeName) || StringUtils.EqualsIgnoreCase(GovInteractContentAttribute.DepartmentId, styleInfo.AttributeName) || StringUtils.EqualsIgnoreCase(GovInteractContentAttribute.TypeId, styleInfo.AttributeName)) continue;
                    string value = $"{{$T.{styleInfo.AttributeName.ToLower()}}}";

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

            if (content.Contains("<!--查询表单循环-->"))
            {
                content = content.Replace("<!--查询表单循环-->", builder.ToString());
            }

            return content;
        }

        public string GetFileFailureTemplate()
        {
            return FileUtils.ReadText(SiteFilesAssets.GetPath("govinteractquery/failureTemplate.html"), ECharset.utf_8);
        }

        public static string GetCallbackScript(PublishmentSystemInfo publishmentSystemInfo, bool isSuccess, GovInteractContentInfo contentInfo, string failureMessage)
        {
            var jsonAttributes = new NameValueCollection();
            jsonAttributes.Add("isSuccess", isSuccess.ToString().ToLower());
            if (isSuccess && contentInfo != null)
            {
                foreach (string attributeName in contentInfo.Attributes.Keys)
                {
                    jsonAttributes.Add(attributeName, contentInfo.GetExtendedAttribute(attributeName));
                }
                jsonAttributes.Add("replystate", EGovInteractStateUtils.GetFrontText(contentInfo.State));
                if (contentInfo.State == EGovInteractState.Checked || contentInfo.State == EGovInteractState.Denied)
                {
                    var replyInfo = DataProvider.GovInteractReplyDao.GetReplyInfoByContentId(publishmentSystemInfo.PublishmentSystemId, contentInfo.Id);
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
