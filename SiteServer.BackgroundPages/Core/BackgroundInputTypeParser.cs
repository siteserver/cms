using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Ajax;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Api;
using SiteServer.CMS.Api.Sys.Editors;
using SiteServer.CMS.Api.Sys.Stl;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.Plugin;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Core
{
    public static class BackgroundInputTypeParser
    {
        public const string Current = "{Current}";
        public const string Value = "{Value}";

        public static string Parse(SiteInfo siteInfo, int channelId, TableStyleInfo styleInfo, IAttributes attributes, NameValueCollection pageScripts, out string extraHtml)
        {
            var retval = string.Empty;
            var extraBuilder = new StringBuilder();
            if (!string.IsNullOrEmpty(styleInfo.HelpText))
            {
                extraBuilder.Append($@"<small class=""form-text text-muted"">{styleInfo.HelpText}</small>");
            }

            var inputType = styleInfo.InputType;

            if (inputType == InputType.Text)
            {
                retval = ParseText(attributes, siteInfo, channelId, styleInfo, extraBuilder);
            }
            else if (inputType == InputType.TextArea)
            {
                retval = ParseTextArea(attributes, styleInfo, extraBuilder);
            }
            else if (inputType == InputType.TextEditor)
            {
                retval = ParseTextEditor(attributes, styleInfo.AttributeName, siteInfo, pageScripts, extraBuilder);
            }
            else if (inputType == InputType.SelectOne)
            {
                retval = ParseSelectOne(attributes, styleInfo, extraBuilder);
            }
            else if (inputType == InputType.SelectMultiple)
            {
                retval = ParseSelectMultiple(attributes, styleInfo, extraBuilder);
            }
            else if (inputType == InputType.SelectCascading)
            {
                retval = ParseSelectCascading(attributes, siteInfo, styleInfo, extraBuilder);
            }
            else if (inputType == InputType.CheckBox)
            {
                retval = ParseCheckBox(attributes, styleInfo, extraBuilder);
            }
            else if (inputType == InputType.Radio)
            {
                retval = ParseRadio(attributes, styleInfo, extraBuilder);
            }
            else if (inputType == InputType.Date)
            {
                retval = ParseDate(attributes, pageScripts, styleInfo, extraBuilder);
            }
            else if (inputType == InputType.DateTime)
            {
                retval = ParseDateTime(attributes, pageScripts, styleInfo, extraBuilder);
            }
            else if (inputType == InputType.Image)
            {
                retval = ParseImage(attributes, siteInfo, channelId, styleInfo, extraBuilder);
            }
            else if (inputType == InputType.Video)
            {
                retval = ParseVideo(attributes, siteInfo, channelId, styleInfo, extraBuilder);
            }
            else if (inputType == InputType.File)
            {
                retval = ParseFile(attributes, siteInfo, channelId, styleInfo, extraBuilder);
            }
            else if (inputType == InputType.Customize)
            {
                retval = ParseCustomize(attributes, styleInfo, extraBuilder);
            }
            else if (inputType == InputType.Hidden)
            {
                retval = string.Empty;
                extraBuilder.Clear();
            }

            extraHtml = extraBuilder.ToString();
            return retval;
        }

        public static string ParseText(IAttributes attributes, SiteInfo siteInfo, int channelId, TableStyleInfo styleInfo, StringBuilder extraBuilder)
        {
            var validateAttributes = InputParserUtils.GetValidateAttributes(styleInfo.Additional.IsValidate, styleInfo.DisplayName, styleInfo.Additional.IsRequired, styleInfo.Additional.MinNum, styleInfo.Additional.MaxNum, styleInfo.Additional.ValidateType, styleInfo.Additional.RegExp, styleInfo.Additional.ErrorMessage);

            if (styleInfo.Additional.IsValidate)
            {
                extraBuilder.Append(
                    $@"<span id=""{styleInfo.AttributeName}_msg"" style=""color:red;display:none;"">*</span><script>event_observe('{styleInfo.AttributeName}', 'blur', checkAttributeValue);</script>");
            }

            if (styleInfo.Additional.IsFormatString)
            {
                var formatStrong = false;
                var formatEm = false;
                var formatU = false;
                var formatColor = string.Empty;
                var formatValues = attributes.GetString(ContentAttribute.GetFormatStringAttributeName(styleInfo.AttributeName));
                if (!string.IsNullOrEmpty(formatValues))
                {
                    ContentUtility.SetTitleFormatControls(formatValues, out formatStrong, out formatEm, out formatU, out formatColor);
                }

                extraBuilder.Append(
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

                extraBuilder.Append($@"
<div class=""btn-group btn-group-sm"" style=""float:left;"">
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

            if (channelId > 0 && styleInfo.AttributeName == ContentAttribute.Title)
            {
                extraBuilder.Append(@"
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
                extraBuilder.Replace("[url]", AjaxCmsService.GetTitlesUrl(siteInfo.Id, channelId));
            }

            var value = StringUtils.HtmlDecode(attributes.GetString(styleInfo.AttributeName));

            return
                $@"<input id=""{styleInfo.AttributeName}"" name=""{styleInfo.AttributeName}"" type=""text"" class=""form-control"" value=""{value}"" {validateAttributes} />";
        }

        public static string ParseTextArea(IAttributes attributes, TableStyleInfo styleInfo, StringBuilder extraBuilder)
        {
            if (styleInfo.Additional.IsValidate)
            {
                extraBuilder.Append(
                $@"<span id=""{styleInfo.AttributeName}_msg"" style=""color:red;display:none;"">*</span><script>event_observe('{styleInfo.AttributeName}', 'blur', checkAttributeValue);</script>");
            }

            var validateAttributes = InputParserUtils.GetValidateAttributes(styleInfo.Additional.IsValidate, styleInfo.DisplayName, styleInfo.Additional.IsRequired, styleInfo.Additional.MinNum, styleInfo.Additional.MaxNum, styleInfo.Additional.ValidateType, styleInfo.Additional.RegExp, styleInfo.Additional.ErrorMessage);

            var height = styleInfo.Additional.Height;
            if (height == 0)
            {
                height = 80;
            }
            string style = $@"style=""height:{height}px;""";

            var value = StringUtils.HtmlDecode(attributes.GetString(styleInfo.AttributeName));

            return
                $@"<textarea id=""{styleInfo.AttributeName}"" name=""{styleInfo.AttributeName}"" class=""form-control"" {style} {validateAttributes}>{value}</textarea>";
        }

        public static string ParseTextEditor(IAttributes attributes, string attributeName, SiteInfo siteInfo, NameValueCollection pageScripts, StringBuilder extraBuilder)
        {
            var value = attributes.GetString(attributeName);

            value = ContentUtility.TextEditorContentDecode(siteInfo, value, true);
            value = UEditorUtils.TranslateToHtml(value);
            value = StringUtils.HtmlEncode(value);

            var controllerUrl = ApiRouteUEditor.GetUrl(ApiManager.InnerApiUrl, siteInfo.Id);
            var editorUrl = SiteServerAssets.GetUrl("ueditor");

            if (pageScripts["uEditor"] == null)
            {
                extraBuilder.Append(
                    $@"<script type=""text/javascript"">window.UEDITOR_HOME_URL = ""{editorUrl}/"";window.UEDITOR_CONTROLLER_URL = ""{controllerUrl}"";</script><script type=""text/javascript"" src=""{editorUrl}/editor_config.js""></script><script type=""text/javascript"" src=""{editorUrl}/ueditor_all_min.js""></script>");
            }
            pageScripts["uEditor"] = string.Empty;

            extraBuilder.Append($@"
<script type=""text/javascript"">
$(function(){{
  UE.getEditor('{attributeName}', {UEditorUtils.ConfigValues});
  $('#{attributeName}').show();
}});
</script>");

            return $@"<textarea id=""{attributeName}"" name=""{attributeName}"" style=""display:none"">{value}</textarea>";
        }

        private static string ParseSelectOne(IAttributes attributes, TableStyleInfo styleInfo, StringBuilder extraBuilder)
        {
            if (styleInfo.Additional.IsValidate)
            {
                extraBuilder.Append(
                    $@"<span id=""{styleInfo.AttributeName}_msg"" style=""color:red;display:none;"">*</span><script>event_observe('{styleInfo.AttributeName}', 'blur', checkAttributeValue);</script>");
            }

            var builder = new StringBuilder();
            var styleItems = styleInfo.StyleItems ?? new List<TableStyleItemInfo>();

            var selectedValue = attributes.GetString(styleInfo.AttributeName);

            var validateAttributes = InputParserUtils.GetValidateAttributes(styleInfo.Additional.IsValidate, styleInfo.DisplayName, styleInfo.Additional.IsRequired, styleInfo.Additional.MinNum, styleInfo.Additional.MaxNum, styleInfo.Additional.ValidateType, styleInfo.Additional.RegExp, styleInfo.Additional.ErrorMessage);
            builder.Append($@"<select id=""{styleInfo.AttributeName}"" name=""{styleInfo.AttributeName}"" class=""form-control""  isListItem=""true"" {validateAttributes}>");

            var isTicked = false;
            foreach (var styleItem in styleItems)
            {
                var isOptionSelected = false;
                if (!isTicked)
                {
                    isTicked = isOptionSelected = styleItem.ItemValue == selectedValue;
                }

                builder.Append($@"<option value=""{styleItem.ItemValue}"" {(isOptionSelected ? "selected" : string.Empty)}>{styleItem.ItemTitle}</option>");
            }

            builder.Append("</select>");

            return builder.ToString();
        }

        private static string ParseSelectMultiple(IAttributes attributes, TableStyleInfo styleInfo, StringBuilder extraBuilder)
        {
            if (styleInfo.Additional.IsValidate)
            {
                extraBuilder.Append(
                    $@"<span id=""{styleInfo.AttributeName}_msg"" style=""color:red;display:none;"">*</span><script>event_observe('{styleInfo.AttributeName}', 'blur', checkAttributeValue);</script>");
            }

            var builder = new StringBuilder();
            var styleItems = styleInfo.StyleItems ?? new List<TableStyleItemInfo>();

            var selectedValues = TranslateUtils.StringCollectionToStringList(attributes.GetString(styleInfo.AttributeName));

            var validateAttributes = InputParserUtils.GetValidateAttributes(styleInfo.Additional.IsValidate, styleInfo.DisplayName, styleInfo.Additional.IsRequired, styleInfo.Additional.MinNum, styleInfo.Additional.MaxNum, styleInfo.Additional.ValidateType, styleInfo.Additional.RegExp, styleInfo.Additional.ErrorMessage);
            builder.Append($@"<select id=""{styleInfo.AttributeName}"" name=""{styleInfo.AttributeName}"" class=""form-control"" isListItem=""true"" multiple  {validateAttributes}>");

            foreach (var styleItem in styleItems)
            {
                var isSelected = selectedValues.Contains(styleItem.ItemValue);
                builder.Append($@"<option value=""{styleItem.ItemValue}"" {(isSelected ? "selected" : string.Empty)}>{styleItem.ItemTitle}</option>");
            }

            builder.Append("</select>");
            return builder.ToString();
        }

        private static string ParseSelectCascading(IAttributes attributes, SiteInfo siteInfo, TableStyleInfo styleInfo, StringBuilder extraBuilder)
        {
            var attributeName = styleInfo.AttributeName;
            var fieldInfo = DataProvider.RelatedFieldDao.GetRelatedFieldInfo(styleInfo.Additional.RelatedFieldId);
            if (fieldInfo == null) return string.Empty;

            var list = DataProvider.RelatedFieldItemDao.GetRelatedFieldItemInfoList(styleInfo.Additional.RelatedFieldId, 0);

            var prefixes = TranslateUtils.StringCollectionToStringCollection(fieldInfo.Prefixes);
            var suffixes = TranslateUtils.StringCollectionToStringCollection(fieldInfo.Suffixes);

            var style = ERelatedFieldStyleUtils.GetEnumType(styleInfo.Additional.RelatedFieldStyle);

            var builder = new StringBuilder();
            builder.Append($@"
<span id=""c_{attributeName}_1"">
    {prefixes[0]}
    <select name=""{attributeName}"" id=""{attributeName}_1"" class=""select"" onchange=""getRelatedField_{fieldInfo.Id}(2);"">
        <option value="""">请选择</option>");

            var values = attributes.GetString(attributeName);
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
<select name=""{attributeName}"" id=""{attributeName}_{i}"" class=""select"" onchange=""getRelatedField_{fieldInfo.Id}({i} + 1);""></select>
{suffixes[i - 1]}
</span>
");
                }
            }

            extraBuilder.Append($@"
<script>
function getRelatedField_{fieldInfo.Id}(level){{
    var attributeName = '{styleInfo.AttributeName}';
    var totalLevel = {fieldInfo.TotalLevel};
    for(i=level;i<=totalLevel;i++){{
        $('#c_' + attributeName + '_' + i).hide();
    }}
    var obj = $('#c_' + attributeName + '_' + (level - 1));
    var itemID = $('option:selected', obj).attr('itemID');
    if (itemID){{
        var url = '{ApiRouteActionsRelatedField.GetUrl(ApiManager.InnerApiUrl, siteInfo.Id,
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
                getRelatedField_{fieldInfo.Id}(level + 1);
            }}
        }}, 'jsonp');
    }}
}}
");

            if (isLoad)
            {
                extraBuilder.Append($@"
$(document).ready(function(){{
    getRelatedField_{fieldInfo.Id}(2);
}});
");
            }

            extraBuilder.Append("</script>");

            return builder.ToString();
        }

        private static string ParseCheckBox(IAttributes attributes, TableStyleInfo styleInfo, StringBuilder extraBuilder)
        {
            if (styleInfo.Additional.IsValidate)
            {
                extraBuilder.Append(
                    $@"<span id=""{styleInfo.AttributeName}_msg"" style=""color:red;display:none;"">*</span><script>event_observe('{styleInfo.AttributeName}', 'blur', checkAttributeValue);</script>");
            }

            var builder = new StringBuilder();

            var styleItems = styleInfo.StyleItems ?? new List<TableStyleItemInfo>();

            var checkBoxList = new CheckBoxList
            {
                CssClass = "checkbox checkbox-primary",
                ID = styleInfo.AttributeName,
                RepeatDirection = styleInfo.IsHorizontal ? RepeatDirection.Horizontal : RepeatDirection.Vertical,
                RepeatColumns = styleInfo.Additional.Columns
            };

            var selectedValues = TranslateUtils.StringCollectionToStringList(attributes.GetString(styleInfo.AttributeName));

            InputParserUtils.GetValidateAttributesForListItem(checkBoxList, styleInfo.Additional.IsValidate, styleInfo.DisplayName, styleInfo.Additional.IsRequired, styleInfo.Additional.MinNum, styleInfo.Additional.MaxNum, styleInfo.Additional.ValidateType, styleInfo.Additional.RegExp, styleInfo.Additional.ErrorMessage);

            foreach (var styleItem in styleItems)
            {
                var isSelected = selectedValues.Contains(styleItem.ItemValue);
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
                builder.Replace($@"name=""{styleInfo.AttributeName}${i}""",
                    $@"name=""{styleInfo.AttributeName}"" value=""{styleItem.ItemValue}""");
                i++;
            }

            return builder.ToString();
        }

        private static string ParseRadio(IAttributes attributes, TableStyleInfo styleInfo, StringBuilder extraBuilder)
        {
            if (styleInfo.Additional.IsValidate)
            {
                extraBuilder.Append(
                    $@"<span id=""{styleInfo.AttributeName}_msg"" style=""color:red;display:none;"">*</span><script>event_observe('{styleInfo.AttributeName}', 'blur', checkAttributeValue);</script>");
            }

            var builder = new StringBuilder();

            var styleItems = styleInfo.StyleItems;
            if (styleItems == null || styleItems.Count == 0)
            {
                styleItems = new List<TableStyleItemInfo>
                {
                    new TableStyleItemInfo
                    {
                        ItemTitle = "是",
                        ItemValue = "1"
                    },
                    new TableStyleItemInfo
                    {
                        ItemTitle = "否",
                        ItemValue = "0"
                    }
                };
            }
            var radioButtonList = new RadioButtonList
            {
                CssClass = "radio radio-primary",
                ID = styleInfo.AttributeName,
                RepeatDirection = styleInfo.IsHorizontal ? RepeatDirection.Horizontal : RepeatDirection.Vertical,
                RepeatColumns = styleInfo.Additional.Columns
            };

            var selectedValue = attributes.GetString(styleInfo.AttributeName);

            InputParserUtils.GetValidateAttributesForListItem(radioButtonList, styleInfo.Additional.IsValidate, styleInfo.DisplayName, styleInfo.Additional.IsRequired, styleInfo.Additional.MinNum, styleInfo.Additional.MaxNum, styleInfo.Additional.ValidateType, styleInfo.Additional.RegExp, styleInfo.Additional.ErrorMessage);

            var isTicked = false;
            foreach (var styleItem in styleItems)
            {
                var isOptionSelected = false;
                if (!isTicked)
                {
                    isTicked = isOptionSelected = styleItem.ItemValue == selectedValue;
                }
                
                var listItem = new ListItem(styleItem.ItemTitle, styleItem.ItemValue)
                {
                    Selected = isOptionSelected
                };
                radioButtonList.Items.Add(listItem);
            }
            radioButtonList.Attributes.Add("isListItem", "true");
            builder.Append(ControlUtils.GetControlRenderHtml(radioButtonList));

            return builder.ToString();
        }

        private static string ParseDate(IAttributes attributes, NameValueCollection pageScripts, TableStyleInfo styleInfo, StringBuilder extraBuilder)
        {
            if (styleInfo.Additional.IsValidate)
            {
                extraBuilder.Append(
                    $@"<span id=""{styleInfo.AttributeName}_msg"" style=""color:red;display:none;"">*</span><script>event_observe('{styleInfo.AttributeName}', 'blur', checkAttributeValue);</script>");
            }

            var selectedValue = attributes.GetString(styleInfo.AttributeName);
            var dateTime = selectedValue == Current ? DateTime.Now : TranslateUtils.ToDateTime(selectedValue);

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

            return
                $@"<input id=""{styleInfo.AttributeName}"" name=""{styleInfo.AttributeName}"" type=""text"" class=""form-control"" value=""{value}"" onfocus=""{SiteServerAssets.DatePicker.OnFocusDateOnly}"" style=""width: 180px"" />";
        }

        private static string ParseDateTime(IAttributes attributes, NameValueCollection pageScripts, TableStyleInfo styleInfo, StringBuilder extraBuilder)
        {
            if (styleInfo.Additional.IsValidate)
            {
                extraBuilder.Append(
                    $@"<span id=""{styleInfo.AttributeName}_msg"" style=""color:red;display:none;"">*</span><script>event_observe('{styleInfo.AttributeName}', 'blur', checkAttributeValue);</script>");
            }

            var selectedValue = attributes.GetString(styleInfo.AttributeName);
            var dateTime = selectedValue == Current ? DateTime.Now : TranslateUtils.ToDateTime(selectedValue);

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

            return $@"<input id=""{styleInfo.AttributeName}"" name=""{styleInfo.AttributeName}"" type=""text"" class=""form-control"" value=""{value}"" onfocus=""{SiteServerAssets.DatePicker.OnFocus}"" style=""width: 180px"" />";
        }

        private static string ParseImage(IAttributes attributes, SiteInfo siteInfo, int channelId, TableStyleInfo styleInfo, StringBuilder extraBuilder)
        {
            var btnAddHtml = string.Empty;

            if (channelId > 0)
            {
                btnAddHtml = $@"
    <button class=""btn"" onclick=""add_{styleInfo.AttributeName}('',true);return false;"">
        新增
    </button>
";
            }

            extraBuilder.Append($@"
<div class=""btn-group btn-group-sm"">
    <button class=""btn"" onclick=""{ModalUploadImage.GetOpenWindowString(siteInfo.Id, styleInfo.AttributeName)}"">
        上传
    </button>
    <button class=""btn"" onclick=""{ModalSelectImage.GetOpenWindowString(siteInfo, styleInfo.AttributeName)}"">
        选择
    </button>
    <button class=""btn"" onclick=""{ModalCuttingImage.GetOpenWindowStringWithTextBox(siteInfo.Id, styleInfo.AttributeName)}"">
        裁切
    </button>
    <button class=""btn"" onclick=""{ModalMessage.GetOpenWindowStringToPreviewImage(siteInfo.Id, styleInfo.AttributeName)}"">
        预览
    </button>
    {btnAddHtml}
</div>
");

            var attributeName = styleInfo.AttributeName;
            var extendAttributeName = ContentAttribute.GetExtendAttributeName(styleInfo.AttributeName);

            extraBuilder.Append($@"
<script type=""text/javascript"">
function select_{styleInfo.AttributeName}(obj, index){{
  var cmd = ""{ModalSelectImage.GetOpenWindowString(siteInfo, styleInfo.AttributeName)}"".replace('{attributeName}', '{attributeName}_' + index).replace('return false;', '');
  eval(cmd);
}}
function upload_{attributeName}(obj, index){{
  var cmd = ""{ModalUploadImage.GetOpenWindowString(siteInfo.Id, attributeName)}"".replace('{attributeName}', '{attributeName}_' + index).replace('return false;', '');
  eval(cmd);
}}
function cutting_{attributeName}(obj, index){{
  var cmd = ""{ModalCuttingImage.GetOpenWindowStringWithTextBox(siteInfo.Id, attributeName)}"".replace('{attributeName}', '{attributeName}_' + index).replace('return false;', '');
  eval(cmd);
}}
function preview_{attributeName}(obj, index){{
  var cmd = ""{ModalMessage.GetOpenWindowStringToPreviewImage(siteInfo.Id, attributeName)}"".replace(/{attributeName}/g, '{attributeName}_' + index).replace('return false;', '');
  eval(cmd);
}}
function delete_{attributeName}(obj){{
  $(obj).parent().parent().parent().remove();
}}
var index_{attributeName} = 0;
function add_{attributeName}(val,foucs){{
    index_{attributeName}++;
    var inputHtml = '<input id=""{attributeName}_'+index_{attributeName}+'"" name=""{extendAttributeName}"" type=""text"" class=""form-control"" value=""'+val+'"" />';
    var btnHtml = '<div class=""btn-group btn-group-sm"">';
    btnHtml += '<button class=""btn"" href=""javascript:;"" onclick=""select_{attributeName}(this, '+index_{attributeName}+');return false;"">选择</button>';
    btnHtml += '<button class=""btn"" href=""javascript:;"" onclick=""upload_{attributeName}(this, '+index_{attributeName}+');return false;"">上传</button>';
    btnHtml += '<button class=""btn"" href=""javascript:;"" onclick=""cutting_{attributeName}(this, '+index_{attributeName}+');return false;"">裁切</button>';
    btnHtml += '<button class=""btn"" href=""javascript:;"" onclick=""preview_{attributeName}(this, '+index_{attributeName}+');return false;"">预览</button>';
    btnHtml += '<button class=""btn"" href=""javascript:;"" onclick=""delete_{attributeName}(this);return false;"">删除</button>';
    btnHtml += '</div>';
    var div = $('.{extendAttributeName}').length == 0 ? $('#{attributeName}').parent().parent() : $('.{extendAttributeName}:last');
    div.after('<div class=""form-group form-row {extendAttributeName}""><label class=""col-sm-1 col-form-label text-right""></label><div class=""col-sm-6"">' + inputHtml + '</div><div class=""col-sm-5"">' + btnHtml + '</div></div>');
    if (foucs) $('#{attributeName}_'+index_{attributeName}).focus();

}}
");

            var extendValues = attributes.GetString(extendAttributeName);
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

            return $@"<input id=""{attributeName}"" name=""{attributeName}"" type=""text"" class=""form-control"" value=""{attributes.GetString(attributeName)}"" />";
        }

        private static string ParseVideo(IAttributes attributes, SiteInfo siteInfo, int channelId, TableStyleInfo styleInfo, StringBuilder extraBulder)
        {
            var attributeName = styleInfo.AttributeName;

            var btnAddHtml = string.Empty;
            if (channelId > 0)
            {
                btnAddHtml = $@"
    <button class=""btn"" onclick=""add_{attributeName}('',true);return false;"">
        新增
    </button>
";
            }

            extraBulder.Append($@"
<div class=""btn-group btn-group-sm"">
    <button class=""btn"" onclick=""{ModalUploadVideo.GetOpenWindowStringToTextBox(siteInfo.Id, attributeName)}"">
        上传
    </button>
    <button class=""btn"" onclick=""{ModalSelectVideo.GetOpenWindowString(siteInfo, attributeName)}"">
        选择
    </button>
    <button class=""btn"" onclick=""{ModalMessage.GetOpenWindowStringToPreviewVideo(siteInfo.Id, attributeName)}"">
        预览
    </button>
    {btnAddHtml}
</div>");

            var extendAttributeName = ContentAttribute.GetExtendAttributeName(attributeName);

            extraBulder.Append($@"
<script type=""text/javascript"">
function select_{attributeName}(obj, index){{
  var cmd = ""{ModalSelectVideo.GetOpenWindowString(siteInfo, attributeName)}"".replace('{attributeName}', '{attributeName}_' + index).replace('return false;', '');
  eval(cmd);
}}
function upload_{attributeName}(obj, index){{
  var cmd = ""{ModalUploadVideo.GetOpenWindowStringToTextBox(siteInfo.Id, attributeName)}"".replace('{attributeName}', '{attributeName}_' + index).replace('return false;', '');
  eval(cmd);
}}
function preview_{attributeName}(obj, index){{
  var cmd = ""{ModalMessage.GetOpenWindowStringToPreviewVideo(siteInfo.Id, attributeName)}"".replace(/{attributeName}/g, '{attributeName}_' + index).replace('return false;', '');
  eval(cmd);
}}
function delete_{attributeName}(obj){{
  $(obj).parent().parent().parent().remove();
}}
var index_{attributeName} = 0;
function add_{attributeName}(val,foucs){{
    index_{attributeName}++;
    var inputHtml = '<input id=""{attributeName}_'+index_{attributeName}+'"" name=""{extendAttributeName}"" type=""text"" class=""form-control"" value=""'+val+'"" />';
    var btnHtml = '<div class=""btn-group btn-group-sm"">';
    btnHtml += '<button class=""btn"" href=""javascript:;"" onclick=""select_{attributeName}(this, '+index_{attributeName}+');return false;"">选择</button>';
    btnHtml += '<button class=""btn"" href=""javascript:;"" onclick=""upload_{attributeName}(this, '+index_{attributeName}+');return false;"">上传</button>';
    btnHtml += '<button class=""btn"" href=""javascript:;"" onclick=""preview_{attributeName}(this, '+index_{attributeName}+');return false;"">预览</button>';
    btnHtml += '<button class=""btn"" href=""javascript:;"" onclick=""delete_{attributeName}(this);return false;"">删除</button>';
    btnHtml += '</div>';
    var div = $('.{extendAttributeName}').length == 0 ? $('#{attributeName}').parent().parent() : $('.{extendAttributeName}:last');
    div.after('<div class=""form-group form-row {extendAttributeName}""><label class=""col-sm-1 col-form-label text-right""></label><div class=""col-sm-6"">' + inputHtml + '</div><div class=""col-sm-5"">' + btnHtml + '</div></div>');
    if (foucs) $('#{attributeName}_'+index_{attributeName}).focus();

}}
");

            var extendValues = attributes.GetString(extendAttributeName);
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

            return $@"<input id=""{attributeName}"" name=""{attributeName}"" type=""text"" class=""form-control"" value=""{attributes.GetString(attributeName)}"" />";
        }

        private static string ParseFile(IAttributes attributes, SiteInfo siteInfo, int channelId, TableStyleInfo styleInfo, StringBuilder extraBuilder)
        {
            var attributeName = styleInfo.AttributeName;
            var value = attributes.GetString(attributeName);
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
            if (channelId > 0)
            {
                btnAddHtml = $@"
<button class=""btn"" onclick=""add_{attributeName}('',true);return false;"">
    新增
</button>
";
            }

            extraBuilder.Append($@"
<div class=""btn-group btn-group-sm"">
    <button class=""btn"" onclick=""{ModalUploadFile.GetOpenWindowStringToTextBox(siteInfo.Id, EUploadType.File, attributeName)}"">
        上传
    </button>
    <button class=""btn"" onclick=""{ModalSelectFile.GetOpenWindowString(siteInfo.Id, attributeName, relatedPath)}"">
        选择
    </button>
    <button class=""btn"" onclick=""{ModalFileView.GetOpenWindowStringWithTextBoxValue(siteInfo.Id, attributeName)}"">
        查看
    </button>
    {btnAddHtml}
</div>
");

            var extendAttributeName = ContentAttribute.GetExtendAttributeName(attributeName);

            extraBuilder.Append($@"
<script type=""text/javascript"">
function select_{attributeName}(obj, index){{
  var cmd = ""{ModalSelectFile.GetOpenWindowString(siteInfo.Id, attributeName, relatedPath)}"".replace('{attributeName}', '{attributeName}_' + index).replace('return false;', '');
  eval(cmd);
}}
function upload_{attributeName}(obj, index){{
  var cmd = ""{ModalUploadFile.GetOpenWindowStringToTextBox(siteInfo.Id, EUploadType.File,
                attributeName)}"".replace('{attributeName}', '{attributeName}_' + index).replace('return false;', '');
  eval(cmd);
}}
function preview_{attributeName}(obj, index){{
  var cmd = ""{ModalFileView.GetOpenWindowStringWithTextBoxValue(siteInfo.Id,
                attributeName)}"".replace(/{attributeName}/g, '{attributeName}_' + index).replace('return false;', '');
  eval(cmd);
}}
function delete_{attributeName}(obj){{
  $(obj).parent().parent().parent().remove();
}}
var index_{attributeName} = 0;
function add_{attributeName}(val,foucs){{
    index_{attributeName}++;
    var inputHtml = '<input id=""{attributeName}_'+index_{attributeName}+'"" name=""{extendAttributeName}"" type=""text"" class=""form-control"" value=""'+val+'"" />';
    var btnHtml = '<div class=""btn-group btn-group-sm"">';
    btnHtml += '<button class=""btn"" href=""javascript:;"" onclick=""select_{attributeName}(this, '+index_{attributeName}+');return false;"">选择</button>';
    btnHtml += '<button class=""btn"" href=""javascript:;"" onclick=""upload_{attributeName}(this, '+index_{attributeName}+');return false;"">上传</button>';
    btnHtml += '<button class=""btn"" href=""javascript:;"" onclick=""preview_{attributeName}(this, '+index_{attributeName}+');return false;"">查看</button>';
    btnHtml += '<button class=""btn"" href=""javascript:;"" onclick=""delete_{attributeName}(this);return false;"">删除</button>';
    btnHtml += '</div>';
    var div = $('.{extendAttributeName}').length == 0 ? $('#{attributeName}').parent().parent() : $('.{extendAttributeName}:last');
    div.after('<div class=""form-group form-row {extendAttributeName}""><label class=""col-sm-1 col-form-label text-right""></label><div class=""col-sm-6"">' + inputHtml + '</div><div class=""col-sm-5"">' + btnHtml + '</div></div>');
    if (foucs) $('#{attributeName}_'+index_{attributeName}).focus();
}}
");

            var extendValues = attributes.GetString(extendAttributeName);
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
                $@"<input id=""{attributeName}"" name=""{attributeName}"" type=""text"" class=""form-control"" value=""{value}"" />";
        }

        private static string ParseCustomize(IAttributes attributes, TableStyleInfo styleInfo, StringBuilder extraBuilder)
        {
            if (styleInfo.Additional.IsValidate)
            {
                extraBuilder.Append(
                    $@"<span id=""{styleInfo.AttributeName}_msg"" style=""color:red;display:none;"">*</span><script>event_observe('{styleInfo.AttributeName}', 'blur', checkAttributeValue);</script>");
            }

            var value = attributes.GetString(styleInfo.AttributeName);
            var left = styleInfo.Additional.CustomizeLeft.Replace(Value, value);
            var right = styleInfo.Additional.CustomizeRight.Replace(Value, value);

            extraBuilder.Append(right);
            return left;
        }

        public static Dictionary<string, object> SaveAttributes(SiteInfo siteInfo, List<TableStyleInfo> styleInfoList, NameValueCollection formCollection, List<string> dontAddAttributes)
        {
            var dict = new Dictionary<string, object>();

            if (dontAddAttributes == null)
            {
                dontAddAttributes = new List<string>();
            }

            foreach (var styleInfo in styleInfoList)
            {
                if (StringUtils.ContainsIgnoreCase(dontAddAttributes, styleInfo.AttributeName)) continue;
                //var theValue = GetValueByForm(styleInfo, siteInfo, formCollection);

                var theValue = formCollection[styleInfo.AttributeName] ?? string.Empty;
                var inputType = styleInfo.InputType;
                if (inputType == InputType.TextEditor)
                {
                    theValue = ContentUtility.TextEditorContentEncode(siteInfo, theValue);
                    theValue = UEditorUtils.TranslateToStlElement(theValue);
                }

                if (inputType != InputType.TextEditor && inputType != InputType.Image && inputType != InputType.File && inputType != InputType.Video && styleInfo.AttributeName != ContentAttribute.LinkUrl)
                {
                    theValue = AttackUtils.FilterXss(theValue);
                }

                dict[styleInfo.AttributeName] = theValue;

                if (styleInfo.Additional.IsFormatString)
                {
                    var formatString = TranslateUtils.ToBool(formCollection[styleInfo.AttributeName + "_formatStrong"]);
                    var formatEm = TranslateUtils.ToBool(formCollection[styleInfo.AttributeName + "_formatEM"]);
                    var formatU = TranslateUtils.ToBool(formCollection[styleInfo.AttributeName + "_formatU"]);
                    var formatColor = formCollection[styleInfo.AttributeName + "_formatColor"];
                    var theFormatString = ContentUtility.GetTitleFormatString(formatString, formatEm, formatU, formatColor);

                    dict[ContentAttribute.GetFormatStringAttributeName(styleInfo.AttributeName)] = theFormatString;
                }

                if (inputType == InputType.Image || inputType == InputType.File || inputType == InputType.Video)
                {
                    var attributeName = ContentAttribute.GetExtendAttributeName(styleInfo.AttributeName);
                    dict[attributeName] = formCollection[attributeName];
                }
            }

            return dict;
        }

        //public static void SaveAttributes(IAttributes attributes, SiteInfo siteInfo, List<TableStyleInfo> styleInfoList, NameValueCollection formCollection, List<string> dontAddAttributes)
        //{
        //    if (dontAddAttributes == null)
        //    {
        //        dontAddAttributes = new List<string>();
        //    }

        //    foreach (var styleInfo in styleInfoList)
        //    {
        //        if (StringUtils.ContainsIgnoreCase(dontAddAttributes, styleInfo.AttributeName)) continue;
        //        //var theValue = GetValueByForm(styleInfo, siteInfo, formCollection);

        //        var theValue = formCollection[styleInfo.AttributeName] ?? string.Empty;
        //        var inputType = styleInfo.InputType;
        //        if (inputType == InputType.TextEditor)
        //        {
        //            theValue = ContentUtility.TextEditorContentEncode(siteInfo, theValue);
        //            theValue = UEditorUtils.TranslateToStlElement(theValue);
        //        }

        //        if (inputType != InputType.TextEditor && inputType != InputType.Image && inputType != InputType.File && inputType != InputType.Video && styleInfo.AttributeName != ContentAttribute.LinkUrl)
        //        {
        //            theValue = AttackUtils.FilterSqlAndXss(theValue);
        //        }

        //        attributes.Set(styleInfo.AttributeName, theValue);
        //        //TranslateUtils.SetOrRemoveAttributeLowerCase(attributes, styleInfo.AttributeName, theValue);

        //        if (styleInfo.Additional.IsFormatString)
        //        {
        //            var formatString = TranslateUtils.ToBool(formCollection[styleInfo.AttributeName + "_formatStrong"]);
        //            var formatEm = TranslateUtils.ToBool(formCollection[styleInfo.AttributeName + "_formatEM"]);
        //            var formatU = TranslateUtils.ToBool(formCollection[styleInfo.AttributeName + "_formatU"]);
        //            var formatColor = formCollection[styleInfo.AttributeName + "_formatColor"];
        //            var theFormatString = ContentUtility.GetTitleFormatString(formatString, formatEm, formatU, formatColor);

        //            attributes.Set(ContentAttribute.GetFormatStringAttributeName(styleInfo.AttributeName), theFormatString);
        //            //TranslateUtils.SetOrRemoveAttributeLowerCase(attributes, ContentAttribute.GetFormatStringAttributeName(styleInfo.AttributeName), theFormatString);
        //        }

        //        if (inputType == InputType.Image || inputType == InputType.File || inputType == InputType.Video)
        //        {
        //            var attributeName = ContentAttribute.GetExtendAttributeName(styleInfo.AttributeName);
        //            attributes.Set(attributeName, formCollection[attributeName]);
        //            //TranslateUtils.SetOrRemoveAttributeLowerCase(attributes, attributeName, formCollection[attributeName]);
        //        }
        //    }
        //}
    }
}
