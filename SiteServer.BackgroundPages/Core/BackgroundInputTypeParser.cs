using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Ajax;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Controllers.Sys.Editors;
using SiteServer.CMS.Controllers.Sys.Stl;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.Plugin.Models;

namespace SiteServer.BackgroundPages.Core
{
    public class BackgroundInputTypeParser
    {
        private BackgroundInputTypeParser()
        {
        }

        public const string Current = "{Current}";

        public static string Parse(PublishmentSystemInfo publishmentSystemInfo, int nodeId, TableStyleInfo styleInfo, ETableStyle tableStyle, string attributeName, NameValueCollection formCollection, bool isEdit, bool isPostBack, string additionalAttributes, NameValueCollection pageScripts, out string extraHtml)
        {
            var retval = string.Empty;
            var extraBuider = new StringBuilder();
            if (!string.IsNullOrEmpty(styleInfo.HelpText))
            {
                extraBuider.Append($@"<span class=""help-block"">{styleInfo.HelpText}</span>");
            }

            var isAddAndNotPostBack = !isEdit && !isPostBack;

            if (InputTypeUtils.Equals(styleInfo.InputType, InputType.Text))
            {
                retval = ParseText(publishmentSystemInfo, nodeId, attributeName, formCollection, isAddAndNotPostBack, additionalAttributes, styleInfo, tableStyle);
            }
            else if (InputTypeUtils.Equals(styleInfo.InputType, InputType.TextArea))
            {
                retval = ParseTextArea(attributeName, formCollection, isAddAndNotPostBack, additionalAttributes, styleInfo);
            }
            else if (InputTypeUtils.Equals(styleInfo.InputType, InputType.TextEditor))
            {
                retval = ParseTextEditor(publishmentSystemInfo, attributeName, formCollection, isAddAndNotPostBack, pageScripts, styleInfo);
            }
            else if (InputTypeUtils.Equals(styleInfo.InputType, InputType.SelectOne))
            {
                retval = ParseSelectOne(attributeName, formCollection, isAddAndNotPostBack, styleInfo);
            }
            else if (InputTypeUtils.Equals(styleInfo.InputType, InputType.SelectMultiple))
            {
                retval = ParseSelectMultiple(attributeName, formCollection, isAddAndNotPostBack, styleInfo);
            }
            else if (InputTypeUtils.Equals(styleInfo.InputType, InputType.CheckBox))
            {
                retval = ParseCheckBox(attributeName, formCollection, isAddAndNotPostBack, styleInfo);
            }
            else if (InputTypeUtils.Equals(styleInfo.InputType, InputType.Radio))
            {
                retval = ParseRadio(attributeName, formCollection, isAddAndNotPostBack, styleInfo);
            }
            else if (InputTypeUtils.Equals(styleInfo.InputType, InputType.Date))
            {
                retval = ParseDate(attributeName, formCollection, isAddAndNotPostBack, additionalAttributes, pageScripts, styleInfo);
            }
            else if (InputTypeUtils.Equals(styleInfo.InputType, InputType.DateTime))
            {
                retval = ParseDateTime(attributeName, formCollection, isAddAndNotPostBack, additionalAttributes, pageScripts, styleInfo);
            }
            else if (InputTypeUtils.Equals(styleInfo.InputType, InputType.Image))
            {
                retval = ParseImage(publishmentSystemInfo, attributeName, formCollection, isAddAndNotPostBack, additionalAttributes, tableStyle, extraBuider);
            }
            else if (InputTypeUtils.Equals(styleInfo.InputType, InputType.Video))
            {
                retval = ParseVideo(publishmentSystemInfo, attributeName, formCollection, isAddAndNotPostBack, additionalAttributes, tableStyle, extraBuider);
            }
            else if (InputTypeUtils.Equals(styleInfo.InputType, InputType.File))
            {
                retval = ParseFile(publishmentSystemInfo, attributeName, formCollection, isAddAndNotPostBack, additionalAttributes, tableStyle, extraBuider);
            }
            else if (InputTypeUtils.Equals(styleInfo.InputType, InputType.RelatedField))
            {
                retval = ParseRelatedField(publishmentSystemInfo, attributeName, formCollection, styleInfo);
            }

            extraHtml = extraBuider.ToString();
            return retval;
        }

        public static string ParseText(PublishmentSystemInfo publishmentSystemInfo, int nodeId, string attributeName, NameValueCollection formCollection, bool isAddAndNotPostBack, string additionalAttributes, TableStyleInfo styleInfo, ETableStyle tableStyle)
        {
            var builder = new StringBuilder();

            var validateAttributes = InputParserUtils.GetValidateAttributes(styleInfo.Additional.IsValidate, styleInfo.DisplayName, styleInfo.Additional.IsRequired, styleInfo.Additional.MinNum, styleInfo.Additional.MaxNum, styleInfo.Additional.ValidateType, styleInfo.Additional.RegExp, styleInfo.Additional.ErrorMessage);

            var value = GetValue(attributeName, formCollection, isAddAndNotPostBack, styleInfo.DefaultValue);
            value = StringUtils.HtmlDecode(value);

            builder.Append(
                $@"<input id=""{attributeName}"" name=""{attributeName}"" type=""text"" class=""form-control"" value=""{value}"" {additionalAttributes} {validateAttributes} />");

            if (styleInfo.Additional.IsValidate)
            {
                builder.Append(
                    $@"&nbsp;<span id=""{styleInfo.AttributeName}_msg"" style=""color:red;display:none;"">*</span>");
                builder.Append(
                    $@"<script>event_observe('{styleInfo.AttributeName}', 'blur', checkAttributeValue);</script>");
            }

            if (styleInfo.Additional.IsFormatString)
            {
                var formatStrong = false;
                var formatEm = false;
                var formatU = false;
                var formatColor = string.Empty;
                if (!isAddAndNotPostBack)
                {
                    if (formCollection?[ContentAttribute.GetFormatStringAttributeName(attributeName)] != null)
                    {
                        ContentUtility.SetTitleFormatControls(formCollection[ContentAttribute.GetFormatStringAttributeName(attributeName)], out formatStrong, out formatEm, out formatU, out formatColor);
                    }
                }

                builder.Append(
                    $@"<a class=""btn"" href=""javascript:;"" onclick=""$('#div_{styleInfo.AttributeName}').toggle();return false;""><i class=""icon-text-height""></i></a>
<script type=""text/javascript"">
function {styleInfo.AttributeName}_strong(e){{
var e = $(e);
if ($('#{styleInfo.AttributeName}_formatStrong').val() == 'true'){{
$('#{styleInfo.AttributeName}_formatStrong').val('false');
e.removeClass('btn-success');
}}else{{
$('#{styleInfo.AttributeName}_formatStrong').val('true');
e.addClass('btn-success');
}}
}}
function {styleInfo.AttributeName}_em(e){{
var e = $(e);
if ($('#{styleInfo.AttributeName}_formatEM').val() == 'true'){{
$('#{styleInfo.AttributeName}_formatEM').val('false');
e.removeClass('btn-success');
}}else{{
$('#{styleInfo.AttributeName}_formatEM').val('true');
e.addClass('btn-success');
}}
}}
function {styleInfo.AttributeName}_u(e){{
var e = $(e);
if ($('#{styleInfo.AttributeName}_formatU').val() == 'true'){{
$('#{styleInfo.AttributeName}_formatU').val('false');
e.removeClass('btn-success');
}}else{{
$('#{styleInfo.AttributeName}_formatU').val('true');
e.addClass('btn-success');
}}
}}
function {styleInfo.AttributeName}_color(){{
if ($('#{styleInfo.AttributeName}_formatColor').val()){{
$('#{styleInfo.AttributeName}_colorBtn').css('color', $('#{styleInfo.AttributeName}_formatColor').val());
$('#{styleInfo.AttributeName}_colorBtn').addClass('btn-success');
}}else{{
$('#{styleInfo.AttributeName}_colorBtn').css('color', '');
$('#{styleInfo.AttributeName}_colorBtn').removeClass('btn-success');
}}
$('#{styleInfo.AttributeName}_colorContainer').hide();
}}
</script>
");

                builder.Append($@"
<div class=""btn-group"" style=""float:left;"">
    <button class=""btn{(formatStrong ? @" btn-success" : string.Empty)}"" style=""font-weight:bold;font-size:12px;"" onclick=""{styleInfo
                    .AttributeName}_strong(this);return false;"">粗体</button>
    <button class=""btn{(formatEm ? " btn-success" : string.Empty)}"" style=""font-style:italic;font-size:12px;"" onclick=""{styleInfo
                    .AttributeName}_em(this);return false;"">斜体</button>
    <button class=""btn{(formatU ? " btn-success" : string.Empty)}"" style=""text-decoration:underline;font-size:12px;"" onclick=""{styleInfo
                    .AttributeName}_u(this);return false;"">下划线</button>
    <button class=""btn{(!string.IsNullOrEmpty(formatColor) ? " btn-success" : string.Empty)}"" style=""font-size:12px;"" id=""{styleInfo
                    .AttributeName}_colorBtn"" onclick=""$('#{styleInfo.AttributeName}_colorContainer').toggle();return false;"">颜色</button>
</div>
<div id=""{styleInfo.AttributeName}_colorContainer"" class=""input-append"" style=""float:left;display:none"">
    <input id=""{styleInfo.AttributeName}_formatColor"" name=""{styleInfo.AttributeName}_formatColor"" class=""input-mini color {{required:false}}"" type=""text"" value=""{formatColor}"" placeholder=""颜色值"">
    <button class=""btn"" type=""button"" onclick=""Title_color();return false;"">确定</button>
</div>
<input id=""{styleInfo.AttributeName}_formatStrong"" name=""{styleInfo.AttributeName}_formatStrong"" type=""hidden"" value=""{formatStrong
                    .ToString().ToLower()}"" />
<input id=""{styleInfo.AttributeName}_formatEM"" name=""{styleInfo.AttributeName}_formatEM"" type=""hidden"" value=""{formatEm
                    .ToString().ToLower()}"" />
<input id=""{styleInfo.AttributeName}_formatU"" name=""{styleInfo.AttributeName}_formatU"" type=""hidden"" value=""{formatU
                    .ToString().ToLower()}"" />
");
            }

            if (nodeId > 0 && tableStyle == ETableStyle.BackgroundContent && styleInfo.AttributeName == ContentAttribute.Title)
            {
                builder.Append(@"
<script type=""text/javascript"">
function getTitles(title){
	$.get('[url]&title=' + encodeURIComponent(title) + '&channelID=' + $('#channelID').val() + '&r=' + Math.random(), function(data) {
		if(data !=''){
			var arr = data.split('|');
			var temp='';
			for(i=0;i<arr.length;i++)
			{
				temp += '<li><a>'+arr[i].replace(title,'<b>' + title + '</b>') + '</a></li>';
			}
			var myli='<ul>'+temp+'</ul>';
			$('#titleTips').html(myli);
			$('#titleTips').show();
		}else{
            $('#titleTips').hide();
        }
		$('#titleTips li').click(function () {
			$('#Title').val($(this).text());
			$('#titleTips').hide();
		})
	});	
}
$(document).ready(function () {
$('#Title').keyup(function (e) {
    if (e.keyCode != 40 && e.keyCode != 38) {
        var title = $('#Title').val();
        if (title != ''){
            window.setTimeout(""getTitles('"" + title + ""');"", 200);
        }else{
            $('#titleTips').hide();
        }
    }
}).blur(function () {
	window.setTimeout(""$('#titleTips').hide();"", 200);
})});
</script>
<div id=""titleTips"" class=""inputTips""></div>");
                builder.Replace("[url]", AjaxCmsService.GetTitlesUrl(publishmentSystemInfo.PublishmentSystemId, nodeId));
            }

            return builder.ToString();
        }

        public static string ParseTextArea(string attributeName, NameValueCollection formCollection, bool isAddAndNotPostBack, string additionalAttributes, TableStyleInfo styleInfo)
        {
            var builder = new StringBuilder();

            var validateAttributes = InputParserUtils.GetValidateAttributes(styleInfo.Additional.IsValidate, styleInfo.DisplayName, styleInfo.Additional.IsRequired, styleInfo.Additional.MinNum, styleInfo.Additional.MaxNum, styleInfo.Additional.ValidateType, styleInfo.Additional.RegExp, styleInfo.Additional.ErrorMessage);

            var value = GetValue(attributeName, formCollection, isAddAndNotPostBack, styleInfo.DefaultValue);
            value = StringUtils.HtmlDecode(value);

            var height = styleInfo.Additional.Height;
            if (height == 0)
            {
                height = 80;
            }
            string style = $@"style=""height:{height}px;""";

            builder.Append($@"<textarea id=""{attributeName}"" name=""{attributeName}"" class=""form-control"" {additionalAttributes} {style} {validateAttributes}>{value}</textarea>");

            if (!styleInfo.Additional.IsValidate) return builder.ToString();

            builder.Append(
                $@"&nbsp;<span id=""{styleInfo.AttributeName}_msg"" style=""color:red;display:none;"">*</span>");
            builder.Append($@"
<script>event_observe('{styleInfo.AttributeName}', 'blur', checkAttributeValue);</script>
");

            return builder.ToString();
        }

        private static string ParseTextEditor(PublishmentSystemInfo publishmentSystemInfo, string attributeName, NameValueCollection formCollection, bool isAddAndNotPostBack, NameValueCollection pageScripts, TableStyleInfo styleInfo)
        {
            return ParseTextEditor(publishmentSystemInfo, attributeName, formCollection, isAddAndNotPostBack, pageScripts, styleInfo.DefaultValue, styleInfo.Additional.Width, styleInfo.Additional.Height);
        }

        public static string ParseTextEditor(PublishmentSystemInfo publishmentSystemInfo, string attributeName, NameValueCollection formCollection, bool isAddAndNotPostBack, NameValueCollection pageScripts, string defaultValue, string width, int height)
        {
            var value = GetValue(attributeName, formCollection, isAddAndNotPostBack, defaultValue);

            value = ContentUtility.TextEditorContentDecode(publishmentSystemInfo, value, true);
            value = ETextEditorTypeUtils.TranslateToHtml(value);
            value = StringUtils.HtmlEncode(value);

            var builder = new StringBuilder();

            var controllerUrl = UEditor.GetUrl(PageUtils.OuterApiUrl, publishmentSystemInfo.PublishmentSystemId);
            var editorUrl = SiteFilesAssets.GetUrl(PageUtils.OuterApiUrl, "ueditor");

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

        private static string ParseDate(string attributeName, NameValueCollection formCollection, bool isAddAndNotPostBack, string additionalAttributes, NameValueCollection pageScripts, TableStyleInfo styleInfo)
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
                    $@"<script language=""javascript"" src=""{SiteServerAssets.GetUrl(SiteServerAssets.DatePicker.Js)}""></script>";
            }

            var value = string.Empty;
            if (dateTime > DateUtils.SqlMinValue)
            {
                value = DateUtils.GetDateString(dateTime);
            }

            builder.Append(
                $@"<input id=""{attributeName}"" name=""{attributeName}"" type=""text"" class=""form-control"" value=""{value}"" {additionalAttributes} onfocus=""{SiteServerAssets
                    .DatePicker.OnFocusDateOnly}"" />");

            return builder.ToString();
        }

        private static string ParseDateTime(string attributeName, NameValueCollection formCollection, bool isAddAndNotPostBack, string additionalAttributes, NameValueCollection pageScripts, TableStyleInfo styleInfo)
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
                    $@"<script type=""text/javascript"" src=""{SiteServerAssets.GetUrl(SiteServerAssets.DatePicker.Js)}""></script>";
            }

            var value = string.Empty;
            if (dateTime > DateUtils.SqlMinValue)
            {
                value = DateUtils.GetDateAndTimeString(dateTime, EDateFormatType.Day, ETimeFormatType.LongTime);
            }

            builder.Append($@"<input id=""{attributeName}"" name=""{attributeName}"" type=""text"" class=""form-control"" value=""{value}"" {additionalAttributes} onfocus=""{SiteServerAssets.DatePicker.OnFocus}"" />");

            return builder.ToString();
        }

        private static string ParseCheckBox(string attributeName, NameValueCollection formCollection, bool isAddAndNotPostBack, TableStyleInfo styleInfo)
        {
            var builder = new StringBuilder();

            var styleItems = styleInfo.StyleItems ?? BaiRongDataProvider.TableStyleItemDao.GetStyleItemInfoList(styleInfo.TableStyleId);

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

            foreach (var styleItem in styleItems)
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

            var styleItems = styleInfo.StyleItems ?? BaiRongDataProvider.TableStyleItemDao.GetStyleItemInfoList(styleInfo.TableStyleId);
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

            foreach (var styleItem in styleItems)
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

            var styleItems = styleInfo.StyleItems ?? BaiRongDataProvider.TableStyleItemDao.GetStyleItemInfoList(styleInfo.TableStyleId);

            var selectedValue = !string.IsNullOrEmpty(formCollection?[attributeName]) ? formCollection[attributeName] : null;
            //验证属性
            var validateAttributes = InputParserUtils.GetValidateAttributes(styleInfo.Additional.IsValidate, styleInfo.DisplayName, styleInfo.Additional.IsRequired, styleInfo.Additional.MinNum, styleInfo.Additional.MaxNum, styleInfo.Additional.ValidateType, styleInfo.Additional.RegExp, styleInfo.Additional.ErrorMessage);
            builder.Append($@"<select id=""{attributeName}"" name=""{attributeName}"" class=""form-control""  isListItem=""true"" {validateAttributes}>");
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

            var styleItems = styleInfo.StyleItems ?? BaiRongDataProvider.TableStyleItemDao.GetStyleItemInfoList(styleInfo.TableStyleId);

            var selectedValues = !string.IsNullOrEmpty(formCollection?[attributeName]) ? formCollection[attributeName] : string.Empty;
            var selectedValueArrayList = TranslateUtils.StringCollectionToStringList(selectedValues);
            //验证属性
            var validateAttributes = InputParserUtils.GetValidateAttributes(styleInfo.Additional.IsValidate, styleInfo.DisplayName, styleInfo.Additional.IsRequired, styleInfo.Additional.MinNum, styleInfo.Additional.MaxNum, styleInfo.Additional.ValidateType, styleInfo.Additional.RegExp, styleInfo.Additional.ErrorMessage);
            builder.Append($@"<select id=""{attributeName}"" name=""{attributeName}"" class=""form-control""  isListItem=""true"" multiple  {validateAttributes}>");
            foreach (var styleItem in styleItems)
            {
                string isSelected;
                if (isAddAndNotPostBack)
                {
                    isSelected = styleItem.IsSelected ? "selected" : string.Empty;
                }
                else
                {
                    isSelected = selectedValueArrayList.Contains(styleItem.ItemValue) ? "selected" : string.Empty;
                }

                builder.Append($@"<option value=""{styleItem.ItemValue}"" {isSelected}>{styleItem.ItemTitle}</option>");
            }
            builder.Append("</select>");

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

        private static string ParseImage(PublishmentSystemInfo publishmentSystemInfo, string attributeName, NameValueCollection formCollection, bool isAddAndNotPostBack, string additionalAttributes, ETableStyle tableStyle, StringBuilder extraBuilder)
        {
            var btnAddHtml = string.Empty;
            if (ETableStyleUtils.IsContent(tableStyle))
            {
                btnAddHtml = $@"
    <button class=""btn"" onclick=""add_{attributeName}('',true)"">
        新增
    </button>
";
            }

            extraBuilder.Append($@"
<div class=""btn-group"">
    <button class=""btn"" onclick=""{ModalUploadImage.GetOpenWindowString(publishmentSystemInfo.PublishmentSystemId, attributeName)}"">
        上传
    </button>
    <button class=""btn"" onclick=""{ModalSelectImage.GetOpenWindowString(publishmentSystemInfo, attributeName)}"">
        选择
    </button>
    <button class=""btn"" onclick=""{ModalCuttingImage.GetOpenWindowStringWithTextBox(publishmentSystemInfo.PublishmentSystemId, attributeName)}"">
        裁切
    </button>
    <button class=""btn"" onclick=""{ModalMessage.GetOpenWindowStringToPreviewImage(publishmentSystemInfo.PublishmentSystemId, attributeName)}"">
        预览
    </button>
    {btnAddHtml}
</div>
");

            var extendAttributeName = ContentAttribute.GetExtendAttributeName(attributeName);

            extraBuilder.Append($@"
<script type=""text/javascript"">
function select_{attributeName}(obj, index){{
  var cmd = ""{ModalSelectImage.GetOpenWindowString(publishmentSystemInfo, attributeName)}"".replace('{attributeName}', '{attributeName}_' + index).replace('return false;', '');
  eval(cmd);
}}
function upload_{attributeName}(obj, index){{
  var cmd = ""{ModalUploadImage.GetOpenWindowString(publishmentSystemInfo.PublishmentSystemId, attributeName)}"".replace('{attributeName}', '{attributeName}_' + index).replace('return false;', '');
  eval(cmd);
}}
function cutting_{attributeName}(obj, index){{
  var cmd = ""{ModalCuttingImage.GetOpenWindowStringWithTextBox(publishmentSystemInfo.PublishmentSystemId, attributeName)}"".replace('{attributeName}', '{attributeName}_' + index).replace('return false;', '');
  eval(cmd);
}}
function preview_{attributeName}(obj, index){{
  var cmd = ""{ModalMessage.GetOpenWindowStringToPreviewImage(publishmentSystemInfo.PublishmentSystemId, attributeName)}"".replace(/{attributeName}/g, '{attributeName}_' + index).replace('return false;', '');
  eval(cmd);
}}
function delete_{attributeName}(obj){{
  $(obj).closest('tr').remove();
}}
var index_{attributeName} = 0;
function add_{attributeName}(val,foucs){{
    index_{attributeName}++;
    var html = '<div class=""clearfix""><div class=""pull-left"">';
    html += '<input id=""{attributeName}_'+index_{attributeName}+'"" name=""{extendAttributeName}"" type=""text"" class=""form-control"" value=""'+val+'"" {additionalAttributes} />&nbsp;';
    html += '</div>';
    html += '<div class=""pull-left btn-group"">';
    html += '<a class=""btn"" href=""javascript:;"" onclick=""select_{attributeName}(this, '+index_{attributeName}+')"" title=""选择""><i class=""icon-th""></i></a>';
    html += '<a class=""btn"" href=""javascript:;"" onclick=""upload_{attributeName}(this, '+index_{attributeName}+')"" title=""上传""><i class=""icon-arrow-up""></i></a>';
    html += '<a class=""btn"" href=""javascript:;"" onclick=""cutting_{attributeName}(this, '+index_{attributeName}+')"" title=""裁切""><i class=""icon-crop""></i></a>';
    html += '<a class=""btn"" href=""javascript:;"" onclick=""preview_{attributeName}(this, '+index_{attributeName}+')"" title=""预览""><i class=""icon-eye-open""></i></a>';
    html += '<a class=""btn"" href=""javascript:;"" onclick=""delete_{attributeName}(this)"" title=""删除""><i class=""icon-remove""></i></a>';
    html += '</div></div>';
    var tr = $('.{extendAttributeName}').length == 0 ? $('#{attributeName}').closest('tr') : $('.{extendAttributeName}:last');
    tr.after('<tr class=""{extendAttributeName}""><td>&nbsp;</td><td colspan=""3"">'+html+'</td></tr>');
    if (foucs) $('#{attributeName}_'+index_{attributeName}).focus();
}}
");

            var extendValues = formCollection[extendAttributeName];
            if (!string.IsNullOrEmpty(extendValues))
            {
                foreach (var extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
                {
                    if (!string.IsNullOrEmpty(extendValue))
                    {
                        extraBuilder.Append($"add_{attributeName}('{extendValue}',false);");
                    }
                }
            }

            extraBuilder.Append("</script>");

            var value = GetValue(attributeName, formCollection, isAddAndNotPostBack, string.Empty);
            return $@"<input id=""{attributeName}"" name=""{attributeName}"" type=""text"" class=""form-control"" value=""{value}"" {additionalAttributes} />";
        }

        private static string ParseVideo(PublishmentSystemInfo publishmentSystemInfo, string attributeName, NameValueCollection formCollection, bool isAddAndNotPostBack, string additionalAttributes, ETableStyle tableStyle, StringBuilder extraBulder)
        {
            var value = GetValue(attributeName, formCollection, isAddAndNotPostBack, string.Empty);

            var btnAddHtml = string.Empty;
            if (ETableStyleUtils.IsContent(tableStyle))
            {
                btnAddHtml = $@"
    <button class=""btn"" onclick=""add_{attributeName}('',true)"">
        新增
    </button>
";
            }

            extraBulder.Append($@"
<div class=""btn-group"">
    <button class=""btn"" onclick=""{ModalUploadVideo.GetOpenWindowStringToTextBox(publishmentSystemInfo.PublishmentSystemId, attributeName)}"">
        上传
    </button>
    <button class=""btn"" onclick=""{ModalSelectVideo.GetOpenWindowString(publishmentSystemInfo, attributeName)}"">
        选择
    </button>
    <button class=""btn"" onclick=""{ModalMessage.GetOpenWindowStringToPreviewVideo(publishmentSystemInfo.PublishmentSystemId, attributeName)}"">
        预览
    </button>
    {btnAddHtml}
</div>");

            var extendAttributeName = ContentAttribute.GetExtendAttributeName(attributeName);

            extraBulder.Append($@"
<script type=""text/javascript"">
function select_{attributeName}(obj, index){{
  var cmd = ""{ModalSelectVideo.GetOpenWindowString(publishmentSystemInfo, attributeName)}"".replace('{attributeName}', '{attributeName}_' + index).replace('return false;', '');
  eval(cmd);
}}
function upload_{attributeName}(obj, index){{
  var cmd = ""{ModalUploadVideo.GetOpenWindowStringToTextBox(publishmentSystemInfo.PublishmentSystemId, attributeName)}"".replace('{attributeName}', '{attributeName}_' + index).replace('return false;', '');
  eval(cmd);
}}
function preview_{attributeName}(obj, index){{
  var cmd = ""{ModalMessage.GetOpenWindowStringToPreviewVideo(publishmentSystemInfo.PublishmentSystemId, attributeName)}"".replace(/{attributeName}/g, '{attributeName}_' + index).replace('return false;', '');
  eval(cmd);
}}
function delete_{attributeName}(obj){{
  $(obj).closest('tr').remove();
}}
var index_{attributeName} = 0;
function add_{attributeName}(val,foucs){{
    index_{attributeName}++;
    var html = '<div class=""clearfix""><div class=""pull-left"">';
    html += '<input id=""{attributeName}_'+index_{attributeName}+'"" name=""{extendAttributeName}"" type=""text"" class=""form-control"" value=""'+val+'"" {additionalAttributes} />&nbsp;';
    html += '</div>';
    html += '<div class=""pull-left btn-group"">';
    html += '<a class=""btn"" href=""javascript:;"" onclick=""select_{attributeName}(this, '+index_{attributeName}+')"" title=""选择""><i class=""icon-th""></i></a>';
    html += '<a class=""btn"" href=""javascript:;"" onclick=""upload_{attributeName}(this, '+index_{attributeName}+')"" title=""上传""><i class=""icon-arrow-up""></i></a>';
    html += '<a class=""btn"" href=""javascript:;"" onclick=""preview_{attributeName}(this, '+index_{attributeName}+')"" title=""预览""><i class=""icon-eye-open""></i></a>';
    html += '<a class=""btn"" href=""javascript:;"" onclick=""delete_{attributeName}(this)"" title=""删除""><i class=""icon-remove""></i></a>';
    html += '</div></div>';
    var tr = $('.{extendAttributeName}').length == 0 ? $('#{attributeName}').closest('tr') : $('.{extendAttributeName}:last');
    tr.after('<tr class=""{extendAttributeName}""><td>&nbsp;</td><td colspan=""3"">'+html+'</td></tr>');
    if (foucs) $('#{attributeName}_'+index_{attributeName}).focus();
}}
");

            var extendValues = formCollection[extendAttributeName];
            if (!string.IsNullOrEmpty(extendValues))
            {
                foreach (var extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
                {
                    if (!string.IsNullOrEmpty(extendValue))
                    {
                        extraBulder.Append($"add_{attributeName}('{extendValue}',false);");
                    }
                }
            }

            extraBulder.Append("</script>");

            return $@"<input id=""{attributeName}"" name=""{attributeName}"" type=""text"" class=""form-control"" value=""{value}"" {additionalAttributes} />";
        }

        private static string ParseFile(PublishmentSystemInfo publishmentSystemInfo, string attributeName, NameValueCollection formCollection, bool isAddAndNotPostBack, string additionalAttributes, ETableStyle tableStyle, StringBuilder extraBuilder)
        {
            var value = GetValue(attributeName, formCollection, isAddAndNotPostBack, string.Empty);
            var relatedPath = string.Empty;
            if (!string.IsNullOrEmpty(value))
            {
                value = value.Trim('/');
                var i = value.LastIndexOf('/');
                if (i != -1)
                {
                    relatedPath = value.Substring(0, i + 1);
                }
            }

            var btnAddHtml = string.Empty;
            if (ETableStyleUtils.IsContent(tableStyle))
            {
                btnAddHtml = $@"
<button class=""btn"" onclick=""add_{attributeName}('',true)"">
    新增
</button>
";
            }

            extraBuilder.Append($@"
<div class=""btn-group"">
    <button class=""btn"" onclick=""{ModalUploadFile.GetOpenWindowStringToTextBox(publishmentSystemInfo.PublishmentSystemId, EUploadType.File, attributeName)}"">
        上传
    </button>
    <button class=""btn"" onclick=""{ModalSelectFile.GetOpenWindowString(publishmentSystemInfo.PublishmentSystemId, attributeName, relatedPath)}"">
        选择
    </button>
    <button class=""btn"" onclick=""{ModalFileView.GetOpenWindowStringWithTextBoxValue(publishmentSystemInfo.PublishmentSystemId, attributeName)}"">
        查看
    </button>
    {btnAddHtml}
</div>
");

            var extendAttributeName = ContentAttribute.GetExtendAttributeName(attributeName);

            extraBuilder.Append($@"
<script type=""text/javascript"">
function select_{attributeName}(obj, index){{
  var cmd = ""{ModalSelectFile.GetOpenWindowString(publishmentSystemInfo.PublishmentSystemId, attributeName, relatedPath)}"".replace('{attributeName}', '{attributeName}_' + index).replace('return false;', '');
  eval(cmd);
}}
function upload_{attributeName}(obj, index){{
  var cmd = ""{ModalUploadFile.GetOpenWindowStringToTextBox(publishmentSystemInfo.PublishmentSystemId, EUploadType.File,
                attributeName)}"".replace('{attributeName}', '{attributeName}_' + index).replace('return false;', '');
  eval(cmd);
}}
function preview_{attributeName}(obj, index){{
  var cmd = ""{ModalFileView.GetOpenWindowStringWithTextBoxValue(publishmentSystemInfo.PublishmentSystemId,
                attributeName)}"".replace(/{attributeName}/g, '{attributeName}_' + index).replace('return false;', '');
  eval(cmd);
}}
function delete_{attributeName}(obj){{
  $(obj).closest('tr').remove();
}}
var index_{attributeName} = 0;
function add_{attributeName}(val,foucs){{
    index_{attributeName}++;
    var html = '<div class=""clearfix""><div class=""pull-left"">';
    html += '<input id=""{attributeName}_'+index_{attributeName}+'"" name=""{extendAttributeName}"" type=""text"" class=""form-control"" value=""'+val+'"" {additionalAttributes} />&nbsp;';
    html += '</div>';
    html += '<div class=""pull-left btn-group"">';
    html += '<a class=""btn"" href=""javascript:;"" onclick=""select_{attributeName}(this, '+index_{attributeName}+')"" title=""选择""><i class=""icon-th""></i></a>';
    html += '<a class=""btn"" href=""javascript:;"" onclick=""upload_{attributeName}(this, '+index_{attributeName}+')"" title=""上传""><i class=""icon-arrow-up""></i></a>';
    html += '<a class=""btn"" href=""javascript:;"" onclick=""preview_{attributeName}(this, '+index_{attributeName}+')"" title=""查看""><i class=""icon-eye-open""></i></a>';
    html += '<a class=""btn"" href=""javascript:;"" onclick=""delete_{attributeName}(this)"" title=""删除""><i class=""icon-remove""></i></a>';
    html += '</div></div>';
    var tr = $('.{extendAttributeName}').length == 0 ? $('#{attributeName}').closest('tr') : $('.{extendAttributeName}:last');
    tr.after('<tr class=""{extendAttributeName}""><td>&nbsp;</td><td colspan=""3"">'+html+'</td></tr>');
    if (foucs) $('#{attributeName}_'+index_{attributeName}).focus();
}}
");

            var extendValues = formCollection[extendAttributeName];
            if (!string.IsNullOrEmpty(extendValues))
            {
                foreach (var extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
                {
                    if (!string.IsNullOrEmpty(extendValue))
                    {
                        extraBuilder.Append($"add_{attributeName}('{extendValue}',false);");
                    }
                }
            }

            extraBuilder.Append("</script>");

            return
                $@"<input id=""{attributeName}"" name=""{attributeName}"" type=""text"" class=""form-control"" value=""{value}"" {additionalAttributes} />";
        }

        private static string ParseRelatedField(PublishmentSystemInfo publishmentSystemInfo, string attributeName, NameValueCollection formCollection, TableStyleInfo styleInfo)
        {
            var fieldInfo = DataProvider.RelatedFieldDao.GetRelatedFieldInfo(styleInfo.Additional.RelatedFieldId);
            if (fieldInfo == null) return string.Empty;

            var builder = new StringBuilder();

            var list = DataProvider.RelatedFieldItemDao.GetRelatedFieldItemInfoList(styleInfo.Additional.RelatedFieldId, 0);

            var prefixes = TranslateUtils.StringCollectionToStringCollection(fieldInfo.Prefixes);
            var suffixes = TranslateUtils.StringCollectionToStringCollection(fieldInfo.Suffixes);

            var style = ERelatedFieldStyleUtils.GetEnumType(styleInfo.Additional.RelatedFieldStyle);

            builder.Append($@"
<span id=""c_{attributeName}_1"">
    {prefixes[0]}
    <select name=""{attributeName}"" id=""{attributeName}_1"" class=""select"" onchange=""getRelatedField_{fieldInfo.RelatedFieldId}(2);"">
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
                var selected = !string.IsNullOrEmpty(itemInfo.ItemValue) && value == itemInfo.ItemValue ? @" selected=""selected""" : string.Empty;
                if (!string.IsNullOrEmpty(selected)) isLoad = true;
                builder.Append($@"
	<option value=""{itemInfo.ItemValue}"" itemID=""{itemInfo.Id}""{selected}>{itemInfo.ItemName}</option>");
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
{prefixes[i - 1]}
<select name=""{attributeName}"" id=""{attributeName}_{i}"" class=""select"" onchange=""getRelatedField_{fieldInfo.RelatedFieldId}({i} + 1);""></select>
{suffixes[i - 1]}
</span>
");
                }
            }

            builder.Append($@"
<script>
function getRelatedField_{fieldInfo.RelatedFieldId}(level){{
    var attributeName = '{styleInfo.AttributeName}';
    var totalLevel = {fieldInfo.TotalLevel};
    for(i=level;i<=totalLevel;i++){{
        $('#c_' + attributeName + '_' + i).hide();
    }}
    var obj = $('#c_' + attributeName + '_' + (level - 1));
    var itemID = $('option:selected', obj).attr('itemID');
    if (itemID){{
        var url = '{ActionsRelatedField.GetUrl(PageUtils.InnerApiUrl, publishmentSystemInfo.PublishmentSystemId,
                styleInfo.Additional.RelatedFieldId, 0)}' + itemID;
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
                getRelatedField_{fieldInfo.RelatedFieldId}(level + 1);
            }}
        }}, 'jsonp');
    }}
}}
");

            if (isLoad)
            {
                builder.Append($@"
$(document).ready(function(){{
    getRelatedField_{fieldInfo.RelatedFieldId}(2);
}});
");
            }

            builder.Append("</script>");
            return builder.ToString();
        }

        public static void AddValuesToAttributes(ETableStyle tableStyle, string tableName, PublishmentSystemInfo publishmentSystemInfo, List<int> relatedIdentities, NameValueCollection formCollection, NameValueCollection attributes, List<string> dontAddAttributes)
        {
            if (dontAddAttributes == null)
            {
                dontAddAttributes = new List<string>();
            }
            var styleInfoList = TableStyleManager.GetTableStyleInfoList(tableStyle, tableName, relatedIdentities);
            foreach (var styleInfo in styleInfoList)
            {
                if (styleInfo.IsVisible == false || dontAddAttributes.Contains(styleInfo.AttributeName.ToLower())) continue;
                var theValue = GetValueByForm(styleInfo, publishmentSystemInfo, formCollection);

                if (!InputTypeUtils.EqualsAny(styleInfo.InputType, InputType.TextEditor, InputType.Image, InputType.File, InputType.Video) && styleInfo.AttributeName != BackgroundContentAttribute.LinkUrl)
                {
                    theValue = PageUtils.FilterSqlAndXss(theValue);
                }

                TranslateUtils.SetOrRemoveAttributeLowerCase(attributes, styleInfo.AttributeName, theValue);

                if (styleInfo.Additional.IsFormatString)
                {
                    var formatString = TranslateUtils.ToBool(formCollection[styleInfo.AttributeName + "_formatStrong"]);
                    var formatEm = TranslateUtils.ToBool(formCollection[styleInfo.AttributeName + "_formatEM"]);
                    var formatU = TranslateUtils.ToBool(formCollection[styleInfo.AttributeName + "_formatU"]);
                    var formatColor = formCollection[styleInfo.AttributeName + "_formatColor"];
                    var theFormatString = ContentUtility.GetTitleFormatString(formatString, formatEm, formatU, formatColor);

                    TranslateUtils.SetOrRemoveAttributeLowerCase(attributes, ContentAttribute.GetFormatStringAttributeName(styleInfo.AttributeName), theFormatString);
                }

                if (InputTypeUtils.EqualsAny(styleInfo.InputType, InputType.Image, InputType.Video, InputType.File))
                {
                    var attributeName = ContentAttribute.GetExtendAttributeName(styleInfo.AttributeName);
                    TranslateUtils.SetOrRemoveAttributeLowerCase(attributes, attributeName, formCollection[attributeName]);
                }
            }
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

            var inputType = InputTypeUtils.GetEnumType(styleInfo.InputType);

            if (inputType == InputType.TextEditor)
            {
                theValue = ContentUtility.TextEditorContentEncode(publishmentSystemInfo, theValue);
                theValue = ETextEditorTypeUtils.TranslateToStlElement(theValue);
            }

            return theValue;
        }
    }
}
