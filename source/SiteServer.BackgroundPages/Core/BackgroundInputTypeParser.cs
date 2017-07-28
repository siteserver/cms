using System;
using System.Collections;
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
using SiteServer.BackgroundPages.Ajax;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Wcm.GovInteract;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.BackgroundPages.Core
{
    public class BackgroundInputTypeParser
    {
        private BackgroundInputTypeParser()
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
                retval = ParseDate(attributeName, formCollection, isAddAndNotPostBack, additionalAttributes, pageScripts, styleInfo);
            }
            else if (EInputTypeUtils.Equals(styleInfo.InputType, EInputType.DateTime))
            {
                retval = ParseDateTime(attributeName, formCollection, isAddAndNotPostBack, additionalAttributes, pageScripts, styleInfo);
            }
            else if (EInputTypeUtils.Equals(styleInfo.InputType, EInputType.Image))
            {
                retval = ParseImage(publishmentSystemInfo, attributeName, formCollection, isAddAndNotPostBack, additionalAttributes, styleInfo, tableStyle);
            }
            else if (EInputTypeUtils.Equals(styleInfo.InputType, EInputType.Video))
            {
                retval = ParseVideo(publishmentSystemInfo, attributeName, formCollection, isAddAndNotPostBack, additionalAttributes, styleInfo, tableStyle);
            }
            else if (EInputTypeUtils.Equals(styleInfo.InputType, EInputType.File))
            {
                retval = ParseFile(publishmentSystemInfo, attributeName, formCollection, isAddAndNotPostBack, additionalAttributes, styleInfo, tableStyle);
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

            if (styleInfo.Additional.IsFormatString)
            {
                var isFormatted = false;
                var formatStrong = false;
                var formatEm = false;
                var formatU = false;
                var formatColor = string.Empty;
                if (!isAddAndNotPostBack)
                {
                    if (formCollection?[ContentAttribute.GetFormatStringAttributeName(attributeName)] != null)
                    {
                        isFormatted = ContentUtility.SetTitleFormatControls(formCollection[ContentAttribute.GetFormatStringAttributeName(attributeName)], out formatStrong, out formatEm, out formatU, out formatColor);
                    }
                }

                builder.Append(string.Format(@"<a class=""btn"" href=""javascript:;"" onclick=""$('#div_{0}').toggle();return false;""><i class=""icon-text-height""></i></a>
<script type=""text/javascript"">
function {0}_strong(e){{
var e = $(e);
if ($('#{0}_formatStrong').val() == 'true'){{
$('#{0}_formatStrong').val('false');
e.removeClass('btn-success');
}}else{{
$('#{0}_formatStrong').val('true');
e.addClass('btn-success');
}}
}}
function {0}_em(e){{
var e = $(e);
if ($('#{0}_formatEM').val() == 'true'){{
$('#{0}_formatEM').val('false');
e.removeClass('btn-success');
}}else{{
$('#{0}_formatEM').val('true');
e.addClass('btn-success');
}}
}}
function {0}_u(e){{
var e = $(e);
if ($('#{0}_formatU').val() == 'true'){{
$('#{0}_formatU').val('false');
e.removeClass('btn-success');
}}else{{
$('#{0}_formatU').val('true');
e.addClass('btn-success');
}}
}}
function {0}_color(){{
if ($('#{0}_formatColor').val()){{
$('#{0}_colorBtn').css('color', $('#{0}_formatColor').val());
$('#{0}_colorBtn').addClass('btn-success');
}}else{{
$('#{0}_colorBtn').css('color', '');
$('#{0}_colorBtn').removeClass('btn-success');
}}
$('#{0}_colorContainer').hide();
}}
</script>
", styleInfo.AttributeName));

                builder.Append(string.Format(@"
<div id=""div_{0}"" style=""display:{1};margin-top:5px;"">
<div class=""btn-group"" style=""float:left;"">
    <button class=""btn{5}"" style=""font-weight:bold;font-size:12px;"" onclick=""{0}_strong(this);return false;"">粗体</button>
    <button class=""btn{6}"" style=""font-style:italic;font-size:12px;"" onclick=""{0}_em(this);return false;"">斜体</button>
    <button class=""btn{7}"" style=""text-decoration:underline;font-size:12px;"" onclick=""{0}_u(this);return false;"">下划线</button>
    <button class=""btn{8}"" style=""font-size:12px;"" id=""{0}_colorBtn"" onclick=""$('#{0}_colorContainer').toggle();return false;"">颜色</button>
</div>
<div id=""{0}_colorContainer"" class=""input-append"" style=""float:left;display:none"">
    <input id=""{0}_formatColor"" name=""{0}_formatColor"" class=""input-mini color {{required:false}}"" type=""text"" value=""{9}"" placeholder=""颜色值"">
    <button class=""btn"" type=""button"" onclick=""Title_color();return false;"">确定</button>
</div>
<input id=""{0}_formatStrong"" name=""{0}_formatStrong"" type=""hidden"" value=""{2}"" />
<input id=""{0}_formatEM"" name=""{0}_formatEM"" type=""hidden"" value=""{3}"" />
<input id=""{0}_formatU"" name=""{0}_formatU"" type=""hidden"" value=""{4}"" />
</div>
", styleInfo.AttributeName, isFormatted ? string.Empty : "none", formatStrong.ToString().ToLower(), formatEm.ToString().ToLower(), formatU.ToString().ToLower(), formatStrong ? @" btn-success" : string.Empty, formatEm ? " btn-success" : string.Empty, formatU ? " btn-success" : string.Empty, !string.IsNullOrEmpty(formatColor) ? " btn-success" : string.Empty, formatColor));
            }

            if (nodeId > 0 && (tableStyle == ETableStyle.BackgroundContent || tableStyle == ETableStyle.GovInteractContent || tableStyle == ETableStyle.GovPublicContent || tableStyle == ETableStyle.VoteContent) && styleInfo.AttributeName == ContentAttribute.Title)
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

            builder.Append(string.Format(@"<textarea id=""{0}"" name=""{0}"" class=""textarea"" {1} {2} {3}>{4}</textarea>", attributeName, additionalAttributes, style, validateAttributes, value));

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
            value = StringUtility.TextEditorContentDecode(value, publishmentSystemInfo, true);
            value = ETextEditorTypeUtils.TranslateToHtml(value);
            value = StringUtils.HtmlEncode(value);

            var builder = new StringBuilder();

            var controllerUrl = CMS.Controllers.Files.UEditor.GetUrl(PageUtils.GetApiUrl(), publishmentSystemInfo.PublishmentSystemId);
            var editorUrl = SiteFilesAssets.GetUrl(PageUtils.GetApiUrl(), "ueditor");

            if (pageScripts["uEditor"] == null)
            {
                builder.Append(string.Format(@"<script type=""text/javascript"">window.UEDITOR_HOME_URL = ""{0}/"";window.UEDITOR_CONTROLLER_URL = ""{1}"";</script><script type=""text/javascript"" src=""{0}/editor_config.js""></script><script type=""text/javascript"" src=""{0}/ueditor_all_min.js""></script>", editorUrl, controllerUrl));
            }
            pageScripts["uEditor"] = string.Empty;

            builder.Append(string.Format(@"
<textarea id=""{0}"" name=""{0}"" style=""display:none"">{1}</textarea>
<script type=""text/javascript"">
$(function(){{
  UE.getEditor('{0}', {{allowDivTransToP: false}});
  $('#{0}').show();
}});
</script>", attributeName, value));

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

            builder.Append(string.Format(@"<input id=""{0}"" name=""{0}"" type=""text"" class=""input_text"" value=""{1}"" {2} onfocus=""{3}"" />", attributeName, value, additionalAttributes, SiteServerAssets.DatePicker.OnFocusDateOnly));

            AddHelpText(builder, styleInfo.HelpText);

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

            builder.Append(string.Format(@"<input id=""{0}"" name=""{0}"" type=""text"" class=""input_text"" value=""{1}"" {2} onfocus=""{3}"" />", attributeName, value, additionalAttributes, SiteServerAssets.DatePicker.OnFocus));

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
            builder.Append(string.Format(@"<select id=""{0}"" name=""{0}"" class=""select""  isListItem=""true"" {1}>", attributeName, validateAttributes));
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
            builder.Append(string.Format(@"<select id=""{0}"" name=""{0}"" class=""select_multiple""  isListItem=""true"" multiple  {1}>", attributeName, validateAttributes));
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

        private static string ParseImage(PublishmentSystemInfo publishmentSystemInfo, string attributeName, NameValueCollection formCollection, bool isAddAndNotPostBack, string additionalAttributes, TableStyleInfo styleInfo, ETableStyle tableStyle)
        {
            var builder = new StringBuilder();

            var value = GetValue(attributeName, formCollection, isAddAndNotPostBack, string.Empty);

            var width = styleInfo.Additional.Width;
            if (string.IsNullOrEmpty(width))
            {
                width = styleInfo.IsSingleLine ? "380px" : "220px";
            }
            string style = $@"style=""width:{TranslateUtils.ToWidth(width)};""";

            var btnAddHtml = string.Empty;
            if (ETableStyleUtils.IsContent(tableStyle))
            {
                btnAddHtml = $@"
    <a class=""btn"" href=""javascript:;"" onclick=""add_{attributeName}('',true)"" title=""新增""><i class=""icon-plus""></i></a>
";
            }

            builder.Append(string.Format(@"
<div class=""clearfix"">
  <div class=""pull-left"">
    <input id=""{0}"" name=""{0}"" type=""text"" class=""input_text"" value=""{1}"" {2} {3} />&nbsp;
  </div>
  <div class=""pull-left btn-group"">
    <a class=""btn"" href=""javascript:;"" onclick=""{4}"" title=""选择""><i class=""icon-th""></i></a>
    <a class=""btn"" href=""javascript:;"" onclick=""{5}"" title=""上传""><i class=""icon-arrow-up""></i></a>
    <a class=""btn"" href=""javascript:;"" onclick=""{6}"" title=""裁切""><i class=""icon-crop""></i></a>
    <a class=""btn"" href=""javascript:;"" onclick=""{7}"" title=""预览""><i class=""icon-eye-open""></i></a>
    {8}
  </div>
</div>
", attributeName, value, additionalAttributes, style, ModalSelectImage.GetOpenWindowString(publishmentSystemInfo, attributeName), ModalUploadImage.GetOpenWindowString(publishmentSystemInfo.PublishmentSystemId, attributeName), ModalCuttingImage.GetOpenWindowStringWithTextBox(publishmentSystemInfo.PublishmentSystemId, attributeName), ModalMessage.GetOpenWindowStringToPreviewImage(publishmentSystemInfo.PublishmentSystemId, attributeName), btnAddHtml));

            var extendAttributeName = ContentAttribute.GetExtendAttributeName(attributeName);

            builder.Append(string.Format(@"
<script type=""text/javascript"">
function select_{0}(obj, index){{
  var cmd = ""{1}"".replace('{0}', '{0}_' + index).replace('return false;', '');
  eval(cmd);
}}
function upload_{0}(obj, index){{
  var cmd = ""{2}"".replace('{0}', '{0}_' + index).replace('return false;', '');
  eval(cmd);
}}
function cutting_{0}(obj, index){{
  var cmd = ""{3}"".replace('{0}', '{0}_' + index).replace('return false;', '');
  eval(cmd);
}}
function preview_{0}(obj, index){{
  var cmd = ""{4}"".replace(/{0}/g, '{0}_' + index).replace('return false;', '');
  eval(cmd);
}}
function delete_{0}(obj){{
  $(obj).closest('tr').remove();
}}
var index_{0} = 0;
function add_{0}(val,foucs){{
    index_{0}++;
    var html = '<div class=""clearfix""><div class=""pull-left"">';
    html += '<input id=""{0}_'+index_{0}+'"" name=""{5}"" type=""text"" class=""input_text"" value=""'+val+'"" {6} {7} />&nbsp;';
    html += '</div>';
    html += '<div class=""pull-left btn-group"">';
    html += '<a class=""btn"" href=""javascript:;"" onclick=""select_{0}(this, '+index_{0}+')"" title=""选择""><i class=""icon-th""></i></a>';
    html += '<a class=""btn"" href=""javascript:;"" onclick=""upload_{0}(this, '+index_{0}+')"" title=""上传""><i class=""icon-arrow-up""></i></a>';
    html += '<a class=""btn"" href=""javascript:;"" onclick=""cutting_{0}(this, '+index_{0}+')"" title=""裁切""><i class=""icon-crop""></i></a>';
    html += '<a class=""btn"" href=""javascript:;"" onclick=""preview_{0}(this, '+index_{0}+')"" title=""预览""><i class=""icon-eye-open""></i></a>';
    html += '<a class=""btn"" href=""javascript:;"" onclick=""delete_{0}(this)"" title=""删除""><i class=""icon-remove""></i></a>';
    html += '</div></div>';
    var tr = $('.{5}').length == 0 ? $('#{0}').closest('tr') : $('.{5}:last');
    tr.after('<tr class=""{5}""><td>&nbsp;</td><td colspan=""3"">'+html+'</td></tr>');
    if (foucs) $('#{0}_'+index_{0}).focus();
}}
", attributeName, ModalSelectImage.GetOpenWindowString(publishmentSystemInfo, attributeName), ModalUploadImage.GetOpenWindowString(publishmentSystemInfo.PublishmentSystemId, attributeName), ModalCuttingImage.GetOpenWindowStringWithTextBox(publishmentSystemInfo.PublishmentSystemId, attributeName), ModalMessage.GetOpenWindowStringToPreviewImage(publishmentSystemInfo.PublishmentSystemId, attributeName), extendAttributeName, additionalAttributes, style));

            var extendValues = formCollection[extendAttributeName];
            if (!string.IsNullOrEmpty(extendValues))
            {
                foreach (string extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
                {
                    if (!string.IsNullOrEmpty(extendValue))
                    {
                        builder.Append($"add_{attributeName}('{extendValue}',false);");
                    }
                }
            }

            builder.Append("</script>");

            AddHelpText(builder, styleInfo.HelpText);

            return builder.ToString();
        }

        public static string GetAttributeNameToUploadForTouGao(string attributeName)
        {
            return attributeName + "_uploader";
        }

        private static string ParseVideo(PublishmentSystemInfo publishmentSystemInfo, string attributeName, NameValueCollection formCollection, bool isAddAndNotPostBack, string additionalAttributes, TableStyleInfo styleInfo, ETableStyle tableStyle)
        {
            var builder = new StringBuilder();

            var value = GetValue(attributeName, formCollection, isAddAndNotPostBack, string.Empty);

            var width = styleInfo.Additional.Width;
            if (string.IsNullOrEmpty(width))
            {
                width = styleInfo.IsSingleLine ? "380px" : "220px";
            }
            string style = $@"style=""width:{TranslateUtils.ToWidth(width)};""";

            var btnAddHtml = string.Empty;
            if (ETableStyleUtils.IsContent(tableStyle))
            {
                btnAddHtml = $@"
    <a class=""btn"" href=""javascript:;"" onclick=""add_{attributeName}('',true)"" title=""新增""><i class=""icon-plus""></i></a>
";
            }

            builder.Append(string.Format(@"
<div class=""clearfix"">
  <div class=""pull-left"">
    <input id=""{0}"" name=""{0}"" type=""text"" class=""input_text"" value=""{1}"" {2} {3} />&nbsp;
  </div>
  <div class=""pull-left btn-group"">
    <a class=""btn"" href=""javascript:;"" onclick=""{4}"" title=""选择""><i class=""icon-th""></i></a>
    <a class=""btn"" href=""javascript:;"" onclick=""{5}"" title=""上传""><i class=""icon-arrow-up""></i></a>
    <a class=""btn"" href=""javascript:;"" onclick=""{6}"" title=""预览""><i class=""icon-eye-open""></i></a>
    {7}
  </div>
</div>
", attributeName, value, additionalAttributes, style, ModalSelectVideo.GetOpenWindowString(publishmentSystemInfo, attributeName), ModalUploadVideo.GetOpenWindowStringToTextBox(publishmentSystemInfo.PublishmentSystemId, attributeName), ModalMessage.GetOpenWindowStringToPreviewVideo(publishmentSystemInfo.PublishmentSystemId, attributeName), btnAddHtml));

            var extendAttributeName = ContentAttribute.GetExtendAttributeName(attributeName);

            builder.Append(string.Format(@"
<script type=""text/javascript"">
function select_{0}(obj, index){{
  var cmd = ""{1}"".replace('{0}', '{0}_' + index).replace('return false;', '');
  eval(cmd);
}}
function upload_{0}(obj, index){{
  var cmd = ""{2}"".replace('{0}', '{0}_' + index).replace('return false;', '');
  eval(cmd);
}}
function preview_{0}(obj, index){{
  var cmd = ""{3}"".replace(/{0}/g, '{0}_' + index).replace('return false;', '');
  eval(cmd);
}}
function delete_{0}(obj){{
  $(obj).closest('tr').remove();
}}
var index_{0} = 0;
function add_{0}(val,foucs){{
    index_{0}++;
    var html = '<div class=""clearfix""><div class=""pull-left"">';
    html += '<input id=""{0}_'+index_{0}+'"" name=""{4}"" type=""text"" class=""input_text"" value=""'+val+'"" {5} {6} />&nbsp;';
    html += '</div>';
    html += '<div class=""pull-left btn-group"">';
    html += '<a class=""btn"" href=""javascript:;"" onclick=""select_{0}(this, '+index_{0}+')"" title=""选择""><i class=""icon-th""></i></a>';
    html += '<a class=""btn"" href=""javascript:;"" onclick=""upload_{0}(this, '+index_{0}+')"" title=""上传""><i class=""icon-arrow-up""></i></a>';
    html += '<a class=""btn"" href=""javascript:;"" onclick=""preview_{0}(this, '+index_{0}+')"" title=""预览""><i class=""icon-eye-open""></i></a>';
    html += '<a class=""btn"" href=""javascript:;"" onclick=""delete_{0}(this)"" title=""删除""><i class=""icon-remove""></i></a>';
    html += '</div></div>';
    var tr = $('.{4}').length == 0 ? $('#{0}').closest('tr') : $('.{4}:last');
    tr.after('<tr class=""{4}""><td>&nbsp;</td><td colspan=""3"">'+html+'</td></tr>');
    if (foucs) $('#{0}_'+index_{0}).focus();
}}
", attributeName, ModalSelectVideo.GetOpenWindowString(publishmentSystemInfo, attributeName), ModalUploadVideo.GetOpenWindowStringToTextBox(publishmentSystemInfo.PublishmentSystemId, attributeName), ModalMessage.GetOpenWindowStringToPreviewVideo(publishmentSystemInfo.PublishmentSystemId, attributeName), extendAttributeName, additionalAttributes, style));

            var extendValues = formCollection[extendAttributeName];
            if (!string.IsNullOrEmpty(extendValues))
            {
                foreach (string extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
                {
                    if (!string.IsNullOrEmpty(extendValue))
                    {
                        builder.Append($"add_{attributeName}('{extendValue}',false);");
                    }
                }
            }

            builder.Append("</script>");

            AddHelpText(builder, styleInfo.HelpText);

            return builder.ToString();
        }

        private static string ParseFile(PublishmentSystemInfo publishmentSystemInfo, string attributeName, NameValueCollection formCollection, bool isAddAndNotPostBack, string additionalAttributes, TableStyleInfo styleInfo, ETableStyle tableStyle)
        {
            var builder = new StringBuilder();

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

            var width = styleInfo.Additional.Width;
            if (string.IsNullOrEmpty(width))
            {
                width = styleInfo.IsSingleLine ? "380px" : "220px";
            }
            string style = $@"style=""width:{TranslateUtils.ToWidth(width)};""";

            var btnAddHtml = string.Empty;
            if (ETableStyleUtils.IsContent(tableStyle))
            {
                btnAddHtml = $@"
    <a class=""btn"" href=""javascript:;"" onclick=""add_{attributeName}('',true)"" title=""新增""><i class=""icon-plus""></i></a>
";
            }

            builder.Append(string.Format(@"
<div class=""clearfix"">
  <div class=""pull-left"">
    <input id=""{0}"" name=""{0}"" type=""text"" class=""input_text"" value=""{1}"" {2} {3} />&nbsp;
  </div>
  <div class=""pull-left btn-group"">
    <a class=""btn"" href=""javascript:;"" onclick=""{4}"" title=""选择""><i class=""icon-th""></i></a>
    <a class=""btn"" href=""javascript:;"" onclick=""{5}"" title=""上传""><i class=""icon-arrow-up""></i></a>
    <a class=""btn"" href=""javascript:;"" onclick=""{6}"" title=""查看""><i class=""icon-eye-open""></i></a>
    {7}
  </div>
</div>
", attributeName, value, additionalAttributes, style, ModalSelectFile.GetOpenWindowString(publishmentSystemInfo.PublishmentSystemId, attributeName, relatedPath), ModalUploadFile.GetOpenWindowStringToTextBox(publishmentSystemInfo.PublishmentSystemId, EUploadType.File, attributeName), ModalFileView.GetOpenWindowStringWithTextBoxValue(publishmentSystemInfo.PublishmentSystemId, attributeName), btnAddHtml));

            var extendAttributeName = ContentAttribute.GetExtendAttributeName(attributeName);

            builder.Append(string.Format(@"
<script type=""text/javascript"">
function select_{0}(obj, index){{
  var cmd = ""{1}"".replace('{0}', '{0}_' + index).replace('return false;', '');
  eval(cmd);
}}
function upload_{0}(obj, index){{
  var cmd = ""{2}"".replace('{0}', '{0}_' + index).replace('return false;', '');
  eval(cmd);
}}
function preview_{0}(obj, index){{
  var cmd = ""{3}"".replace(/{0}/g, '{0}_' + index).replace('return false;', '');
  eval(cmd);
}}
function delete_{0}(obj){{
  $(obj).closest('tr').remove();
}}
var index_{0} = 0;
function add_{0}(val,foucs){{
    index_{0}++;
    var html = '<div class=""clearfix""><div class=""pull-left"">';
    html += '<input id=""{0}_'+index_{0}+'"" name=""{4}"" type=""text"" class=""input_text"" value=""'+val+'"" {5} {6} />&nbsp;';
    html += '</div>';
    html += '<div class=""pull-left btn-group"">';
    html += '<a class=""btn"" href=""javascript:;"" onclick=""select_{0}(this, '+index_{0}+')"" title=""选择""><i class=""icon-th""></i></a>';
    html += '<a class=""btn"" href=""javascript:;"" onclick=""upload_{0}(this, '+index_{0}+')"" title=""上传""><i class=""icon-arrow-up""></i></a>';
    html += '<a class=""btn"" href=""javascript:;"" onclick=""preview_{0}(this, '+index_{0}+')"" title=""查看""><i class=""icon-eye-open""></i></a>';
    html += '<a class=""btn"" href=""javascript:;"" onclick=""delete_{0}(this)"" title=""删除""><i class=""icon-remove""></i></a>';
    html += '</div></div>';
    var tr = $('.{4}').length == 0 ? $('#{0}').closest('tr') : $('.{4}:last');
    tr.after('<tr class=""{4}""><td>&nbsp;</td><td colspan=""3"">'+html+'</td></tr>');
    if (foucs) $('#{0}_'+index_{0}).focus();
}}
", attributeName, ModalSelectFile.GetOpenWindowString(publishmentSystemInfo.PublishmentSystemId, attributeName, relatedPath), ModalUploadFile.GetOpenWindowStringToTextBox(publishmentSystemInfo.PublishmentSystemId, EUploadType.File, attributeName), ModalFileView.GetOpenWindowStringWithTextBoxValue(publishmentSystemInfo.PublishmentSystemId, attributeName), extendAttributeName, additionalAttributes, style));

            var extendValues = formCollection[extendAttributeName];
            if (!string.IsNullOrEmpty(extendValues))
            {
                foreach (string extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
                {
                    if (!string.IsNullOrEmpty(extendValue))
                    {
                        builder.Append($"add_{attributeName}('{extendValue}',false);");
                    }
                }
            }

            builder.Append("</script>");

            AddHelpText(builder, styleInfo.HelpText);

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

                builder.Append(string.Format(@"
<span id=""c_{0}_1"">{1}<select name=""{0}"" id=""{0}_1"" class=""select"" onchange=""getRelatedField_{2}(2);"">
	<option value="""">请选择</option>", attributeName, prefixes[0], fieldInfo.RelatedFieldID));

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
                        builder.Append(string.Format(@"
{0}<select name=""{1}"" id=""{1}_{2}"" class=""select"" onchange=""getRelatedField_{3}({2} + 1);""></select>{4}</span>
", prefixes[i - 1], attributeName, i, fieldInfo.RelatedFieldID, suffixes[i - 1]));
                    }
                }

                builder.Append(string.Format(@"
<script>
function getRelatedField_{0}(level){{
    var attributeName = '{1}';
    var totalLevel = {2};
    for(i=level;i<=totalLevel;i++){{
        $('#c_' + attributeName + '_' + i).hide();
    }}
    var obj = $('#c_' + attributeName + '_' + (level - 1));
    var itemID = $('option:selected', obj).attr('itemID');
    if (itemID){{
        var url = '{3}' + itemID;
        var values = '{4}';
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
                getRelatedField_{0}(level + 1);
            }}
        }}, 'jsonp');
    }}
}}
", fieldInfo.RelatedFieldID, styleInfo.AttributeName, fieldInfo.TotalLevel, CMS.Controllers.Stl.ActionsRelatedField.GetUrl(PageUtils.GetApiUrl(), publishmentSystemInfo.PublishmentSystemId, styleInfo.Additional.RelatedFieldId, 0), values));

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

        public static void AddValuesToAttributes(ETableStyle tableStyle, string tableName, PublishmentSystemInfo publishmentSystemInfo, List<int> relatedIdentities, NameValueCollection formCollection, NameValueCollection attributes, ArrayList dontAddAttributes, bool isSaveImage)
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
