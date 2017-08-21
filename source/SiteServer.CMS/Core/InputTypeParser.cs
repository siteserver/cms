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
using SiteServer.Plugin;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Core
{
    public class InputTypeParser
    {
        private InputTypeParser()
        {
        }

        public const string Current = "{Current}";

        public static string Parse(PublishmentSystemInfo publishmentSystemInfo, TableStyleInfo styleInfo, string attributeName, NameValueCollection pageScripts)
        {
            var retval = string.Empty;

            if (InputTypeUtils.Equals(styleInfo.InputType, InputType.Text))
            {
                retval = ParseText(attributeName, styleInfo);
            }
            else if (InputTypeUtils.Equals(styleInfo.InputType, InputType.TextArea))
            {
                retval = ParseTextArea(attributeName, styleInfo);
            }
            else if (InputTypeUtils.Equals(styleInfo.InputType, InputType.TextEditor))
            {
                retval = ParseTextEditor(publishmentSystemInfo, attributeName, pageScripts, styleInfo);
            }
            else if (InputTypeUtils.Equals(styleInfo.InputType, InputType.SelectOne))
            {
                retval = ParseSelectOne(attributeName, styleInfo);
            }
            else if (InputTypeUtils.Equals(styleInfo.InputType, InputType.SelectMultiple))
            {
                retval = ParseSelectMultiple(attributeName, styleInfo);
            }
            else if (InputTypeUtils.Equals(styleInfo.InputType, InputType.CheckBox))
            {
                retval = ParseCheckBox(attributeName, styleInfo);
            }
            else if (InputTypeUtils.Equals(styleInfo.InputType, InputType.Radio))
            {
                retval = ParseRadio(attributeName, styleInfo);
            }
            else if (InputTypeUtils.Equals(styleInfo.InputType, InputType.Date))
            {
                retval = ParseDate(publishmentSystemInfo, attributeName, pageScripts, styleInfo);
            }
            else if (InputTypeUtils.Equals(styleInfo.InputType, InputType.DateTime))
            {
                retval = ParseDateTime(publishmentSystemInfo, attributeName, pageScripts, styleInfo);
            }
            else if (InputTypeUtils.Equals(styleInfo.InputType, InputType.Image))
            {
                retval = ParseImageUpload(attributeName, styleInfo);
            }
            else if (InputTypeUtils.Equals(styleInfo.InputType, InputType.Video))
            {
                retval = ParseVideoUpload(attributeName, styleInfo);
            }
            else if (InputTypeUtils.Equals(styleInfo.InputType, InputType.File))
            {
                retval = ParseFileUpload(attributeName, styleInfo);
            }
            else if (InputTypeUtils.Equals(styleInfo.InputType, InputType.RelatedField))
            {
                retval = ParseRelatedField(publishmentSystemInfo, attributeName, styleInfo);
            }

            return retval;
        }

        public static string ParseText(string attributeName, TableStyleInfo styleInfo)
        {
            var builder = new StringBuilder();

            var value = styleInfo.DefaultValue ?? string.Empty;
            value = StringUtils.HtmlDecode(value);

            builder.Append(
                $@"<input id=""{attributeName}"" type=""text"" value=""{value}"" />");

            AddHelpText(builder, styleInfo.HelpText);

            return builder.ToString();
        }

        public static string ParseTextArea(string attributeName, TableStyleInfo styleInfo)
        {
            var builder = new StringBuilder();

            var value = styleInfo.DefaultValue ?? string.Empty;
            value = StringUtils.HtmlDecode(value);

            builder.Append(
                $@"<textarea id=""{attributeName}"">{value}</textarea>");

            AddHelpText(builder, styleInfo.HelpText);

            return builder.ToString();
        }

        private static string ParseTextEditor(PublishmentSystemInfo publishmentSystemInfo, string attributeName, NameValueCollection pageScripts, TableStyleInfo styleInfo)
        {
            return ParseTextEditor(publishmentSystemInfo, attributeName, pageScripts, styleInfo.DefaultValue, styleInfo.Additional.Width, styleInfo.Additional.Height);
        }

        public static string ParseTextEditor(PublishmentSystemInfo publishmentSystemInfo, string attributeName, NameValueCollection pageScripts, string defaultValue, string width, int height)
        {
            var value = defaultValue ?? string.Empty;

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

        private static string ParseDate(PublishmentSystemInfo publishmentSystemInfo, string attributeName, NameValueCollection pageScripts, TableStyleInfo styleInfo)
        {
            var builder = new StringBuilder();

            var dateTime = DateUtils.SqlMinValue;
            if (!string.IsNullOrEmpty(styleInfo.DefaultValue))
            {
                dateTime = styleInfo.DefaultValue == Current ? DateTime.Now : TranslateUtils.ToDateTime(styleInfo.DefaultValue);
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
                $@"<input id=""{attributeName}"" type=""text"" value=""{value}"" onfocus=""{SiteFilesAssets.DatePicker.OnFocusDateOnly}"" />");

            AddHelpText(builder, styleInfo.HelpText);

            return builder.ToString();
        }

        private static string ParseDateTime(PublishmentSystemInfo publishmentSystemInfo, string attributeName, NameValueCollection pageScripts, TableStyleInfo styleInfo)
        {
            var builder = new StringBuilder();

            var dateTime = DateUtils.SqlMinValue;
            if (!string.IsNullOrEmpty(styleInfo.DefaultValue))
            {
                dateTime = styleInfo.DefaultValue == Current ? DateTime.Now : TranslateUtils.ToDateTime(styleInfo.DefaultValue);
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
                $@"<input id=""{attributeName}"" type=""text"" value=""{value}"" onfocus=""{SiteFilesAssets.DatePicker.OnFocus}"" />");

            AddHelpText(builder, styleInfo.HelpText);

            return builder.ToString();
        }

        private static string ParseCheckBox(string attributeName, TableStyleInfo styleInfo)
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

            foreach (var styleItem in styleItems)
            {
                var isSelected = styleItem.IsSelected;
                var listItem = new ListItem(styleItem.ItemTitle, styleItem.ItemValue)
                {
                    Selected = isSelected
                };

                checkBoxList.Items.Add(listItem);
            }
            checkBoxList.Attributes.Add("isListItem", "true");
            builder.Append(ControlUtils.GetControlRenderHtml(checkBoxList));

            var i = 0;
            foreach (var styleItem in styleItems)
            {
                builder.Replace($@"name=""{attributeName}${i}""",
                    $@"name=""{attributeName}"" value=""{styleItem.ItemValue}""");
                i++;
            }

            AddHelpText(builder, styleInfo.HelpText);

            return builder.ToString();
        }

        private static string ParseRadio(string attributeName, TableStyleInfo styleInfo)
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

            foreach (var styleItem in styleItems)
            {
                bool isSelected = styleItem.IsSelected;
                var listItem = new ListItem(styleItem.ItemTitle, styleItem.ItemValue)
                {
                    Selected = isSelected
                };
                radioButtonList.Items.Add(listItem);
            }
            radioButtonList.Attributes.Add("isListItem", "true");
            builder.Append(ControlUtils.GetControlRenderHtml(radioButtonList));

            AddHelpText(builder, styleInfo.HelpText);

            return builder.ToString();
        }

        private static string ParseSelectOne(string attributeName, TableStyleInfo styleInfo)
        {
            var builder = new StringBuilder();

            var styleItems = styleInfo.StyleItems ?? BaiRongDataProvider.TableStyleDao.GetStyleItemInfoList(styleInfo.TableStyleId);

            builder.Append(
                $@"<select id=""{attributeName}"">");
            foreach (var styleItem in styleItems)
            {
                var isSelected = styleItem.IsSelected ? "selected" : string.Empty;

                builder.Append($@"<option value=""{styleItem.ItemValue}"" {isSelected}>{styleItem.ItemTitle}</option>");
            }
            builder.Append("</select>");

            AddHelpText(builder, styleInfo.HelpText);

            return builder.ToString();
        }

        private static string ParseSelectMultiple(string attributeName, TableStyleInfo styleInfo)
        {
            var builder = new StringBuilder();

            var styleItems = styleInfo.StyleItems ?? BaiRongDataProvider.TableStyleDao.GetStyleItemInfoList(styleInfo.TableStyleId);

            builder.Append(
                $@"<select id=""{attributeName}"" multiple>");
            foreach (var styleItem in styleItems)
            {
                string isSelected = styleItem.IsSelected ? "selected" : string.Empty;

                builder.Append($@"<option value=""{styleItem.ItemValue}"" {isSelected}>{styleItem.ItemTitle}</option>");
            }
            builder.Append("</select>");

            AddHelpText(builder, styleInfo.HelpText);
            return builder.ToString();
        }

        public static string GetAttributeNameToUploadForTouGao(string attributeName)
        {
            return attributeName + "_uploader";
        }

        private static string ParseImageUpload(string attributeName, TableStyleInfo styleInfo)
        {
            var builder = new StringBuilder();

            builder.Append(
                $@"<input id=""{attributeName}"" type=""file"" />");

            AddHelpText(builder, styleInfo.HelpText);

            return builder.ToString();
        }

        private static string ParseVideoUpload(string attributeName, TableStyleInfo styleInfo)
        {
            var builder = new StringBuilder();

            builder.Append(
                $@"<input id=""{attributeName}"" type=""file"" />");

            AddHelpText(builder, styleInfo.HelpText);

            return builder.ToString();
        }

        private static string ParseFileUpload(string attributeName, TableStyleInfo styleInfo)
        {
            var builder = new StringBuilder();

            builder.Append(
                $@"<input id=""{attributeName}"" type=""file"" />");

            AddHelpText(builder, styleInfo.HelpText);

            return builder.ToString();
        }

        private static string ParseRelatedField(PublishmentSystemInfo publishmentSystemInfo, string attributeName, TableStyleInfo styleInfo)
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
<span id=""c_{attributeName}_1"">{prefixes[0]}<select name=""{attributeName}"" id=""{attributeName}_1"" onchange=""getRelatedField_{fieldInfo.RelatedFieldID}(2);"">
	<option value="""">请选择</option>");

                var value = string.Empty;

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
{prefixes[i - 1]}<select name=""{attributeName}"" id=""{attributeName}_{i}"" onchange=""getRelatedField_{fieldInfo.RelatedFieldID}({i} + 1);""></select>{suffixes[i - 1]}</span>
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
        var values = '';
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

                if (!InputTypeUtils.EqualsAny(styleInfo.InputType, InputType.TextEditor, InputType.Image, InputType.File, InputType.Video) && styleInfo.AttributeName != BackgroundContentAttribute.LinkUrl)
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

                if (!InputTypeUtils.EqualsAny(styleInfo.InputType, InputType.TextEditor, InputType.Image, InputType.File, InputType.Video) && styleInfo.AttributeName != BackgroundContentAttribute.LinkUrl)
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

                if (InputTypeUtils.EqualsAny(styleInfo.InputType, InputType.Image, InputType.Video, InputType.File))
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
                if (!InputTypeUtils.EqualsAny(styleInfo.InputType, InputType.TextEditor, InputType.Image, InputType.File, InputType.Video) && styleInfo.AttributeName != BackgroundContentAttribute.LinkUrl)
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

                if (InputTypeUtils.EqualsAny(styleInfo.InputType, InputType.Image, InputType.Video, InputType.File))
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

                if (!InputTypeUtils.EqualsAny(styleInfo.InputType, InputType.TextEditor, InputType.Image, InputType.File, InputType.Video) && styleInfo.AttributeName != BackgroundContentAttribute.LinkUrl)
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

            var inputType = InputTypeUtils.GetEnumType(styleInfo.InputType);

            if (inputType == InputType.TextEditor)
            {
                theValue = StringUtility.TextEditorContentEncode(theValue, publishmentSystemInfo);
            }

            return theValue;
        }

        private static string GetValueByForm(TableStyleInfo styleInfo, PublishmentSystemInfo publishmentSystemInfo, NameValueCollection formCollection)
        {
            var theValue = formCollection[styleInfo.AttributeName] ?? string.Empty;

            var inputType = InputTypeUtils.GetEnumType(styleInfo.InputType);

            if (inputType == InputType.TextEditor)
            {
                theValue = StringUtility.TextEditorContentEncode(theValue, publishmentSystemInfo);
                theValue = ETextEditorTypeUtils.TranslateToStlElement(theValue);
            }

            return theValue;
        }

        private static string GetValueByForm(TableStyleInfo styleInfo, PublishmentSystemInfo publishmentSystemInfo, NameValueCollection formCollection, bool isSaveImage)
        {
            var theValue = formCollection[styleInfo.AttributeName] ?? string.Empty;

            var inputType = InputTypeUtils.GetEnumType(styleInfo.InputType);

            if (inputType == InputType.TextEditor)
            {
                theValue = StringUtility.TextEditorContentEncode(theValue, publishmentSystemInfo, isSaveImage && publishmentSystemInfo.Additional.IsSaveImageInTextEditor);
                theValue = ETextEditorTypeUtils.TranslateToStlElement(theValue);
            }

            return theValue;
        }
    }
}
