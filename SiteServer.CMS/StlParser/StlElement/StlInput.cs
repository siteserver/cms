using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parsers;
using SiteServer.CMS.StlParser.Utility;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.Cache;
using SiteServer.Plugin;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "提交表单", Description = "通过 stl:input 标签在模板中实现提交表单功能")]
    public class StlInput
    {
        private StlInput() { }
        public const string ElementName = "stl:input";

        public const string AttributeInputName = "inputName";

        public static SortedList<string, string> AttributeList => new SortedList<string, string>
        {
            {AttributeInputName, "提交表单名称"}
        };

        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
        {
            var inputName = string.Empty;

            foreach (var name in contextInfo.Attributes.Keys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, AttributeInputName))
                {
                    inputName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
            }

            //var inputId = DataProvider.InputDao.GetInputIdAsPossible(inputName, pageInfo.PublishmentSystemId);
            var inputId = Input.GetInputIdAsPossible(inputName, pageInfo.PublishmentSystemId);
            if (inputId <= 0) return string.Empty;

            //var inputInfo = DataProvider.InputDao.GetInputInfo(inputId);
            var inputInfo = Input.GetInputInfo(inputId);
            var relatedIdentities = RelatedIdentities.GetRelatedIdentities(ETableStyle.InputContent, pageInfo.PublishmentSystemId, inputInfo.InputId);
            var styleInfoList = TableStyleManager.GetTableStyleInfoList(ETableStyle.InputContent, DataProvider.InputContentDao.TableName, relatedIdentities);
            var pageScripts = new NameValueCollection();
            var attributesHtml = GetAttributesHtml(pageScripts, pageInfo.PublishmentSystemInfo, styleInfoList);

            string template;
            string loading;
            string yes;
            string no;
            StlInnerUtility.GetTemplateLoadingYesNo(contextInfo.InnerXml, out template, out loading, out yes, out no);

            if (string.IsNullOrEmpty(template))
            {
                template = attributesHtml + TemplateManager.GetContentByFilePath(SiteFilesAssets.Input.TemplatePath);
            }
            if (string.IsNullOrEmpty(loading))
            {
                loading = TemplateManager.GetContentByFilePath(SiteFilesAssets.Input.LoadingPath);
            }
            if (string.IsNullOrEmpty(yes))
            {
                yes = TemplateManager.GetContentByFilePath(SiteFilesAssets.Input.YesPath);
            }
            if (string.IsNullOrEmpty(no))
            {
                no = TemplateManager.GetContentByFilePath(SiteFilesAssets.Input.NoPath);
            }

            return ParseImpl(pageInfo, contextInfo, inputInfo, pageScripts, styleInfoList, template, loading, yes, no);
        }

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, InputInfo inputInfo, NameValueCollection pageScripts, List<TableStyleInfo> styleInfoList, string template, string loading, string yes, string no)
        {
            pageInfo.AddPageScriptsIfNotExists(PageInfo.Components.Jquery);
            var stlContainerId = $"stl_input_{inputInfo.InputId}";
            var stlFormId = $"stl_form_{inputInfo.InputId}";

            template = StlParserManager.ParseInnerContent(template, pageInfo, contextInfo);
            loading = StlParserManager.ParseInnerContent(loading, pageInfo, contextInfo);
            yes = StlParserManager.ParseInnerContent(yes, pageInfo, contextInfo);
            no = StlParserManager.ParseInnerContent(no, pageInfo, contextInfo);

            var templateBuilder = new StringBuilder();
            templateBuilder.Append($@"
<script type=""text/javascript"" src=""{SiteFilesAssets.Input.GetScriptUrl(pageInfo.ApiUrl)}""></script>
<form id=""{stlFormId}"" name=""{stlFormId}"" style=""margin:0;padding:0"" method=""post"" enctype=""multipart/form-data"" action=""{ActionsInputAdd.GetUrl(pageInfo.ApiUrl, pageInfo.PublishmentSystemId, inputInfo.InputId)}"" target=""stl_iframe_{inputInfo.InputId}"">
    {template}
</form>
<iframe id=""stl_iframe_{inputInfo.InputId}"" name=""stl_iframe_{inputInfo.InputId}"" width=""0"" height=""0"" frameborder=""0""></iframe>
");

            foreach (string key in pageScripts.Keys)
            {
                templateBuilder.Append(pageScripts[key]);
            }

            var idList = new List<string>();
            var formElements = StlHtmlUtility.GetHtmlFormElements(templateBuilder.ToString());
            if (formElements != null && formElements.Count > 0)
            {
                foreach (var formElement in formElements)
                {
                    string tagName;
                    string innerXml;
                    NameValueCollection attributes;
                    StlHtmlUtility.ParseHtmlElement(formElement, out tagName, out innerXml, out attributes);

                    if (string.IsNullOrEmpty(attributes["id"])) continue;

                    foreach (var styleInfo in styleInfoList)
                    {
                        if (!StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, attributes["id"])) continue;

                        string validateAttributes;
                        var validateHtmlString = GetValidateHtmlString(styleInfo, out validateAttributes);
                        attributes["id"] = styleInfo.AttributeName;
                        attributes["name"] = styleInfo.AttributeName;

                        var replace = StringUtils.EqualsIgnoreCase(tagName, "input")
                            ? $@"<{tagName} {TranslateUtils.ToAttributesString(attributes)} {validateAttributes} />{validateHtmlString}"
                            : $@"<{tagName} {TranslateUtils.ToAttributesString(attributes)} {validateAttributes} >{innerXml}</{tagName}>{validateHtmlString}";

                        templateBuilder.Replace(formElement, replace);

                        idList.Add(styleInfo.AttributeName);
                    }
                }
            }

            StlHtmlUtility.RewriteSubmitButton(templateBuilder, $"inputSubmit(this, '{stlFormId}', '{stlContainerId}', [{TranslateUtils.ToSqlInStringWithQuote(idList)}]);return false;");

            StlParserManager.ParseInnerContent(templateBuilder, pageInfo, contextInfo);

            return $@"
<div id=""{stlContainerId}"">
    <div class=""stl_input_template"">{templateBuilder}</div>
    <div class=""stl_input_loading"" style=""display:none"">{loading}</div>
    <div class=""stl_input_yes"" style=""display:none"">{yes}</div>
    <div class=""stl_input_no"" style=""display:none"">{no}</div>
</div>
";
        }

        public static string GetDefaultStlInputStlElement(PublishmentSystemInfo publishmentSystemInfo, InputInfo inputInfo)
        {
            var pageScripts = new NameValueCollection();
            var relatedIdentities = RelatedIdentities.GetRelatedIdentities(ETableStyle.InputContent, publishmentSystemInfo.PublishmentSystemId, inputInfo.InputId);
            var styleInfoList = TableStyleManager.GetTableStyleInfoList(ETableStyle.InputContent, DataProvider.InputContentDao.TableName, relatedIdentities);
            var attributesHtml = GetAttributesHtml(pageScripts, publishmentSystemInfo, styleInfoList);

            var template = attributesHtml + TemplateManager.GetContentByFilePath(SiteFilesAssets.Input.TemplatePath);
            var loading = TemplateManager.GetContentByFilePath(SiteFilesAssets.Input.LoadingPath);
            var yes = TemplateManager.GetContentByFilePath(SiteFilesAssets.Input.YesPath);
            var no = TemplateManager.GetContentByFilePath(SiteFilesAssets.Input.NoPath);

            return $@"
<stl:input inputName=""{inputInfo.InputName}"">
    <stl:template>
        {template}
    </stl:template>

    <stl:loading>
        {loading}
    </stl:loading>

    <stl:yes>
        {yes}
    </stl:yes>

    <stl:no>
        {no}
    </stl:no>
</stl:input>";
        }

        private static string GetAttributesHtml(NameValueCollection pageScripts, PublishmentSystemInfo publishmentSystemInfo, List<TableStyleInfo> styleInfoList)
        {
            if (styleInfoList == null) return string.Empty;

            var output = new StringBuilder();

            foreach (var styleInfo in styleInfoList)
            {
                if (styleInfo.IsVisible == false) continue;

                var helpHtml = styleInfo.DisplayName + "：";
                var inputHtml = InputTypeParser.Parse(publishmentSystemInfo, styleInfo, styleInfo.AttributeName, pageScripts);
                output.Append($@"
<div>
    {helpHtml}
    {inputHtml}
</div>
");
            }

            return output.ToString();
        }

        private static string GetValidateHtmlString(TableStyleInfo styleInfo, out string validateAttributes)
        {
            validateAttributes = string.Empty;
            if (!styleInfo.Additional.IsValidate || InputTypeUtils.Equals(styleInfo.InputType, InputType.TextEditor)) return string.Empty;

            var builder = new StringBuilder();
            validateAttributes = InputParserUtils.GetValidateAttributes(styleInfo.Additional.IsValidate, styleInfo.DisplayName, styleInfo.Additional.IsRequired, styleInfo.Additional.MinNum, styleInfo.Additional.MaxNum, styleInfo.Additional.ValidateType, styleInfo.Additional.RegExp, styleInfo.Additional.ErrorMessage);

            builder.Append(
                $@"<span id=""{styleInfo.AttributeName}_msg"" style=""color:red;margin-left: 5px;display:none;"">*</span>");
            builder.Append($@"
<script>inputBlur('{styleInfo.AttributeName}');</script>
");
            return builder.ToString();
        }

        public static string GetPostMessageScript(int inputId, bool isSuccess)
        {
            var containerId = $"stl_input_{inputId}";
            return $"<script>window.parent.postMessage({{containerId: '{containerId}', isSuccess: {isSuccess.ToString().ToLower()}}}, '*');</script>";
        }
    }
}
