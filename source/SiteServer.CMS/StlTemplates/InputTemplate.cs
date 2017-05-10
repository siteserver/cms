using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Xml;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlTemplates
{
    public class InputTemplate : InputTemplateBase
    {
        private readonly PublishmentSystemInfo _publishmentSystemInfo;
        private readonly List<TableStyleInfo> _styleInfoList;
        private readonly InputInfo _inputInfo;

        public const string HolderInputId = "{InputID}";

        public InputTemplate(PublishmentSystemInfo publishmentSystemInfo, InputInfo inputInfo)
        {
            _publishmentSystemInfo = publishmentSystemInfo;
            var relatedIdentities = RelatedIdentities.GetRelatedIdentities(ETableStyle.InputContent, publishmentSystemInfo.PublishmentSystemId, inputInfo.InputId);
            _styleInfoList = TableStyleManager.GetTableStyleInfoList(ETableStyle.InputContent, DataProvider.InputContentDao.TableName, relatedIdentities);
            _inputInfo = inputInfo;
        }

        public string GetTemplate(bool isLoadValues, string inputTemplateString, string successTemplateString, string failureTemplateString)
        {
            var builder = new StringBuilder();

            builder.Append(
                $@"<script type=""text/javascript"" charset=""{SiteFilesAssets.Validate.Charset}"" src=""{SiteFilesAssets
                    .GetUrl(_publishmentSystemInfo.Additional.ApiUrl, SiteFilesAssets.Validate.Js)}""></script>");

            if (string.IsNullOrEmpty(inputTemplateString))
            {
                builder.Append($@"<style type=""text/css"">{GetStyle(ETableStyle.InputContent)}</style>");
                builder.Append($@"<script type=""text/javascript"">{GetScript()}</script>");

                builder.Append(GetContent());
            }
            else
            {
                builder.Append($@"<style type=""text/css"">{GetStyle(ETableStyle.InputContent)}</style>");
                builder.Append($@"<script type=""text/javascript"">{GetScript()}</script>");
                builder.Append(inputTemplateString);
            }

            return ReplacePlaceHolder(builder.ToString(), isLoadValues, successTemplateString, failureTemplateString);
        }

        public string GetScript()
        {
            var builder = new StringBuilder();

            var additionalScript = string.Empty;
            if (_inputInfo.Additional.IsSuccessHide)
            {
                additionalScript += $@"
			document.getElementById('inputContainer_{HolderInputId}').style.display = 'none';";
            }
            if (_inputInfo.Additional.IsSuccessReload)
            {
                additionalScript += @"
			setTimeout('window.location.reload(false)', 2000);";
            }
            builder.Append($@"
function stlInputCallback_{HolderInputId}(jsonString){{
    closeModal();
	var obj = eval('(' + jsonString + ')');
	if (obj){{
		document.getElementById('inputSuccess_{HolderInputId}').style.display = 'none';
		document.getElementById('inputFailure_{HolderInputId}').style.display = 'none';
		if (obj.isSuccess == 'false'){{
			document.getElementById('inputFailure_{HolderInputId}').style.display = '';
			document.getElementById('inputFailure_{HolderInputId}').innerHTML = obj.message;
		}}else{{
			document.getElementById('inputSuccess_{HolderInputId}').style.display = '';
			document.getElementById('inputSuccess_{HolderInputId}').innerHTML = obj.message;{additionalScript}
		}}
	}}
}}
");

            return builder.ToString();
        }

        public string GetContent()
        {
            var builder = new StringBuilder();

            builder.Append(@"
<table cellSpacing=""3"" cellPadding=""3"" border=""0"" width=""98%"">");

            var pageScripts = new NameValueCollection();
            var attributesHtml = GetAttributesHtml(pageScripts, _publishmentSystemInfo, _styleInfoList);

            builder.Append(attributesHtml);

            builder.Append($@"
<tr>
    <td colspan=""2"" style=""padding: 5px; text-align: center;"">
        <input id=""submit"" type=""button"" class=""is_btn"" value="" 提 交 "" />
        &nbsp;&nbsp;&nbsp;
        <input type=""reset"" class=""is_btn"" value="" 重 置 "" />
    </td>
</tr>
</table>
");

            return builder.ToString();
        }

        public string ReplacePlaceHolder(string template, bool isLoadValues, string successTemplateString, string failureTemplateString)
        {
            var parsedContent = new StringBuilder();

            parsedContent.Append($@"
<div id=""inputSuccess_{_inputInfo.InputId}"" class=""is_success"" style=""display:none""></div>
<div id=""inputFailure_{_inputInfo.InputId}"" class=""is_failure"" style=""display:none""></div>
<div id=""inputContainer_{_inputInfo.InputId}"">");

            //添加遮罩层
            parsedContent.Append($@"
<div id=""inputModal_{_inputInfo.InputId}"" times=""2"" id=""xubox_shade2"" class=""xubox_shade"" style=""z-index:19891016; background-color: #FFF; opacity: 0.5; filter:alpha(opacity=10);top: 0;left: 0;width: 100%;height: 100%;position: fixed;display:none;""></div>
<div id=""inputModalMsg_{_inputInfo.InputId}"" times=""2"" showtime=""0"" style=""z-index: 19891016; left: 50%; top: 206px; width: 500px; height: 360px; margin-left: -250px;position: fixed;text-align: center;display:none;"" id=""xubox_layer2"" class=""xubox_layer"" type=""iframe""><img src = ""{SiteFilesAssets.GetUrl(_publishmentSystemInfo.Additional.ApiUrl, SiteFilesAssets.FileWaiting)}"" style="""">
<br>
<span style=""font-size:10px;font-family:Microsoft Yahei"">正在提交...</span>
</div>
<script>
		function openModal()
        {{
			document.getElementById(""inputModal_{_inputInfo.InputId}"").style.display = '';
            document.getElementById(""inputModalMsg_{_inputInfo.InputId}"").style.display = '';
        }}
        function closeModal()
        {{
			document.getElementById(""inputModal_{_inputInfo.InputId}"").style.display = 'none';
            document.getElementById(""inputModalMsg_{_inputInfo.InputId}"").style.display = 'none';
        }}
</script>");

            var actionUrl = ActionsInputAdd.GetUrl(_publishmentSystemInfo.Additional.ApiUrl, _publishmentSystemInfo.PublishmentSystemId, _inputInfo.InputId);
            parsedContent.Append($@"
<form id=""frmInput_{_inputInfo.InputId}"" name=""frmInput_{_inputInfo.InputId}"" style=""margin:0;padding:0"" method=""post"" enctype=""multipart/form-data"" action=""{actionUrl}"" target=""loadInput_{_inputInfo.InputId}"">
");

            if (!string.IsNullOrEmpty(successTemplateString))
            {
                parsedContent.AppendFormat(@"<input type=""hidden"" id=""successTemplateString"" value=""{0}"" />", TranslateUtils.EncryptStringBySecretKey(successTemplateString));
            }
            if (!string.IsNullOrEmpty(failureTemplateString))
            {
                parsedContent.AppendFormat(@"<input type=""hidden"" id=""failureTemplateString"" value=""{0}"" />", TranslateUtils.EncryptStringBySecretKey(failureTemplateString));
            }

            parsedContent.Append(template);

            parsedContent.AppendFormat(@"
</form>
<iframe id=""loadInput_{0}"" name=""loadInput_{0}"" width=""0"" height=""0"" frameborder=""0""></iframe>
</div>", _inputInfo.InputId);

            var pageScripts = new NameValueCollection();
            GetAttributesHtml(pageScripts, _publishmentSystemInfo, _styleInfoList);

            foreach (string key in pageScripts.Keys)
            {
                parsedContent.Append(pageScripts[key]);
            }

            if (_inputInfo.Additional.IsCtrlEnter)
            {
                parsedContent.AppendFormat(@"
<script>document.body.onkeydown=function(e)
{{e=e?e:window.event;var tagname=e.srcElement?e.srcElement.tagName:e.target.tagName;if(tagname=='INPUT'||tagname=='TEXTAREA'){{if(e!=null&&e.ctrlKey&&e.keyCode==13){{document.getElementById('submit_{0}').click();}}}}}}</script>", _inputInfo.InputId);
            }

            string clickString =
                $@"if (checkFormValueById('frmInput_{HolderInputId}')){{openModal(); document.getElementById('frmInput_{HolderInputId}').submit();}}";

            StlHtmlUtility.ReWriteSubmitButton(parsedContent, clickString);

            var stlFormElements = StlHtmlUtility.GetStlFormElementsArrayList(parsedContent.ToString());
            if (stlFormElements != null && stlFormElements.Count > 0)
            {
                foreach (string stlFormElement in stlFormElements)
                {
                    XmlNode elementNode;
                    NameValueCollection attributes;
                    StlHtmlUtility.ReWriteFormElements(stlFormElement, out elementNode, out attributes);

                    var validateAttributes = string.Empty;
                    var validateHtmlString = string.Empty;

                    if (!string.IsNullOrEmpty(attributes["id"]))
                    {
                        foreach (var styleInfo in _styleInfoList)
                        {
                            if (StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, attributes["id"]))
                            {
                                validateHtmlString = InputParserUtility.GetValidateHtmlString(styleInfo, out validateAttributes);
                                attributes["id"] = styleInfo.AttributeName;
                            }
                        }
                    }

                    if (StringUtils.EqualsIgnoreCase(elementNode.Name, "input"))
                    {
                        parsedContent.Replace(stlFormElement,
                            $@"<{elementNode.Name} {TranslateUtils.ToAttributesString(attributes)} {validateAttributes}/>{validateHtmlString}");
                    }
                    else
                    {
                        parsedContent.Replace(stlFormElement,
                            $@"<{elementNode.Name} {TranslateUtils.ToAttributesString(attributes)} {validateAttributes}>{elementNode
                                .InnerXml}</{elementNode.Name}>{validateHtmlString}");
                    }
                }
            }

            parsedContent.Replace(HolderInputId, _inputInfo.InputId.ToString());

            if (isLoadValues)
            {
                parsedContent.AppendFormat(@"
<script type=""text/javascript"">stlInputLoadValues('frmInput_{0}');</script>
", _inputInfo.InputId);
            }

            return parsedContent.ToString();
        }

        public static string GetInputCallbackScript(PublishmentSystemInfo publishmentSystemInfo, int inputId, bool isSuccess, string message)
        {
            var jsonAttributes = new NameValueCollection
            {
                {"isSuccess", isSuccess.ToString().ToLower()},
                {"message", message}
            };

            var jsonString = TranslateUtils.NameValueCollectionToJsonString(jsonAttributes);
            jsonString = StringUtils.ToJsString(jsonString);
            return $"<script>window.parent.stlInputCallback_{inputId}('{jsonString}');</script>";
        }
    }
}