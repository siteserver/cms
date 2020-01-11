using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using SiteServer.BackgroundPages.Ajax;
using SiteServer.CMS.Core;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Api;
using SiteServer.CMS.Api.Sys.Editors;
using SiteServer.CMS.Api.Sys.Stl;
using SiteServer.CMS.Context;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Repositories;
using TableStyle = SiteServer.Abstractions.TableStyle;
using SiteServer.Abstractions;

namespace SiteServer.BackgroundPages.Core
{
    public static class BackgroundInputTypeParser
    {
        public const string Current = "{Current}";
        public const string Value = "{Value}";

        public static string Parse(Site site, int channelId, TableStyle style, IDictionary<string, object> attributes, NameValueCollection pageScripts, out string extraHtml)
        {
            var retVal = string.Empty;
            var extraBuilder = new StringBuilder();
            if (!string.IsNullOrEmpty(style.HelpText))
            {
                extraBuilder.Append($@"<small class=""form-text text-muted"">{style.HelpText}</small>");
            }

            var inputType = style.Type;

            if (inputType == InputType.Text)
            {
                retVal = ParseText(attributes, site, channelId, style, extraBuilder);
            }
            else if (inputType == InputType.TextArea)
            {
                retVal = ParseTextArea(attributes, style, extraBuilder);
            }
            else if (inputType == InputType.TextEditor)
            {
                retVal = ParseTextEditor(attributes, style.AttributeName, site, pageScripts, extraBuilder);
            }
            else if (inputType == InputType.SelectOne)
            {
                retVal = ParseSelectOne(attributes, style, extraBuilder);
            }
            else if (inputType == InputType.SelectMultiple)
            {
                retVal = ParseSelectMultiple(attributes, style, extraBuilder);
            }
            else if (inputType == InputType.SelectCascading)
            {
                retVal = ParseSelectCascading(attributes, site, style, extraBuilder);
            }
            else if (inputType == InputType.CheckBox)
            {
                retVal = ParseCheckBox(attributes, style, extraBuilder);
            }
            else if (inputType == InputType.Radio)
            {
                retVal = ParseRadio(attributes, style, extraBuilder);
            }
            else if (inputType == InputType.Date)
            {
                retVal = ParseDate(attributes, pageScripts, style, extraBuilder);
            }
            else if (inputType == InputType.DateTime)
            {
                retVal = ParseDateTime(attributes, pageScripts, style, extraBuilder);
            }
            else if (inputType == InputType.Image)
            {
                retVal = ParseImage(attributes, site, channelId, style, extraBuilder);
            }
            else if (inputType == InputType.Video)
            {
                retVal = ParseVideo(attributes, site, channelId, style, extraBuilder);
            }
            else if (inputType == InputType.File)
            {
                retVal = ParseFile(attributes, site, channelId, style, extraBuilder);
            }
            else if (inputType == InputType.Customize)
            {
                retVal = ParseCustomize(attributes, style, extraBuilder);
            }
            else if (inputType == InputType.Hidden)
            {
                retVal = string.Empty;
                extraBuilder.Clear();
            }

            extraHtml = extraBuilder.ToString();
            return retVal;
        }

        public static string ParseText(IDictionary<string, object> attributes, Site site, int channelId, TableStyle style, StringBuilder extraBuilder)
        {
            var validateAttributes = InputParserUtils.GetValidateAttributes(style.IsValidate, style.DisplayName, style.IsRequired, style.MinNum, style.MaxNum, style.ValidateType, style.RegExp, style.ErrorMessage);

            if (style.IsValidate)
            {
                extraBuilder.Append(
                    $@"<span id=""{style.AttributeName}_msg"" style=""color:red;display:none;"">*</span><script>event_observe('{style.AttributeName}', 'blur', checkAttributeValue);</script>");
            }

            if (style.IsFormatString)
            {
                var formatStrong = false;
                var formatEm = false;
                var formatU = false;
                var formatColor = string.Empty;
                var formatValues = TranslateUtils.Get<string>(attributes, ContentAttribute.GetFormatStringAttributeName(style.AttributeName));

                if (!string.IsNullOrEmpty(formatValues))
                {
                    ContentUtility.SetTitleFormatControls(formatValues, out formatStrong, out formatEm, out formatU, out formatColor);
                }

                extraBuilder.Append(
                    $@"<a class=""btn"" href=""javascript:;"" onclick=""$('#div_{style.AttributeName}').toggle();return false;""><i class=""icon-text-height""></i></a>
<script type=""text/javascript"">
function {style.AttributeName}_strong(e){{
var e = $(e);
if ($('#{style.AttributeName}_formatStrong').val() == 'true'){{
$('#{style.AttributeName}_formatStrong').val('false');
e.removeClass('btn-success');
}}else{{
$('#{style.AttributeName}_formatStrong').val('true');
e.addClass('btn-success');
}}
}}
function {style.AttributeName}_em(e){{
var e = $(e);
if ($('#{style.AttributeName}_formatEM').val() == 'true'){{
$('#{style.AttributeName}_formatEM').val('false');
e.removeClass('btn-success');
}}else{{
$('#{style.AttributeName}_formatEM').val('true');
e.addClass('btn-success');
}}
}}
function {style.AttributeName}_u(e){{
var e = $(e);
if ($('#{style.AttributeName}_formatU').val() == 'true'){{
$('#{style.AttributeName}_formatU').val('false');
e.removeClass('btn-success');
}}else{{
$('#{style.AttributeName}_formatU').val('true');
e.addClass('btn-success');
}}
}}
function {style.AttributeName}_color(){{
if ($('#{style.AttributeName}_formatColor').val()){{
$('#{style.AttributeName}_colorBtn').css('color', $('#{style.AttributeName}_formatColor').val());
$('#{style.AttributeName}_colorBtn').addClass('btn-success');
}}else{{
$('#{style.AttributeName}_colorBtn').css('color', '');
$('#{style.AttributeName}_colorBtn').removeClass('btn-success');
}}
$('#{style.AttributeName}_colorContainer').hide();
}}
</script>
");

                extraBuilder.Append($@"
<div class=""btn-group btn-group-sm"" style=""float:left;"">
    <button class=""btn{(formatStrong ? @" btn-success" : string.Empty)}"" style=""font-weight:bold;font-size:12px;"" onclick=""{style
                    .AttributeName}_strong(this);return false;"">粗体</button>
    <button class=""btn{(formatEm ? " btn-success" : string.Empty)}"" style=""font-style:italic;font-size:12px;"" onclick=""{style
                    .AttributeName}_em(this);return false;"">斜体</button>
    <button class=""btn{(formatU ? " btn-success" : string.Empty)}"" style=""text-decoration:underline;font-size:12px;"" onclick=""{style
                    .AttributeName}_u(this);return false;"">下划线</button>
    <button class=""btn{(!string.IsNullOrEmpty(formatColor) ? " btn-success" : string.Empty)}"" style=""font-size:12px;"" id=""{style
                    .AttributeName}_colorBtn"" onclick=""$('#{style.AttributeName}_colorContainer').toggle();return false;"">颜色</button>
</div>
<div id=""{style.AttributeName}_colorContainer"" class=""input-append"" style=""float:left;display:none"">
    <input id=""{style.AttributeName}_formatColor"" name=""{style.AttributeName}_formatColor"" class=""input-mini color {{required:false}}"" type=""text"" value=""{formatColor}"" placeholder=""颜色值"">
    <button class=""btn"" type=""button"" onclick=""Title_color();return false;"">确定</button>
</div>
<input id=""{style.AttributeName}_formatStrong"" name=""{style.AttributeName}_formatStrong"" type=""hidden"" value=""{formatStrong
                    .ToString().ToLower()}"" />
<input id=""{style.AttributeName}_formatEM"" name=""{style.AttributeName}_formatEM"" type=""hidden"" value=""{formatEm
                    .ToString().ToLower()}"" />
<input id=""{style.AttributeName}_formatU"" name=""{style.AttributeName}_formatU"" type=""hidden"" value=""{formatU
                    .ToString().ToLower()}"" />
");
            }

            if (channelId > 0 && style.AttributeName == ContentAttribute.Title)
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
                extraBuilder.Replace("[url]", AjaxCmsService.GetTitlesUrl(site.Id, channelId));
            }

            var value = CMS.Context.WebUtils.HtmlDecode(TranslateUtils.Get<string>(attributes, style.AttributeName));

            return
                $@"<input id=""{style.AttributeName}"" name=""{style.AttributeName}"" type=""text"" class=""form-control"" value=""{value}"" {validateAttributes} />";
        }

        public static string ParseTextArea(IDictionary<string, object> attributes, TableStyle tableStyle, StringBuilder extraBuilder)
        {
            if (tableStyle.IsValidate)
            {
                extraBuilder.Append(
                $@"<span id=""{tableStyle.AttributeName}_msg"" style=""color:red;display:none;"">*</span><script>event_observe('{tableStyle.AttributeName}', 'blur', checkAttributeValue);</script>");
            }

            var validateAttributes = InputParserUtils.GetValidateAttributes(tableStyle.IsValidate, tableStyle.DisplayName, tableStyle.IsRequired, tableStyle.MinNum, tableStyle.MaxNum, tableStyle.ValidateType, tableStyle.RegExp, tableStyle.ErrorMessage);

            var height = tableStyle.Height;
            if (height == 0)
            {
                height = 80;
            }
            string style = $@"style=""height:{height}px;""";

            var value = CMS.Context.WebUtils.HtmlDecode(TranslateUtils.Get<string>(attributes, tableStyle.AttributeName));

            return
                $@"<textarea id=""{tableStyle.AttributeName}"" name=""{tableStyle.AttributeName}"" class=""form-control"" {style} {validateAttributes}>{value}</textarea>";
        }

        public static string ParseTextEditor(IDictionary<string, object> attributes, string attributeName, Site site, NameValueCollection pageScripts, StringBuilder extraBuilder)
        {
            var value = TranslateUtils.Get<string>(attributes, attributeName);

            value = ContentUtility.TextEditorContentDecode(site, value, true);
            value = UEditorUtils.TranslateToHtml(value);
            value = CMS.Context.WebUtils.HtmlEncode(value);

            var controllerUrl = ApiRouteUEditor.GetUrl(ApiManager.InnerApiUrl, site.Id);
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

        private static string ParseSelectOne(IDictionary<string, object> attributes, TableStyle style, StringBuilder extraBuilder)
        {
            if (style.IsValidate)
            {
                extraBuilder.Append(
                    $@"<span id=""{style.AttributeName}_msg"" style=""color:red;display:none;"">*</span><script>event_observe('{style.AttributeName}', 'blur', checkAttributeValue);</script>");
            }

            var builder = new StringBuilder();
            var styleItems = style.StyleItems ?? new List<TableStyleItem>();

            var selectedValue = TranslateUtils.Get<string>(attributes, style.AttributeName);

            var validateAttributes = InputParserUtils.GetValidateAttributes(style.IsValidate, style.DisplayName, style.IsRequired, style.MinNum, style.MaxNum, style.ValidateType, style.RegExp, style.ErrorMessage);
            builder.Append($@"<select id=""{style.AttributeName}"" name=""{style.AttributeName}"" class=""form-control""  isListItem=""true"" {validateAttributes}>");

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

        private static string ParseSelectMultiple(IDictionary<string, object> attributes, TableStyle style, StringBuilder extraBuilder)
        {
            if (style.IsValidate)
            {
                extraBuilder.Append(
                    $@"<span id=""{style.AttributeName}_msg"" style=""color:red;display:none;"">*</span><script>event_observe('{style.AttributeName}', 'blur', checkAttributeValue);</script>");
            }

            var builder = new StringBuilder();
            var styleItems = style.StyleItems ?? new List<TableStyleItem>();

            var selectedValues = StringUtils.GetStringList(TranslateUtils.Get<string>(attributes, style.AttributeName));

            var validateAttributes = InputParserUtils.GetValidateAttributes(style.IsValidate, style.DisplayName, style.IsRequired, style.MinNum, style.MaxNum, style.ValidateType, style.RegExp, style.ErrorMessage);
            builder.Append($@"<select id=""{style.AttributeName}"" name=""{style.AttributeName}"" class=""form-control"" isListItem=""true"" multiple  {validateAttributes}>");

            foreach (var styleItem in styleItems)
            {
                var isSelected = selectedValues.Contains(styleItem.ItemValue);
                builder.Append($@"<option value=""{styleItem.ItemValue}"" {(isSelected ? "selected" : string.Empty)}>{styleItem.ItemTitle}</option>");
            }

            builder.Append("</select>");
            return builder.ToString();
        }

        private static string ParseSelectCascading(IDictionary<string, object> attributes, Site site, TableStyle tableStyle, StringBuilder extraBuilder)
        {
            var attributeName = tableStyle.AttributeName;
            var fieldInfo = DataProvider.RelatedFieldRepository.GetRelatedFieldAsync(tableStyle.RelatedFieldId).GetAwaiter().GetResult();
            if (fieldInfo == null) return string.Empty;

            var list = DataProvider.RelatedFieldItemRepository.GetRelatedFieldItemInfoListAsync(tableStyle.RelatedFieldId, 0).GetAwaiter().GetResult();

            var prefixes = TranslateUtils.StringCollectionToStringCollection(fieldInfo.Prefixes);
            var suffixes = TranslateUtils.StringCollectionToStringCollection(fieldInfo.Suffixes);

            var style = ERelatedFieldStyleUtils.GetEnumType(tableStyle.RelatedFieldStyle);

            var builder = new StringBuilder();
            builder.Append($@"
<span id=""c_{attributeName}_1"">
    {prefixes[0]}
    <select name=""{attributeName}"" id=""{attributeName}_1"" class=""select"" onchange=""getRelatedField_{fieldInfo.Id}(2);"">
        <option value="""">请选择</option>");

            var values = TranslateUtils.Get<string>(attributes, attributeName);
            
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
    var attributeName = '{tableStyle.AttributeName}';
    var totalLevel = {fieldInfo.TotalLevel};
    for(i=level;i<=totalLevel;i++){{
        $('#c_' + attributeName + '_' + i).hide();
    }}
    var obj = $('#c_' + attributeName + '_' + (level - 1));
    var itemID = $('option:selected', obj).attr('itemID');
    if (itemID){{
        var url = '{ApiRouteActionsRelatedField.GetUrl(ApiManager.InnerApiUrl, site.Id,
                tableStyle.RelatedFieldId, 0)}' + itemID;
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

        private static string ParseCheckBox(IDictionary<string, object> attributes, TableStyle style, StringBuilder extraBuilder)
        {
            if (style.IsValidate)
            {
                extraBuilder.Append(
                    $@"<span id=""{style.AttributeName}_msg"" style=""color:red;display:none;"">*</span><script>event_observe('{style.AttributeName}', 'blur', checkAttributeValue);</script>");
            }

            var builder = new StringBuilder();

            var styleItems = style.StyleItems ?? new List<TableStyleItem>();

            var checkBoxList = new CheckBoxList
            {
                CssClass = "checkbox checkbox-primary",
                ID = style.AttributeName,
                RepeatDirection = style.Horizontal ? RepeatDirection.Horizontal : RepeatDirection.Vertical,
                RepeatColumns = style.Columns
            };

            var selectedValues = StringUtils.GetStringList(TranslateUtils.Get<string>(attributes, style.AttributeName));

            InputParserUtils.GetValidateAttributesForListItem(checkBoxList, style.IsValidate, style.DisplayName, style.IsRequired, style.MinNum, style.MaxNum, style.ValidateType, style.RegExp, style.ErrorMessage);

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
                builder.Replace($@"name=""{style.AttributeName}${i}""",
                    $@"name=""{style.AttributeName}"" value=""{styleItem.ItemValue}""");
                i++;
            }

            return builder.ToString();
        }

        private static string ParseRadio(IDictionary<string, object> attributes, TableStyle style, StringBuilder extraBuilder)
        {
            if (style.IsValidate)
            {
                extraBuilder.Append(
                    $@"<span id=""{style.AttributeName}_msg"" style=""color:red;display:none;"">*</span><script>event_observe('{style.AttributeName}', 'blur', checkAttributeValue);</script>");
            }

            var builder = new StringBuilder();

            var styleItems = style.StyleItems;
            if (styleItems == null || styleItems.Count == 0)
            {
                styleItems = new List<TableStyleItem>
                {
                    new TableStyleItem
                    {
                        ItemTitle = "是",
                        ItemValue = "1"
                    },
                    new TableStyleItem
                    {
                        ItemTitle = "否",
                        ItemValue = "0"
                    }
                };
            }
            var radioButtonList = new RadioButtonList
            {
                CssClass = "radio radio-primary",
                ID = style.AttributeName,
                RepeatDirection = style.Horizontal ? RepeatDirection.Horizontal : RepeatDirection.Vertical,
                RepeatColumns = style.Columns
            };

            var selectedValue = TranslateUtils.Get<string>(attributes, style.AttributeName);

            InputParserUtils.GetValidateAttributesForListItem(radioButtonList, style.IsValidate, style.DisplayName, style.IsRequired, style.MinNum, style.MaxNum, style.ValidateType, style.RegExp, style.ErrorMessage);

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

        private static string ParseDate(IDictionary<string, object> attributes, NameValueCollection pageScripts, TableStyle style, StringBuilder extraBuilder)
        {
            if (style.IsValidate)
            {
                extraBuilder.Append(
                    $@"<span id=""{style.AttributeName}_msg"" style=""color:red;display:none;"">*</span><script>event_observe('{style.AttributeName}', 'blur', checkAttributeValue);</script>");
            }

            var selectedValue = TranslateUtils.Get<string>(attributes, style.AttributeName);
            var dateTime = selectedValue == Current ? DateTime.Now : TranslateUtils.ToDateTime(selectedValue);

            if (pageScripts != null)
            {
                pageScripts["calendar"] =
                    $@"<script language=""javascript"" src=""{SiteServerAssets.GetUrl(SiteServerAssets.DatePicker.Js)}""></script>";
            }

            var value = string.Empty;
            if (dateTime > Constants.SqlMinValue)
            {
                value = DateUtils.GetDateString(dateTime);
            }

            return
                $@"<input id=""{style.AttributeName}"" name=""{style.AttributeName}"" type=""text"" class=""form-control"" value=""{value}"" onfocus=""{SiteServerAssets.DatePicker.OnFocusDateOnly}"" style=""width: 180px"" />";
        }

        private static string ParseDateTime(IDictionary<string, object> attributes, NameValueCollection pageScripts, TableStyle style, StringBuilder extraBuilder)
        {
            if (style.IsValidate)
            {
                extraBuilder.Append(
                    $@"<span id=""{style.AttributeName}_msg"" style=""color:red;display:none;"">*</span><script>event_observe('{style.AttributeName}', 'blur', checkAttributeValue);</script>");
            }

            var selectedValue = TranslateUtils.Get<string>(attributes, style.AttributeName);
            var dateTime = selectedValue == Current ? DateTime.Now : TranslateUtils.ToDateTime(selectedValue);

            if (pageScripts != null)
            {
                pageScripts["calendar"] =
                    $@"<script type=""text/javascript"" src=""{SiteServerAssets.GetUrl(SiteServerAssets.DatePicker.Js)}""></script>";
            }

            var value = string.Empty;
            if (dateTime > Constants.SqlMinValue)
            {
                value = DateUtils.GetDateAndTimeString(dateTime, EDateFormatType.Day, ETimeFormatType.LongTime);
            }

            return $@"<input id=""{style.AttributeName}"" name=""{style.AttributeName}"" type=""text"" class=""form-control"" value=""{value}"" onfocus=""{SiteServerAssets.DatePicker.OnFocus}"" style=""width: 180px"" />";
        }

        private static string ParseImage(IDictionary<string, object> attributes, Site site, int channelId, TableStyle style, StringBuilder extraBuilder)
        {
            var btnAddHtml = string.Empty;

            if (channelId > 0)
            {
                btnAddHtml = $@"
    <button class=""btn"" onclick=""add_{style.AttributeName}('',true);return false;"">
        新增
    </button>
";
            }

            extraBuilder.Append($@"
<div class=""btn-group btn-group-sm"">
    <button class=""btn"" onclick=""{ModalUploadImage.GetOpenWindowString(site.Id, style.AttributeName)}"">
        上传
    </button>
    <button class=""btn"" onclick=""{ModalSelectImage.GetOpenWindowString(site, style.AttributeName)}"">
        选择
    </button>
    <button class=""btn"" onclick=""{ModalCuttingImage.GetOpenWindowStringWithTextBox(site.Id, style.AttributeName)}"">
        裁切
    </button>
    <button class=""btn"" onclick=""{ModalMessage.GetOpenWindowStringToPreviewImage(site.Id, style.AttributeName)}"">
        预览
    </button>
    {btnAddHtml}
</div>
");

            var attributeName = style.AttributeName;
            var extendAttributeName = ContentAttribute.GetExtendAttributeName(style.AttributeName);

            extraBuilder.Append($@"
<script type=""text/javascript"">
function select_{style.AttributeName}(obj, index){{
  var cmd = ""{ModalSelectImage.GetOpenWindowString(site, style.AttributeName)}"".replace('{attributeName}', '{attributeName}_' + index).replace('return false;', '');
  eval(cmd);
}}
function upload_{attributeName}(obj, index){{
  var cmd = ""{ModalUploadImage.GetOpenWindowString(site.Id, attributeName)}"".replace('{attributeName}', '{attributeName}_' + index).replace('return false;', '');
  eval(cmd);
}}
function cutting_{attributeName}(obj, index){{
  var cmd = ""{ModalCuttingImage.GetOpenWindowStringWithTextBox(site.Id, attributeName)}"".replace('{attributeName}', '{attributeName}_' + index).replace('return false;', '');
  eval(cmd);
}}
function preview_{attributeName}(obj, index){{
  var cmd = ""{ModalMessage.GetOpenWindowStringToPreviewImage(site.Id, attributeName)}"".replace(/{attributeName}/g, '{attributeName}_' + index).replace('return false;', '');
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

            var extendValues = TranslateUtils.Get<string>(attributes, extendAttributeName);
            if (!string.IsNullOrEmpty(extendValues))
            {
                foreach (var extendValue in StringUtils.GetStringList(extendValues))
                {
                    if (!string.IsNullOrEmpty(extendValue))
                    {
                        extraBuilder.Append($"add_{attributeName}('{extendValue}',false);");
                    }
                }
            }

            extraBuilder.Append("</script>");

            return $@"<input id=""{attributeName}"" name=""{attributeName}"" type=""text"" class=""form-control"" value=""{TranslateUtils.Get<string>(attributes, attributeName)}"" />";
        }

        private static string ParseVideo(IDictionary<string, object> attributes, Site site, int channelId, TableStyle style, StringBuilder extraBulder)
        {
            var attributeName = style.AttributeName;

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
    <button class=""btn"" onclick=""{ModalUploadVideo.GetOpenWindowStringToTextBox(site.Id, attributeName)}"">
        上传
    </button>
    <button class=""btn"" onclick=""{ModalSelectVideo.GetOpenWindowString(site, attributeName)}"">
        选择
    </button>
    <button class=""btn"" onclick=""{ModalMessage.GetOpenWindowStringToPreviewVideo(site.Id, attributeName)}"">
        预览
    </button>
    {btnAddHtml}
</div>");

            var extendAttributeName = ContentAttribute.GetExtendAttributeName(attributeName);

            extraBulder.Append($@"
<script type=""text/javascript"">
function select_{attributeName}(obj, index){{
  var cmd = ""{ModalSelectVideo.GetOpenWindowString(site, attributeName)}"".replace('{attributeName}', '{attributeName}_' + index).replace('return false;', '');
  eval(cmd);
}}
function upload_{attributeName}(obj, index){{
  var cmd = ""{ModalUploadVideo.GetOpenWindowStringToTextBox(site.Id, attributeName)}"".replace('{attributeName}', '{attributeName}_' + index).replace('return false;', '');
  eval(cmd);
}}
function preview_{attributeName}(obj, index){{
  var cmd = ""{ModalMessage.GetOpenWindowStringToPreviewVideo(site.Id, attributeName)}"".replace(/{attributeName}/g, '{attributeName}_' + index).replace('return false;', '');
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

            var extendValues = TranslateUtils.Get<string>(attributes, extendAttributeName);
            if (!string.IsNullOrEmpty(extendValues))
            {
                foreach (var extendValue in StringUtils.GetStringList(extendValues))
                {
                    if (!string.IsNullOrEmpty(extendValue))
                    {
                        extraBulder.Append($"add_{attributeName}('{extendValue}',false);");
                    }
                }
            }

            extraBulder.Append("</script>");

            return $@"<input id=""{attributeName}"" name=""{attributeName}"" type=""text"" class=""form-control"" value=""{TranslateUtils.Get<string>(attributes, attributeName)}"" />";
        }

        private static string ParseFile(IDictionary<string, object> attributes, Site site, int channelId, TableStyle style, StringBuilder extraBuilder)
        {
            var attributeName = style.AttributeName;
            var value = TranslateUtils.Get<string>(attributes, attributeName);
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
    <button class=""btn"" onclick=""{ModalUploadFile.GetOpenWindowStringToTextBox(site.Id, UploadType.File, attributeName)}"">
        上传
    </button>
    <button class=""btn"" onclick=""{ModalSelectFile.GetOpenWindowString(site.Id, attributeName, relatedPath)}"">
        选择
    </button>
    <button class=""btn"" onclick=""{ModalFileView.GetOpenWindowStringWithTextBoxValue(site.Id, attributeName)}"">
        查看
    </button>
    {btnAddHtml}
</div>
");

            var extendAttributeName = ContentAttribute.GetExtendAttributeName(attributeName);

            extraBuilder.Append($@"
<script type=""text/javascript"">
function select_{attributeName}(obj, index){{
  var cmd = ""{ModalSelectFile.GetOpenWindowString(site.Id, attributeName, relatedPath)}"".replace('{attributeName}', '{attributeName}_' + index).replace('return false;', '');
  eval(cmd);
}}
function upload_{attributeName}(obj, index){{
  var cmd = ""{ModalUploadFile.GetOpenWindowStringToTextBox(site.Id, UploadType.File,
                attributeName)}"".replace('{attributeName}', '{attributeName}_' + index).replace('return false;', '');
  eval(cmd);
}}
function preview_{attributeName}(obj, index){{
  var cmd = ""{ModalFileView.GetOpenWindowStringWithTextBoxValue(site.Id,
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

            var extendValues = TranslateUtils.Get<string>(attributes, extendAttributeName);
            if (!string.IsNullOrEmpty(extendValues))
            {
                foreach (var extendValue in StringUtils.GetStringList(extendValues))
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

        private static string ParseCustomize(IDictionary<string, object> attributes, TableStyle style, StringBuilder extraBuilder)
        {
            if (style.IsValidate)
            {
                extraBuilder.Append(
                    $@"<span id=""{style.AttributeName}_msg"" style=""color:red;display:none;"">*</span><script>event_observe('{style.AttributeName}', 'blur', checkAttributeValue);</script>");
            }

            var value = TranslateUtils.Get<string>(attributes, style.AttributeName);
            var left = style.CustomizeLeft.Replace(Value, value);
            var right = style.CustomizeRight.Replace(Value, value);

            extraBuilder.Append(right);
            return left;
        }

        public static async Task<Dictionary<string, object>> SaveAttributesAsync(Site site, List<TableStyle> styleList, NameValueCollection formCollection, List<string> dontAddAttributes)
        {
            var dict = new Dictionary<string, object>();

            if (dontAddAttributes == null)
            {
                dontAddAttributes = new List<string>();
            }

            foreach (var style in styleList)
            {
                if (StringUtils.ContainsIgnoreCase(dontAddAttributes, style.AttributeName)) continue;
                //var theValue = GetValueByForm(style, site, formCollection);

                var theValue = formCollection[style.AttributeName] ?? string.Empty;
                var inputType = style.Type;
                if (inputType == InputType.TextEditor)
                {
                    theValue = await ContentUtility.TextEditorContentEncodeAsync(site, theValue);
                    theValue = UEditorUtils.TranslateToStlElement(theValue);
                }

                if (inputType != InputType.TextEditor && inputType != InputType.Image && inputType != InputType.File && inputType != InputType.Video && style.AttributeName != ContentAttribute.LinkUrl)
                {
                    theValue = AttackUtils.FilterXss(theValue);
                }

                dict[style.AttributeName] = theValue;

                if (style.IsFormatString)
                {
                    var formatString = TranslateUtils.ToBool(formCollection[style.AttributeName + "_formatStrong"]);
                    var formatEm = TranslateUtils.ToBool(formCollection[style.AttributeName + "_formatEM"]);
                    var formatU = TranslateUtils.ToBool(formCollection[style.AttributeName + "_formatU"]);
                    var formatColor = formCollection[style.AttributeName + "_formatColor"];
                    var theFormatString = ContentUtility.GetTitleFormatString(formatString, formatEm, formatU, formatColor);

                    dict[ContentAttribute.GetFormatStringAttributeName(style.AttributeName)] = theFormatString;
                }

                if (inputType == InputType.Image || inputType == InputType.File || inputType == InputType.Video)
                {
                    var attributeName = ContentAttribute.GetExtendAttributeName(style.AttributeName);
                    dict[attributeName] = formCollection[attributeName];
                }
            }

            return dict;
        }

        //public static void SaveAttributes(IAttributes attributes, Site site, List<TableStyle> styleList, NameValueCollection formCollection, List<string> dontAddAttributes)
        //{
        //    if (dontAddAttributes == null)
        //    {
        //        dontAddAttributes = new List<string>();
        //    }

        //    foreach (var style in styleList)
        //    {
        //        if (StringUtils.ContainsIgnoreCase(dontAddAttributes, style.AttributeName)) continue;
        //        //var theValue = GetValueByForm(style, site, formCollection);

        //        var theValue = formCollection[style.AttributeName] ?? string.Empty;
        //        var inputType = style.InputType;
        //        if (inputType == InputType.TextEditor)
        //        {
        //            theValue = ContentUtility.TextEditorContentEncode(site, theValue);
        //            theValue = UEditorUtils.TranslateToStlElement(theValue);
        //        }

        //        if (inputType != InputType.TextEditor && inputType != InputType.Image && inputType != InputType.File && inputType != InputType.Video && style.AttributeName != ContentAttribute.LinkUrl)
        //        {
        //            theValue = AttackUtils.FilterSqlAndXss(theValue);
        //        }

        //        attributes.Set(style.AttributeName, theValue);
        //        //TranslateUtils.SetOrRemoveAttributeLowerCase(attributes, style.AttributeName, theValue);

        //        if (style.IsFormatString)
        //        {
        //            var formatString = TranslateUtils.ToBool(formCollection[style.AttributeName + "_formatStrong"]);
        //            var formatEm = TranslateUtils.ToBool(formCollection[style.AttributeName + "_formatEM"]);
        //            var formatU = TranslateUtils.ToBool(formCollection[style.AttributeName + "_formatU"]);
        //            var formatColor = formCollection[style.AttributeName + "_formatColor"];
        //            var theFormatString = ContentUtility.GetTitleFormatString(formatString, formatEm, formatU, formatColor);

        //            attributes.Set(ContentAttribute.GetFormatStringAttributeName(style.AttributeName), theFormatString);
        //            //TranslateUtils.SetOrRemoveAttributeLowerCase(attributes, ContentAttribute.GetFormatStringAttributeName(style.AttributeName), theFormatString);
        //        }

        //        if (inputType == InputType.Image || inputType == InputType.File || inputType == InputType.Video)
        //        {
        //            var attributeName = ContentAttribute.GetExtendAttributeName(style.AttributeName);
        //            attributes.Set(attributeName, formCollection[attributeName]);
        //            //TranslateUtils.SetOrRemoveAttributeLowerCase(attributes, attributeName, formCollection[attributeName]);
        //        }
        //    }
        //}
    }
}
