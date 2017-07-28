using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Wcm.GovInteract;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.CMS.Core
{
    public class InputTypeParser
    {
        private InputTypeParser()
        {
        }

        public const string Current = "{Current}";

        public static string Parse(PublishmentSystemInfo publishmentSystemInfo, int nodeId, TableStyleInfo styleInfo, ETableStyle tableStyle, string attributeName, NameValueCollection formCollection, bool isEdit, bool isPostBack, string additionalAttributes, NameValueCollection pageScripts, bool isValidate)
        {
            var retval = string.Empty;

            var isAddAndNotPostBack = !isEdit && !isPostBack;

            var oriIsValidate = styleInfo.Additional.IsValidate;
            if (!isValidate)
            {
                styleInfo.Additional.IsValidate = false;
            }

            if (EInputTypeUtils.Equals(styleInfo.InputType, EInputType.Text))
            {
                retval = ParseText(publishmentSystemInfo, nodeId, attributeName, formCollection, isAddAndNotPostBack, additionalAttributes, styleInfo, tableStyle);
            }
            else if (EInputTypeUtils.Equals(styleInfo.InputType, EInputType.TextArea))
            {
                retval = ParseTextArea(attributeName, formCollection, isAddAndNotPostBack, additionalAttributes, styleInfo);
            }
            else if (EInputTypeUtils.Equals(styleInfo.InputType, EInputType.TextEditor))
            {
                retval = ParseTextEditor(publishmentSystemInfo, attributeName, formCollection, isAddAndNotPostBack, pageScripts, styleInfo);
            }
            else if (EInputTypeUtils.Equals(styleInfo.InputType, EInputType.SelectOne))
            {
                retval = ParseSelectOne(attributeName, formCollection, isAddAndNotPostBack, styleInfo);
            }
            else if (EInputTypeUtils.Equals(styleInfo.InputType, EInputType.SelectMultiple))
            {
                retval = ParseSelectMultiple(attributeName, formCollection, isAddAndNotPostBack, styleInfo);
            }
            else if (EInputTypeUtils.Equals(styleInfo.InputType, EInputType.CheckBox))
            {
                retval = ParseCheckBox(attributeName, formCollection, isAddAndNotPostBack, styleInfo);
            }
            else if (EInputTypeUtils.Equals(styleInfo.InputType, EInputType.Radio))
            {
                retval = ParseRadio(attributeName, formCollection, isAddAndNotPostBack, styleInfo);
            }
            else if (EInputTypeUtils.Equals(styleInfo.InputType, EInputType.Date))
            {
                retval = ParseDate(publishmentSystemInfo, attributeName, formCollection, isAddAndNotPostBack, additionalAttributes, pageScripts, styleInfo);
            }
            else if (EInputTypeUtils.Equals(styleInfo.InputType, EInputType.DateTime))
            {
                retval = ParseDateTime(publishmentSystemInfo, attributeName, formCollection, isAddAndNotPostBack, additionalAttributes, pageScripts, styleInfo);
            }
            else if (EInputTypeUtils.Equals(styleInfo.InputType, EInputType.Image))
            {
                retval = !isValidate ? ParseImageUpload(attributeName, additionalAttributes, styleInfo) : ParseImageUploadForTouGao(formCollection, isAddAndNotPostBack, attributeName, styleInfo);
            }
            else if (EInputTypeUtils.Equals(styleInfo.InputType, EInputType.Video))
            {
                retval = !isValidate ? ParseVideoUpload(attributeName, additionalAttributes, styleInfo) : ParseVideoUploadForTouGao(formCollection, isAddAndNotPostBack, attributeName, styleInfo);
            }
            else if (EInputTypeUtils.Equals(styleInfo.InputType, EInputType.File))
            {
                retval = !isValidate ? ParseFileUpload(attributeName, additionalAttributes, styleInfo) : ParseFileUploadForTouGao(attributeName, formCollection, isAddAndNotPostBack, styleInfo);
            }
            else if (EInputTypeUtils.Equals(styleInfo.InputType, EInputType.RelatedField))
            {
                retval = ParseRelatedField(publishmentSystemInfo, attributeName, formCollection, styleInfo);
            }
            else if (EInputTypeUtils.Equals(styleInfo.InputType, EInputType.SpecifiedValue))
            {
                retval = ParseSpecifiedValue(publishmentSystemInfo, nodeId, tableStyle, attributeName, formCollection, isAddAndNotPostBack, styleInfo);
            }

            styleInfo.Additional.IsValidate = oriIsValidate;

            return retval;
        }

        public static string ParseText(PublishmentSystemInfo publishmentSystemInfo, int nodeId, string attributeName, NameValueCollection formCollection, bool isAddAndNotPostBack, string additionalAttributes, TableStyleInfo styleInfo, ETableStyle tableStyle)
        {
            var builder = new StringBuilder();

            var validateAttributes = InputParserUtils.GetValidateAttributes(styleInfo.Additional.IsValidate, styleInfo.DisplayName, styleInfo.Additional.IsRequired, styleInfo.Additional.MinNum, styleInfo.Additional.MaxNum, styleInfo.Additional.ValidateType, styleInfo.Additional.RegExp, styleInfo.Additional.ErrorMessage);

            var value = GetValue(attributeName, formCollection, isAddAndNotPostBack, styleInfo.DefaultValue);
            value = StringUtils.HtmlDecode(value);

            var width = styleInfo.Additional.Width;
            if (string.IsNullOrEmpty(width))
            {
                width = styleInfo.IsSingleLine ? "380px" : "220px";
            }
            string style = $@"style=""width:{TranslateUtils.ToWidth(width)};""";
            builder.Append(
                $@"<input id=""{attributeName}"" name=""{attributeName}"" type=""text"" class=""input_text"" value=""{value}"" {additionalAttributes} {style} {validateAttributes} />");

            AddHelpText(builder, styleInfo.HelpText);

            if (styleInfo.Additional.IsValidate)
            {
                builder.Append(
                    $@"&nbsp;<span id=""{styleInfo.AttributeName}_msg"" style=""color:red;display:none;"">*</span>");
                builder.Append(
                    $@"<script>event_observe('{styleInfo.AttributeName}', 'blur', checkAttributeValue);</script>");
            }

            return builder.ToString();
        }

        public static string ParseTextArea(string attributeName, NameValueCollection formCollection, bool isAddAndNotPostBack, string additionalAttributes, TableStyleInfo styleInfo)
        {
            var builder = new StringBuilder();

            var validateAttributes = InputParserUtils.GetValidateAttributes(styleInfo.Additional.IsValidate, styleInfo.DisplayName, styleInfo.Additional.IsRequired, styleInfo.Additional.MinNum, styleInfo.Additional.MaxNum, styleInfo.Additional.ValidateType, styleInfo.Additional.RegExp, styleInfo.Additional.ErrorMessage);

            var value = GetValue(attributeName, formCollection, isAddAndNotPostBack, styleInfo.DefaultValue);
            value = StringUtils.HtmlDecode(value);

            var width = styleInfo.Additional.Width;
            if (string.IsNullOrEmpty(width))
            {
                width = styleInfo.IsSingleLine ? "98%" : "220px";
            }
            var height = styleInfo.Additional.Height;
            if (height == 0)
            {
                height = 80;
            }
            string style = $@"style=""width:{TranslateUtils.ToWidth(width)};height:{height}px;""";

            builder.Append(
                $@"<textarea id=""{attributeName}"" name=""{attributeName}"" class=""textarea"" {additionalAttributes} {style} {validateAttributes}>{value}</textarea>");

            AddHelpText(builder, styleInfo.HelpText);

            if (styleInfo.Additional.IsValidate)
            {
                builder.Append(
                    $@"&nbsp;<span id=""{styleInfo.AttributeName}_msg"" style=""color:red;display:none;"">*</span>");
                builder.Append($@"
<script>event_observe('{styleInfo.AttributeName}', 'blur', checkAttributeValue);</script>
");
            }

            return builder.ToString();
        }

        private static string ParseTextEditor(PublishmentSystemInfo publishmentSystemInfo, string attributeName, NameValueCollection formCollection, bool isAddAndNotPostBack, NameValueCollection pageScripts, TableStyleInfo styleInfo)
        {
            return ParseTextEditor(publishmentSystemInfo, attributeName, formCollection, isAddAndNotPostBack, pageScripts, styleInfo.DefaultValue, styleInfo.Additional.Width, styleInfo.Additional.Height);
        }

        public static string ParseTextEditor(PublishmentSystemInfo publishmentSystemInfo, string attributeName, NameValueCollection formCollection, bool isAddAndNotPostBack, NameValueCollection pageScripts, string defaultValue, string width, int height)
        {
            var value = GetValue(attributeName, formCollection, isAddAndNotPostBack, defaultValue);

            /****获取编辑器中内容，解析@符号，添加了远程路径处理 20151103****/
            value = StringUtility.TextEditorContentDecode(value, publishmentSystemInfo);
            value = ETextEditorTypeUtils.TranslateToHtml(value);
            value = StringUtils.HtmlEncode(value);

            var builder = new StringBuilder();

            var controllerUrl = Controllers.Files.UEditor.GetUrl(publishmentSystemInfo.Additional.ApiUrl, publishmentSystemInfo.PublishmentSystemId);
            var editorUrl = SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, "ueditor");

            if (pageScripts["uEditor"] == null)
            {
                builder.Append(
                    $@"<script type=""text/javascript"">window.UEDITOR_HOME_URL = ""{editorUrl}/"";window.UEDITOR_CONTROLLER_URL = ""{controllerUrl}"";</script><script type=""text/javascript"" src=""{editorUrl}/editor_config.js""></script><script type=""text/javascript"" src=""{editorUrl}/ueditor_all_min.js""></script>");
            }
            pageScripts["uEditor"] = string.Empty;

            builder.Append($@"
<textarea id=""{attributeName}"" name=""{attributeName}"" style=""display:none"">{value}</textarea>
<script type=""text/javascript"">
$(function(){{
  UE.getEditor('{attributeName}', {{allowDivTransToP: false}});
  $('#{attributeName}').show();
}});
</script>");

            return builder.ToString();
        }

        private static string ParseDate(PublishmentSystemInfo publishmentSystemInfo, string attributeName, NameValueCollection formCollection, bool isAddAndNotPostBack, string additionalAttributes, NameValueCollection pageScripts, TableStyleInfo styleInfo)
        {
            var builder = new StringBuilder();

            var dateTime = DateUtils.SqlMinValue;
            if (isAddAndNotPostBack)
            {
                if (!string.IsNullOrEmpty(styleInfo.DefaultValue))
                {
                    dateTime = styleInfo.DefaultValue == Current ? DateTime.Now : TranslateUtils.ToDateTime(styleInfo.DefaultValue);
                }
            }
            else
            {
                if (formCollection?[attributeName] != null)
                {
                    dateTime = TranslateUtils.ToDateTime(formCollection[attributeName]);
                }
            }

            if (pageScripts != null)
            {
                pageScripts["calendar"] =
                    $@"<script language=""javascript"" src=""{SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, SiteFilesAssets.DatePicker.Js)}""></script>";
            }

            var value = string.Empty;
            if (dateTime > DateUtils.SqlMinValue)
            {
                value = DateUtils.GetDateString(dateTime);
            }

            builder.Append(
                $@"<input id=""{attributeName}"" name=""{attributeName}"" type=""text"" class=""input_text"" value=""{value}"" {additionalAttributes} onfocus=""{SiteFilesAssets.DatePicker.OnFocusDateOnly}"" />");

            AddHelpText(builder, styleInfo.HelpText);

            return builder.ToString();
        }

        private static string ParseDateTime(PublishmentSystemInfo publishmentSystemInfo, string attributeName, NameValueCollection formCollection, bool isAddAndNotPostBack, string additionalAttributes, NameValueCollection pageScripts, TableStyleInfo styleInfo)
        {
            var builder = new StringBuilder();

            var dateTime = DateUtils.SqlMinValue;
            if (isAddAndNotPostBack)
            {
                if (!string.IsNullOrEmpty(styleInfo.DefaultValue))
                {
                    dateTime = styleInfo.DefaultValue == Current ? DateTime.Now : TranslateUtils.ToDateTime(styleInfo.DefaultValue);
                }
            }
            else
            {
                if (formCollection?[attributeName] != null)
                {
                    dateTime = TranslateUtils.ToDateTime(formCollection[attributeName]);
                }
            }

            if (pageScripts != null)
            {
                pageScripts["calendar"] =
                    $@"<script type=""text/javascript"" src=""{SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, SiteFilesAssets.DatePicker.Js)}""></script>";
            }

            var value = string.Empty;
            if (dateTime > DateUtils.SqlMinValue)
            {
                value = DateUtils.GetDateAndTimeString(dateTime, EDateFormatType.Day, ETimeFormatType.LongTime);
            }

            builder.Append(
                $@"<input id=""{attributeName}"" name=""{attributeName}"" type=""text"" class=""input_text"" value=""{value}"" {additionalAttributes} onfocus=""{SiteFilesAssets.DatePicker.OnFocus}"" />");

            AddHelpText(builder, styleInfo.HelpText);

            return builder.ToString();
        }

        private static string ParseCheckBox(string attributeName, NameValueCollection formCollection, bool isAddAndNotPostBack, TableStyleInfo styleInfo)
        {
            var builder = new StringBuilder();

            var styleItems = styleInfo.StyleItems ?? BaiRongDataProvider.TableStyleDao.GetStyleItemInfoList(styleInfo.TableStyleId);

            var checkBoxList = new CheckBoxList
            {
                CssClass = "checkboxlist",
                ID = attributeName,
                RepeatDirection = styleInfo.IsHorizontal ? RepeatDirection.Horizontal : RepeatDirection.Vertical,
                RepeatColumns = styleInfo.Additional.Columns
            };
            var selectedValues = !string.IsNullOrEmpty(formCollection?[attributeName]) ? formCollection[attributeName] : string.Empty;
            var selectedValueArrayList = TranslateUtils.StringCollectionToStringList(selectedValues);

            //验证属性
            InputParserUtils.GetValidateAttributesForListItem(checkBoxList, styleInfo.Additional.IsValidate, styleInfo.DisplayName, styleInfo.Additional.IsRequired, styleInfo.Additional.MinNum, styleInfo.Additional.MaxNum, styleInfo.Additional.ValidateType, styleInfo.Additional.RegExp, styleInfo.Additional.ErrorMessage);

            foreach (TableStyleItemInfo styleItem in styleItems)
            {
                var isSelected = isAddAndNotPostBack ? styleItem.IsSelected : selectedValueArrayList.Contains(styleItem.ItemValue);
                var listItem = new ListItem(styleItem.ItemTitle, styleItem.ItemValue)
                {
                    Selected = isSelected
                };

                checkBoxList.Items.Add(listItem);
            }
            checkBoxList.Attributes.Add("isListItem", "true");
            builder.Append(ControlUtils.GetControlRenderHtml(checkBoxList));

            var i = 0;
            foreach (TableStyleItemInfo styleItem in styleItems)
            {
                builder.Replace($@"name=""{attributeName}${i}""",
                    $@"name=""{attributeName}"" value=""{styleItem.ItemValue}""");
                i++;
            }

            AddHelpText(builder, styleInfo.HelpText);

            if (styleInfo.Additional.IsValidate)
            {
                builder.Append(
                    $@"&nbsp;<span id=""{styleInfo.AttributeName}_msg"" style=""color:red;display:none;"">*</span>");
                builder.Append($@"
<script>event_observe('{styleInfo.AttributeName}', 'blur', checkAttributeValue);</script>
");
            }

            return builder.ToString();
        }

        private static string ParseRadio(string attributeName, NameValueCollection formCollection, bool isAddAndNotPostBack, TableStyleInfo styleInfo)
        {
            var builder = new StringBuilder();

            var styleItems = styleInfo.StyleItems ?? BaiRongDataProvider.TableStyleDao.GetStyleItemInfoList(styleInfo.TableStyleId);
            var radioButtonList = new RadioButtonList
            {
                CssClass = "radiobuttonlist",
                ID = attributeName,
                RepeatDirection = styleInfo.IsHorizontal ? RepeatDirection.Horizontal : RepeatDirection.Vertical,
                RepeatColumns = styleInfo.Additional.Columns
            };
            var selectedValue = !string.IsNullOrEmpty(formCollection?[attributeName]) ? formCollection[attributeName] : null;

            //验证属性
            InputParserUtils.GetValidateAttributesForListItem(radioButtonList, styleInfo.Additional.IsValidate, styleInfo.DisplayName, styleInfo.Additional.IsRequired, styleInfo.Additional.MinNum, styleInfo.Additional.MaxNum, styleInfo.Additional.ValidateType, styleInfo.Additional.RegExp, styleInfo.Additional.ErrorMessage);

            foreach (TableStyleItemInfo styleItem in styleItems)
            {
                bool isSelected;
                if (isAddAndNotPostBack)
                {
                    isSelected = styleItem.IsSelected;
                }
                else
                {
                    isSelected = (styleItem.ItemValue == selectedValue);
                }
                var listItem = new ListItem(styleItem.ItemTitle, styleItem.ItemValue)
                {
                    Selected = isSelected
                };
                listItem.Attributes.Add("class", "input_radio");
                radioButtonList.Items.Add(listItem);
            }
            radioButtonList.Attributes.Add("isListItem", "true");
            builder.Append(ControlUtils.GetControlRenderHtml(radioButtonList));

            AddHelpText(builder, styleInfo.HelpText);


            if (styleInfo.Additional.IsValidate)
            {
                builder.Append(
                    $@"&nbsp;<span id=""{styleInfo.AttributeName}_msg"" style=""color:red;display:none;"">*</span>");
                builder.Append($@"
<script>event_observe('{styleInfo.AttributeName}', 'blur', checkAttributeValue);</script>
");
            }

            return builder.ToString();
        }

        private static string ParseSelectOne(string attributeName, NameValueCollection formCollection, bool isAddAndNotPostBack, TableStyleInfo styleInfo)
        {
            var builder = new StringBuilder();

            var styleItems = styleInfo.StyleItems ?? BaiRongDataProvider.TableStyleDao.GetStyleItemInfoList(styleInfo.TableStyleId);

            var selectedValue = !string.IsNullOrEmpty(formCollection?[attributeName]) ? formCollection[attributeName] : null;
            //验证属性
            var validateAttributes = InputParserUtils.GetValidateAttributes(styleInfo.Additional.IsValidate, styleInfo.DisplayName, styleInfo.Additional.IsRequired, styleInfo.Additional.MinNum, styleInfo.Additional.MaxNum, styleInfo.Additional.ValidateType, styleInfo.Additional.RegExp, styleInfo.Additional.ErrorMessage);
            builder.Append(
                $@"<select id=""{attributeName}"" name=""{attributeName}"" class=""select""  isListItem=""true"" {validateAttributes}>");
            foreach (var styleItem in styleItems)
            {
                string isSelected;
                if (isAddAndNotPostBack)
                {
                    isSelected = styleItem.IsSelected ? "selected" : string.Empty;
                }
                else
                {
                    isSelected = (styleItem.ItemValue == selectedValue) ? "selected" : string.Empty;
                }

                builder.Append($@"<option value=""{styleItem.ItemValue}"" {isSelected}>{styleItem.ItemTitle}</option>");
            }
            builder.Append("</select>");

            AddHelpText(builder, styleInfo.HelpText);
            if (styleInfo.Additional.IsValidate)
            {
                builder.Append(
                    $@"&nbsp;<span id=""{styleInfo.AttributeName}_msg"" style=""color:red;display:none;"">*</span>");
                builder.Append($@"
<script>event_observe('{styleInfo.AttributeName}', 'blur', checkAttributeValue);</script>
");
            }
            return builder.ToString();
        }

        private static string ParseSelectMultiple(string attributeName, NameValueCollection formCollection, bool isAddAndNotPostBack, TableStyleInfo styleInfo)
        {
            var builder = new StringBuilder();

            var styleItems = styleInfo.StyleItems ?? BaiRongDataProvider.TableStyleDao.GetStyleItemInfoList(styleInfo.TableStyleId);

            var selectedValues = !string.IsNullOrEmpty(formCollection?[attributeName]) ? formCollection[attributeName] : string.Empty;
            var selectedValueArrayList = TranslateUtils.StringCollectionToStringList(selectedValues);
            //验证属性
            var validateAttributes = InputParserUtils.GetValidateAttributes(styleInfo.Additional.IsValidate, styleInfo.DisplayName, styleInfo.Additional.IsRequired, styleInfo.Additional.MinNum, styleInfo.Additional.MaxNum, styleInfo.Additional.ValidateType, styleInfo.Additional.RegExp, styleInfo.Additional.ErrorMessage);
            builder.Append(
                $@"<select id=""{attributeName}"" name=""{attributeName}"" class=""select_multiple""  isListItem=""true"" multiple  {validateAttributes}>");
            foreach (var styleItem in styleItems)
            {
                string isSelected;
                if (isAddAndNotPostBack)
                {
                    isSelected = styleItem.IsSelected ? "selected" : string.Empty;
                }
                else
                {
                    isSelected = (selectedValueArrayList.Contains(styleItem.ItemValue)) ? "selected" : string.Empty;
                }

                builder.Append($@"<option value=""{styleItem.ItemValue}"" {isSelected}>{styleItem.ItemTitle}</option>");
            }
            builder.Append("</select>");

            AddHelpText(builder, styleInfo.HelpText);
            if (styleInfo.Additional.IsValidate)
            {
                builder.Append(
                    $@"&nbsp;<span id=""{styleInfo.AttributeName}_msg"" style=""color:red;display:none;"">*</span>");
                builder.Append($@"
<script>event_observe('{styleInfo.AttributeName}', 'blur', checkAttributeValue);</script>
");
            }
            return builder.ToString();
        }

        public static string GetAttributeNameToUploadForTouGao(string attributeName)
        {
            return attributeName + "_uploader";
        }

        private static string ParseImageUpload(string attributeName, string additionalAttributes, TableStyleInfo styleInfo)
        {
            var builder = new StringBuilder();

            var validateAttributes = InputParserUtils.GetValidateAttributes(styleInfo.Additional.IsValidate, styleInfo.DisplayName, styleInfo.Additional.IsRequired, styleInfo.Additional.MinNum, styleInfo.Additional.MaxNum, styleInfo.Additional.ValidateType, styleInfo.Additional.RegExp, styleInfo.Additional.ErrorMessage);

            builder.Append(
                $@"<input id=""{attributeName}"" name=""{attributeName}"" type=""file"" class=""input_file"" {additionalAttributes} {validateAttributes} />");

            AddHelpText(builder, styleInfo.HelpText);

            if (styleInfo.Additional.IsValidate)
            {
                builder.Append(
                    $@"&nbsp;<span id=""{styleInfo.AttributeName}_msg"" style=""color:red;display:none;"">*</span>");
                builder.Append($@"
<script>event_observe('{styleInfo.AttributeName}', 'blur', checkAttributeValue);</script>
");
            }

            return builder.ToString();
        }

        private static string ParseImageUploadForTouGao(NameValueCollection formCollection, bool isAddAndNotPostBack, string attributeName, TableStyleInfo styleInfo)
        {
            var builder = new StringBuilder();

            var attributeNameToUpload = GetAttributeNameToUploadForTouGao(attributeName);
            var value = GetValue(attributeName, formCollection, isAddAndNotPostBack, string.Empty);

            builder.Append($@"
<table width=""100%"" border=""0"" cellspacing=""0"" cellpadding=""0"">
<tr>
    <td width=""400"">
        <input id=""{attributeName}"" name=""{attributeName}"" type=""text"" class=""input-txt"" value='{value}' style=""display:;width:320px"" />
        <input id=""{attributeNameToUpload}"" name=""{attributeNameToUpload}"" style=""width:320px;display:none"" type=""file"" class=""input-txt"" />
    </td>
</tr>
<tr>
    <td valign=""top"">
        <a id=""{attributeName}_link1"" style=""font-weight:bold"" href=""javascript:;"" onclick=""document.getElementById('{attributeName}_link2').style.fontWeight = '';this.style.fontWeight = 'bold';document.getElementById('{attributeName}').style.display = '';document.getElementById('{attributeNameToUpload}').style.display = 'none';"">输入 URL</a>
        &nbsp;&nbsp;&nbsp;&nbsp;
        <a id=""{attributeName}_link2"" href=""javascript:;"" onclick=""document.getElementById('{attributeName}_link1').style.fontWeight = '';this.style.fontWeight = 'bold';document.getElementById('{attributeName}').style.display = 'none';document.getElementById('{attributeNameToUpload}').style.display = '';"" >上传图片</a>
    </td>
</tr>
</table>");

            AddHelpText(builder, styleInfo.HelpText);

            if (styleInfo.Additional.IsValidate)
            {
                builder.Append(
                    $@"&nbsp;<span id=""{styleInfo.AttributeName}_msg"" style=""color:red;display:none;"">*</span>");
                builder.Append($@"
<script>event_observe('{styleInfo.AttributeName}', 'blur', checkAttributeValue);</script>
");
            }

            return builder.ToString();
        }

        private static string ParseVideoUpload(string attributeName, string additionalAttributes, TableStyleInfo styleInfo)
        {
            var builder = new StringBuilder();

            var validateAttributes = InputParserUtils.GetValidateAttributes(styleInfo.Additional.IsValidate, styleInfo.DisplayName, styleInfo.Additional.IsRequired, styleInfo.Additional.MinNum, styleInfo.Additional.MaxNum, styleInfo.Additional.ValidateType, styleInfo.Additional.RegExp, styleInfo.Additional.ErrorMessage);

            builder.Append(
                $@"<input id=""{attributeName}"" name=""{attributeName}"" type=""file"" class=""input_file"" {additionalAttributes} {validateAttributes} />");

            AddHelpText(builder, styleInfo.HelpText);

            if (styleInfo.Additional.IsValidate)
            {
                builder.Append(
                    $@"&nbsp;<span id=""{styleInfo.AttributeName}_msg"" style=""color:red;display:none;"">*</span>");
                builder.Append($@"
<script>event_observe('{styleInfo.AttributeName}', 'blur', checkAttributeValue);</script>
");
            }

            return builder.ToString();
        }

        private static string ParseVideoUploadForTouGao(NameValueCollection formCollection, bool isAddAndNotPostBack, string attributeName, TableStyleInfo styleInfo)
        {
            var builder = new StringBuilder();

            var attributeNameToUpload = GetAttributeNameToUploadForTouGao(attributeName);
            var value = GetValue(attributeName, formCollection, isAddAndNotPostBack, string.Empty);

            builder.Append($@"
<table width=""100%"" border=""0"" cellspacing=""0"" cellpadding=""0"">
<tr>
    <td width=""400"">
        <input id=""{attributeName}"" name=""{attributeName}"" type=""text"" class=""input-txt"" value='{value}' style=""display:;width:320px"" />
        <input id=""{attributeNameToUpload}"" name=""{attributeNameToUpload}"" style=""width:320px;display:none"" type=""file"" class=""input-txt"" />
    </td>
</tr>
<tr>
    <td valign=""top"">
        <a id=""{attributeName}_link1"" style=""font-weight:bold"" href=""javascript:;"" onclick=""document.getElementById('{attributeName}_link2').style.fontWeight = '';this.style.fontWeight = 'bold';document.getElementById('{attributeName}').style.display = '';document.getElementById('{attributeNameToUpload}').style.display = 'none';"">输入 URL</a>
        &nbsp;&nbsp;&nbsp;&nbsp;
        <a id=""{attributeName}_link2"" href=""javascript:;"" onclick=""document.getElementById('{attributeName}_link1').style.fontWeight = '';this.style.fontWeight = 'bold';document.getElementById('{attributeName}').style.display = 'none';document.getElementById('{attributeNameToUpload}').style.display = '';"" >上传视频</a>
    </td>
</tr>
</table>");

            AddHelpText(builder, styleInfo.HelpText);

            if (styleInfo.Additional.IsValidate)
            {
                builder.Append(
                    $@"&nbsp;<span id=""{styleInfo.AttributeName}_msg"" style=""color:red;display:none;"">*</span>");
                builder.Append($@"
<script>event_observe('{styleInfo.AttributeName}', 'blur', checkAttributeValue);</script>
");
            }

            return builder.ToString();
        }

        private static string ParseFileUpload(string attributeName, string additionalAttributes, TableStyleInfo styleInfo)
        {
            var builder = new StringBuilder();

            var validateAttributes = InputParserUtils.GetValidateAttributes(styleInfo.Additional.IsValidate, styleInfo.DisplayName, styleInfo.Additional.IsRequired, styleInfo.Additional.MinNum, styleInfo.Additional.MaxNum, styleInfo.Additional.ValidateType, styleInfo.Additional.RegExp, styleInfo.Additional.ErrorMessage);

            builder.Append(
                $@"<input id=""{attributeName}"" name=""{attributeName}"" type=""file"" class=""input_file"" {additionalAttributes} {validateAttributes} />");

            AddHelpText(builder, styleInfo.HelpText);

            if (styleInfo.Additional.IsValidate)
            {
                builder.Append(
                    $@"&nbsp;<span id=""{styleInfo.AttributeName}_msg"" style=""color:red;display:none;"">*</span>");
                builder.Append($@"
<script>event_observe('{styleInfo.AttributeName}', 'blur', checkAttributeValue);</script>
");
            }

            return builder.ToString();
        }

        private static string ParseFileUploadForTouGao(string attributeName, NameValueCollection formCollection, bool isAddAndNotPostBack, TableStyleInfo styleInfo)
        {
            var builder = new StringBuilder();

            var attributeNameToUpload = GetAttributeNameToUploadForTouGao(attributeName);
            var value = GetValue(attributeName, formCollection, isAddAndNotPostBack, string.Empty);

            builder.Append($@"
<table width=""100%"" border=""0"" cellspacing=""0"" cellpadding=""0"">
<tr>
    <td width=""400"">
        <input id=""{attributeName}"" name=""{attributeName}"" type=""text"" class=""input-txt"" value='{value}' style=""display:;width:320px"" />
        <input id=""{attributeNameToUpload}"" name=""{attributeNameToUpload}"" style=""width:320px;display:none"" type=""file"" class=""input-txt"" />
    </td>
</tr>
<tr>
    <td valign=""top"">
        <a id=""{attributeName}_link1"" style=""font-weight:bold"" href=""javascript:;"" onclick=""document.getElementById('{attributeName}_link2').style.fontWeight = '';this.style.fontWeight = 'bold';document.getElementById('{attributeName}').style.display = '';document.getElementById('{attributeNameToUpload}').style.display = 'none';"">输入 URL</a>
        &nbsp;&nbsp;&nbsp;&nbsp;
        <a id=""{attributeName}_link2"" href=""javascript:;"" onclick=""document.getElementById('{attributeName}_link1').style.fontWeight = '';this.style.fontWeight = 'bold';document.getElementById('{attributeName}').style.display = 'none';document.getElementById('{attributeNameToUpload}').style.display = '';"" >上传附件</a>
    </td>
</tr>
</table>");

            AddHelpText(builder, styleInfo.HelpText);

            if (styleInfo.Additional.IsValidate)
            {
                builder.Append(
                    $@"&nbsp;<span id=""{styleInfo.AttributeName}_msg"" style=""color:red;display:none;"">*</span>");
                builder.Append($@"
<script>event_observe('{styleInfo.AttributeName}', 'blur', checkAttributeValue);</script>
");
            }

            return builder.ToString();
        }

        private static string ParseRelatedField(PublishmentSystemInfo publishmentSystemInfo, string attributeName, NameValueCollection formCollection, TableStyleInfo styleInfo)
        {
            var builder = new StringBuilder();

            var fieldInfo = DataProvider.RelatedFieldDao.GetRelatedFieldInfo(styleInfo.Additional.RelatedFieldId);
            if (fieldInfo != null)
            {
                var list = DataProvider.RelatedFieldItemDao.GetRelatedFieldItemInfoList(styleInfo.Additional.RelatedFieldId, 0);

                var prefixes = TranslateUtils.StringCollectionToStringCollection(fieldInfo.Prefixes);
                var suffixes = TranslateUtils.StringCollectionToStringCollection(fieldInfo.Suffixes);

                var style = ERelatedFieldStyleUtils.GetEnumType(styleInfo.Additional.RelatedFieldStyle);

                builder.Append($@"
<span id=""c_{attributeName}_1"">{prefixes[0]}<select name=""{attributeName}"" id=""{attributeName}_1"" class=""select"" onchange=""getRelatedField_{fieldInfo.RelatedFieldID}(2);"">
	<option value="""">请选择</option>");

                var values = formCollection[attributeName];
                var value = string.Empty;
                if (!string.IsNullOrEmpty(values))
                {
                    value = values.Split(',')[0];
                }

                var isLoad = false;
                foreach (var itemInfo in list)
                {
                    var selected = (!string.IsNullOrEmpty(itemInfo.ItemValue) && value == itemInfo.ItemValue) ? @" selected=""selected""" : string.Empty;
                    if (!string.IsNullOrEmpty(selected)) isLoad = true;
                    builder.Append($@"
	<option value=""{itemInfo.ItemValue}"" itemID=""{itemInfo.ID}""{selected}>{itemInfo.ItemName}</option>");
                }

                builder.Append($@"
</select>{suffixes[0]}</span>");

                if (fieldInfo.TotalLevel > 1)
                {
                    for (var i = 2; i <= fieldInfo.TotalLevel; i++)
                    {
                        builder.Append($@"<span id=""c_{attributeName}_{i}"" style=""display:none"">");
                        builder.Append(style == ERelatedFieldStyle.Virtical ? @"<br />" : "&nbsp;");
                        builder.Append($@"
{prefixes[i - 1]}<select name=""{attributeName}"" id=""{attributeName}_{i}"" class=""select"" onchange=""getRelatedField_{fieldInfo.RelatedFieldID}({i} + 1);""></select>{suffixes[i - 1]}</span>
");
                    }
                }

                builder.Append($@"
<script>
function getRelatedField_{fieldInfo.RelatedFieldID}(level){{
    var attributeName = '{styleInfo.AttributeName}';
    var totalLevel = {fieldInfo.TotalLevel};
    for(i=level;i<=totalLevel;i++){{
        $('#c_' + attributeName + '_' + i).hide();
    }}
    var obj = $('#c_' + attributeName + '_' + (level - 1));
    var itemID = $('option:selected', obj).attr('itemID');
    if (itemID){{
        var url = '{Controllers.Stl.ActionsRelatedField.GetUrl(publishmentSystemInfo.Additional.ApiUrl, publishmentSystemInfo.PublishmentSystemId, styleInfo.Additional.RelatedFieldId, 0)}' + itemID;
        var values = '{values}';
        var value = (values) ? values.split(',')[level - 1] : '';
        $.post(url + '&callback=?', '', function(data, textStatus){{
            var $sel = $('#' + attributeName + '_' + level);
            $('option', $sel).each(function(){{
	            $(this).remove();
            }})
            $sel.append('<option value="""">请选择</option>');
            var show = false;
            var isLoad = false;
            $.each(data, function(i, item){{
                show = true;
                var selected = '';
                if (value == item.value){{
                    isLoad = true;
                    selected = ' selected=""selected""'
                }}
                $opt = $('<option value=""' + item.value + '"" itemID=""' + item.id + '""' + selected + '>' + item.name + '</option>');
                $opt.appendTo($sel);
            }});
            if (show) $('#c_' + attributeName + '_' + level).show();
            if (isLoad && level <= totalLevel){{
                getRelatedField_{fieldInfo.RelatedFieldID}(level + 1);
            }}
        }}, 'jsonp');
    }}
}}
");

                if (isLoad)
                {
                    builder.Append($@"
$(document).ready(function(){{
    getRelatedField_{fieldInfo.RelatedFieldID}(2);
}});
");
                }

                builder.Append("</script>");

                AddHelpText(builder, styleInfo.HelpText);
            }
            return builder.ToString();
        }

        private static string ParseSpecifiedValue(PublishmentSystemInfo publishmentSystemInfo, int nodeId, ETableStyle tableStyle, string attributeName, NameValueCollection formCollection, bool isAddAndNotPostBack, TableStyleInfo styleInfo)
        {
            if (tableStyle == ETableStyle.GovInteractContent)
            {
                if (StringUtils.EqualsIgnoreCase(attributeName, GovInteractContentAttribute.TypeId))
                {
                    styleInfo.StyleItems = new List<TableStyleItemInfo>();
                    var itemInfo = new TableStyleItemInfo(0, styleInfo.TableStyleId, "<<请选择>>", string.Empty, false);
                    styleInfo.StyleItems.Add(itemInfo);
                    var typeInfoArrayList = DataProvider.GovInteractTypeDao.GetTypeInfoArrayList(nodeId);
                    foreach (GovInteractTypeInfo typeInfo in typeInfoArrayList)
                    {
                        var isSelected = false;
                        if (!isAddAndNotPostBack)
                        {
                            isSelected = formCollection[attributeName] == typeInfo.TypeID.ToString();
                        }
                        itemInfo = new TableStyleItemInfo(0, styleInfo.TableStyleId, typeInfo.TypeName, typeInfo.TypeID.ToString(), isSelected);
                        styleInfo.StyleItems.Add(itemInfo);
                    }
                    return ParseSelectOne(attributeName, formCollection, isAddAndNotPostBack, styleInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(attributeName, GovInteractContentAttribute.DepartmentId))
                {
                    styleInfo.StyleItems = new List<TableStyleItemInfo>();
                    var itemInfo = new TableStyleItemInfo(0, styleInfo.TableStyleId, "<<请选择>>", string.Empty, false);
                    styleInfo.StyleItems.Add(itemInfo);
                    var channelInfo = DataProvider.GovInteractChannelDao.GetChannelInfo(publishmentSystemInfo.PublishmentSystemId, nodeId);
                    var departmentIdList = GovInteractManager.GetFirstDepartmentIdList(channelInfo);
                    foreach (var departmentId in departmentIdList)
                    {
                        var departmentInfo = DepartmentManager.GetDepartmentInfo(departmentId);
                        if (departmentInfo != null)
                        {
                            var isSelected = false;
                            if (!isAddAndNotPostBack)
                            {
                                isSelected = formCollection[attributeName] == departmentInfo.DepartmentId.ToString();
                            }
                            itemInfo = new TableStyleItemInfo(0, styleInfo.TableStyleId, departmentInfo.DepartmentName, departmentInfo.DepartmentId.ToString(), isSelected);
                            styleInfo.StyleItems.Add(itemInfo);
                        }
                    }
                    return ParseSelectOne(attributeName, formCollection, isAddAndNotPostBack, styleInfo);
                }
            }

            return string.Empty;
        }

        private static void AddHelpText(StringBuilder builder, string helpText)
        {
            if (!string.IsNullOrEmpty(helpText))
            {
                builder.Append($@"&nbsp;<span style=""color:#999"">{helpText}</span>");
            }
        }

        public static void AddValuesToAttributes(ETableStyle tableStyle, string tableName, PublishmentSystemInfo publishmentSystemInfo, List<int> relatedIdentities, Control containerControl, NameValueCollection attributes)
        {
            var styleInfoList = TableStyleManager.GetTableStyleInfoList(tableStyle, tableName, relatedIdentities);
            foreach (var styleInfo in styleInfoList)
            {
                if (styleInfo.IsVisible == false) continue;
                var theValue = GetValueByControl(styleInfo, publishmentSystemInfo, containerControl);

                if (!EInputTypeUtils.EqualsAny(styleInfo.InputType, EInputType.TextEditor, EInputType.Image, EInputType.File, EInputType.Video) && styleInfo.AttributeName != BackgroundContentAttribute.LinkUrl)
                {
                    theValue = PageUtils.FilterSqlAndXss(theValue);
                }

                ExtendedAttributes.SetExtendedAttribute(attributes, styleInfo.AttributeName, theValue);
            }

            //ArrayList metadataInfoArrayList = TableManager.GetTableMetadataInfoArrayList(tableName);
            //foreach (TableMetadataInfo metadataInfo in metadataInfoArrayList)
            //{
            //    if (!isSystemContained && metadataInfo.IsSystem == EBoolean.True) continue;

            //    TableStyleInfo styleInfo = TableStyleManager.GetTableStyleInfo(tableType, metadataInfo, relatedIdentities);
            //    if (styleInfo.IsVisible == EBoolean.False) continue;

            //    string theValue = InputTypeParser.GetValueByControl(metadataInfo, styleInfo, publishmentSystemInfo, containerControl);
            //    ExtendedAttributes.SetExtendedAttribute(attributes, metadataInfo.AttributeName, theValue);
            //}
        }

        public static void AddValuesToAttributes(ETableStyle tableStyle, string tableName, PublishmentSystemInfo publishmentSystemInfo, List<int> relatedIdentities, NameValueCollection formCollection, NameValueCollection attributes, List<string> dontAddAttributes, bool isSaveImage)
        {
            var styleInfoList = TableStyleManager.GetTableStyleInfoList(tableStyle, tableName, relatedIdentities);
            foreach (var styleInfo in styleInfoList)
            {
                if (styleInfo.IsVisible == false || dontAddAttributes.Contains(styleInfo.AttributeName.ToLower())) continue;
                var theValue = GetValueByForm(styleInfo, publishmentSystemInfo, formCollection, isSaveImage);

                if (!EInputTypeUtils.EqualsAny(styleInfo.InputType, EInputType.TextEditor, EInputType.Image, EInputType.File, EInputType.Video) && styleInfo.AttributeName != BackgroundContentAttribute.LinkUrl)
                {
                    theValue = PageUtils.FilterSqlAndXss(theValue);
                }

                ExtendedAttributes.SetExtendedAttribute(attributes, styleInfo.AttributeName, theValue);

                if (styleInfo.Additional.IsFormatString)
                {
                    var formatString = TranslateUtils.ToBool(formCollection[styleInfo.AttributeName + "_formatStrong"]);
                    var formatEm = TranslateUtils.ToBool(formCollection[styleInfo.AttributeName + "_formatEM"]);
                    var formatU = TranslateUtils.ToBool(formCollection[styleInfo.AttributeName + "_formatU"]);
                    var formatColor = formCollection[styleInfo.AttributeName + "_formatColor"];
                    var theFormatString = ContentUtility.GetTitleFormatString(formatString, formatEm, formatU, formatColor);

                    ExtendedAttributes.SetExtendedAttribute(attributes, ContentAttribute.GetFormatStringAttributeName(styleInfo.AttributeName), theFormatString);
                }

                if (EInputTypeUtils.EqualsAny(styleInfo.InputType, EInputType.Image, EInputType.Video, EInputType.File))
                {
                    var attributeName = ContentAttribute.GetExtendAttributeName(styleInfo.AttributeName);
                    ExtendedAttributes.SetExtendedAttribute(attributes, attributeName, formCollection[attributeName]);
                }
            }
        }

        public static void AddValuesToAttributes(ETableStyle tableStyle, string tableName, PublishmentSystemInfo publishmentSystemInfo, List<int> relatedIdentities, NameValueCollection formCollection, NameValueCollection attributes)
        {
            AddValuesToAttributes(tableStyle, tableName, publishmentSystemInfo, relatedIdentities, formCollection, attributes, true);
        }

        public static void AddValuesToAttributes(ETableStyle tableStyle, string tableName, PublishmentSystemInfo publishmentSystemInfo, List<int> relatedIdentities, NameValueCollection formCollection, NameValueCollection attributes, List<string> dontAddAttributes)
        {
            var styleInfoList = TableStyleManager.GetTableStyleInfoList(tableStyle, tableName, relatedIdentities);
            foreach (var styleInfo in styleInfoList)
            {
                if (styleInfo.IsVisible == false || dontAddAttributes.Contains(styleInfo.AttributeName.ToLower())) continue;
                var theValue = GetValueByForm(styleInfo, publishmentSystemInfo, formCollection);
                if (!EInputTypeUtils.EqualsAny(styleInfo.InputType, EInputType.TextEditor, EInputType.Image, EInputType.File, EInputType.Video) && styleInfo.AttributeName != BackgroundContentAttribute.LinkUrl)
                {
                    theValue = PageUtils.FilterSqlAndXss(theValue);
                }

                ExtendedAttributes.SetExtendedAttribute(attributes, styleInfo.AttributeName, theValue);

                if (styleInfo.Additional.IsFormatString)
                {
                    var formatString = TranslateUtils.ToBool(formCollection[styleInfo.AttributeName + "_formatStrong"]);
                    var formatEm = TranslateUtils.ToBool(formCollection[styleInfo.AttributeName + "_formatEM"]);
                    var formatU = TranslateUtils.ToBool(formCollection[styleInfo.AttributeName + "_formatU"]);
                    var formatColor = formCollection[styleInfo.AttributeName + "_formatColor"];
                    var theFormatString = ContentUtility.GetTitleFormatString(formatString, formatEm, formatU, formatColor);

                    ExtendedAttributes.SetExtendedAttribute(attributes, ContentAttribute.GetFormatStringAttributeName(styleInfo.AttributeName), theFormatString);
                }

                if (EInputTypeUtils.EqualsAny(styleInfo.InputType, EInputType.Image, EInputType.Video, EInputType.File))
                {
                    var attributeName = ContentAttribute.GetExtendAttributeName(styleInfo.AttributeName);
                    ExtendedAttributes.SetExtendedAttribute(attributes, attributeName, formCollection[attributeName]);
                }
            }
        }

        public static void AddValuesToAttributes(ETableStyle tableStyle, string tableName, PublishmentSystemInfo publishmentSystemInfo, List<int> relatedIdentities, NameValueCollection formCollection, NameValueCollection attributes, bool isBackground)
        {
            var styleInfoList = TableStyleManager.GetTableStyleInfoList(tableStyle, tableName, relatedIdentities);
            foreach (var styleInfo in styleInfoList)
            {
                if (styleInfo.IsVisible == false) continue;
                var theValue = GetValueByForm(styleInfo, publishmentSystemInfo, formCollection);

                if (!EInputTypeUtils.EqualsAny(styleInfo.InputType, EInputType.TextEditor, EInputType.Image, EInputType.File, EInputType.Video) && styleInfo.AttributeName != BackgroundContentAttribute.LinkUrl)
                {
                    theValue = PageUtils.FilterSqlAndXss(theValue);
                }

                ExtendedAttributes.SetExtendedAttribute(attributes, styleInfo.AttributeName, theValue);
            }
        }

        public static void AddSingleValueToAttributes(ETableStyle tableStyle, string tableName, PublishmentSystemInfo publishmentSystemInfo, List<int> relatedIdentities, NameValueCollection formCollection, string attributeName, NameValueCollection attributes, bool isSystemContained)
        {
            var metadataInfo = TableManager.GetTableMetadataInfo(tableName, attributeName);
            if (!isSystemContained && metadataInfo.IsSystem) return;

            var styleInfo = TableStyleManager.GetTableStyleInfo(tableStyle, tableName, attributeName, relatedIdentities);
            if (styleInfo.IsVisible == false) return;

            var theValue = GetValueByForm(styleInfo, publishmentSystemInfo, formCollection);

            ExtendedAttributes.SetExtendedAttribute(attributes, metadataInfo.AttributeName, theValue);
        }

        private static string GetValueByControl(TableStyleInfo styleInfo, PublishmentSystemInfo publishmentSystemInfo, Control containerControl)
        {
            var theValue = ControlUtils.GetInputValue(containerControl, styleInfo.AttributeName) ?? string.Empty;

            var inputType = EInputTypeUtils.GetEnumType(styleInfo.InputType);

            if (inputType == EInputType.TextEditor)
            {
                theValue = StringUtility.TextEditorContentEncode(theValue, publishmentSystemInfo);
            }

            return theValue;
        }

        private static string GetValue(string attributeName, NameValueCollection formCollection, bool isAddAndNotPostBack, string defaultValue)
        {
            var value = string.Empty;
            if (formCollection?[attributeName] != null)
            {
                value = formCollection[attributeName];
            }
            if (isAddAndNotPostBack && string.IsNullOrEmpty(value))
            {
                value = defaultValue;
            }
            return value;
        }

        private static string GetValueByForm(TableStyleInfo styleInfo, PublishmentSystemInfo publishmentSystemInfo, NameValueCollection formCollection)
        {
            var theValue = formCollection[styleInfo.AttributeName] ?? string.Empty;

            var inputType = EInputTypeUtils.GetEnumType(styleInfo.InputType);

            if (inputType == EInputType.TextEditor)
            {
                theValue = StringUtility.TextEditorContentEncode(theValue, publishmentSystemInfo);
                theValue = ETextEditorTypeUtils.TranslateToStlElement(theValue);
            }

            return theValue;
        }

        private static string GetValueByForm(TableStyleInfo styleInfo, PublishmentSystemInfo publishmentSystemInfo, NameValueCollection formCollection, bool isSaveImage)
        {
            var theValue = formCollection[styleInfo.AttributeName] ?? string.Empty;

            var inputType = EInputTypeUtils.GetEnumType(styleInfo.InputType);

            if (inputType == EInputType.TextEditor)
            {
                theValue = StringUtility.TextEditorContentEncode(theValue, publishmentSystemInfo, isSaveImage && publishmentSystemInfo.Additional.IsSaveImageInTextEditor);
                theValue = ETextEditorTypeUtils.TranslateToStlElement(theValue);
            }

            return theValue;
        }
    }
}
