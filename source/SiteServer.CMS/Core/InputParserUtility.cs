using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.HtmlControls;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Core
{
    public class InputParserUtility
    {
        private InputParserUtility()
        {
        }

        public static string GetValidateHtmlString(TableStyleInfo styleInfo, out string validateAttributes)
        {
            var builder = new StringBuilder();

            validateAttributes = string.Empty;

            if (styleInfo.Additional.IsValidate && !EInputTypeUtils.Equals(styleInfo.InputType, EInputType.TextEditor))
            {
                validateAttributes = InputParserUtils.GetValidateAttributes(styleInfo.Additional.IsValidate, styleInfo.DisplayName, styleInfo.Additional.IsRequired, styleInfo.Additional.MinNum, styleInfo.Additional.MaxNum, styleInfo.Additional.ValidateType, styleInfo.Additional.RegExp, styleInfo.Additional.ErrorMessage);

                builder.Append(
                    $@"&nbsp;<span id=""{styleInfo.AttributeName}_msg"" style=""color:red;display:none;"">*</span>");
                builder.Append($@"
<script>event_observe('{styleInfo.AttributeName}', 'blur', checkAttributeValue);</script>
");
            }
            return builder.ToString();
        }

        public static string GetContentByTableStyle(string content, PublishmentSystemInfo publishmentSystemInfo, ETableStyle tableStyle, TableStyleInfo styleInfo)
        {
            if (!string.IsNullOrEmpty(content))
            {
                return GetContentByTableStyle(content, ",", publishmentSystemInfo, tableStyle, styleInfo, string.Empty, null, string.Empty, false);
            }
            return string.Empty;
        }

        public static string GetContentByTableStyle(string content, string separator, PublishmentSystemInfo publishmentSystemInfo, ETableStyle tableStyle, TableStyleInfo styleInfo, string formatString, StringDictionary attributes, string innerXml, bool isStlEntity)
        {
            var parsedContent = content;

            var inputType = EInputTypeUtils.GetEnumType(styleInfo.InputType);

            if (inputType == EInputType.Date)
            {
                var dateTime = TranslateUtils.ToDateTime(content);
                if (dateTime != DateUtils.SqlMinValue)
                {
                    if (string.IsNullOrEmpty(formatString))
                    {
                        formatString = DateUtils.FormatStringDateOnly;
                    }
                    parsedContent = DateUtils.Format(dateTime, formatString);
                }
                else
                {
                    parsedContent = string.Empty;
                }
            }
            else if (inputType == EInputType.DateTime)
            {
                var dateTime = TranslateUtils.ToDateTime(content);
                if (dateTime != DateUtils.SqlMinValue)
                {
                    if (string.IsNullOrEmpty(formatString))
                    {
                        formatString = DateUtils.FormatStringDateTime;
                    }
                    parsedContent = DateUtils.Format(dateTime, formatString);
                }
                else
                {
                    parsedContent = string.Empty;
                }
            }
            else if (inputType == EInputType.CheckBox || inputType == EInputType.Radio || inputType == EInputType.SelectMultiple || inputType == EInputType.SelectOne)//选择类型
            {
                var selectedTexts = new ArrayList();
                var selectedValues = TranslateUtils.StringCollectionToStringList(content);
                var styleItems = styleInfo.StyleItems;
                if (styleItems == null)
                {
                    styleItems = BaiRongDataProvider.TableStyleDao.GetStyleItemInfoList(styleInfo.TableStyleId);
                }
                foreach (var itemInfo in styleItems)
                {
                    if (selectedValues.Contains(itemInfo.ItemValue))
                    {
                        if (isStlEntity)
                        {
                            selectedTexts.Add(itemInfo.ItemValue);
                        }
                        else
                        {
                            selectedTexts.Add(itemInfo.ItemTitle);
                        }
                    }
                }
                if (separator == null)
                {
                    parsedContent = TranslateUtils.ObjectCollectionToString(selectedTexts);
                }
                else
                {
                    parsedContent = TranslateUtils.ObjectCollectionToString(selectedTexts, separator);
                }
            }
            //else if (styleInfo.InputType == EInputType.TextArea)
            //{
            //    parsedContent = StringUtils.ReplaceNewlineToBR(parsedContent);
            //}
            else if (inputType == EInputType.TextEditor)
            {
                /****获取编辑器中内容，解析@符号，添加了远程路径处理 20151103****/
                parsedContent = StringUtility.TextEditorContentDecode(parsedContent, publishmentSystemInfo, true);
            }
            else if (inputType == EInputType.Image)
            {
                parsedContent = InputParserUtility.GetImageOrFlashHtml(publishmentSystemInfo, parsedContent, attributes, isStlEntity);
            }
            else if (inputType == EInputType.Video)
            {
                parsedContent = InputParserUtility.GetVideoHtml(publishmentSystemInfo, parsedContent, attributes, isStlEntity);
            }
            else if (inputType == EInputType.File)
            {
                parsedContent = InputParserUtility.GetFileHtmlWithoutCount(publishmentSystemInfo, parsedContent, attributes, innerXml, isStlEntity);
            }

            return parsedContent;
        }

        public static string GetContentByTableStyle(ContentInfo contentInfo, string separator, PublishmentSystemInfo publishmentSystemInfo, ETableStyle tableStyle, TableStyleInfo styleInfo, string formatString, int no, StringDictionary attributes, string innerXml, bool isStlEntity)
        {
            var value = contentInfo.GetExtendedAttribute(styleInfo.AttributeName);
            var parsedContent = string.Empty;

            var inputType = EInputTypeUtils.GetEnumType(styleInfo.InputType);

            if (inputType == EInputType.Date)
            {
                var dateTime = TranslateUtils.ToDateTime(value);
                if (dateTime != DateUtils.SqlMinValue)
                {
                    if (string.IsNullOrEmpty(formatString))
                    {
                        formatString = DateUtils.FormatStringDateOnly;
                    }
                    parsedContent = DateUtils.Format(dateTime, formatString);
                }
            }
            else if (inputType == EInputType.DateTime)
            {
                var dateTime = TranslateUtils.ToDateTime(value);
                if (dateTime != DateUtils.SqlMinValue)
                {
                    if (string.IsNullOrEmpty(formatString))
                    {
                        formatString = DateUtils.FormatStringDateTime;
                    }
                    parsedContent = DateUtils.Format(dateTime, formatString);
                }
            }
            else if (inputType == EInputType.CheckBox || inputType == EInputType.Radio || inputType == EInputType.SelectMultiple || inputType == EInputType.SelectOne)//选择类型
            {
                var selectedTexts = new ArrayList();
                var selectedValues = TranslateUtils.StringCollectionToStringList(value);
                var styleItems = styleInfo.StyleItems;
                if (styleItems == null)
                {
                    styleItems = BaiRongDataProvider.TableStyleDao.GetStyleItemInfoList(styleInfo.TableStyleId);
                }
                foreach (var itemInfo in styleItems)
                {
                    if (selectedValues.Contains(itemInfo.ItemValue))
                    {
                        if (isStlEntity)
                        {
                            selectedTexts.Add(itemInfo.ItemValue);
                        }
                        else
                        {
                            selectedTexts.Add(itemInfo.ItemTitle);
                        }
                    }
                }
                if (separator == null)
                {
                    parsedContent = TranslateUtils.ObjectCollectionToString(selectedTexts);
                }
                else
                {
                    parsedContent = TranslateUtils.ObjectCollectionToString(selectedTexts, separator);
                }
            }
            else if (inputType == EInputType.TextEditor)
            {
                /****获取编辑器中内容，解析@符号，添加了远程路径处理 20151103****/
                parsedContent = StringUtility.TextEditorContentDecode(value, publishmentSystemInfo, true);
            }
            else if (inputType == EInputType.Image)
            {
                if (no <= 1)
                {
                    parsedContent = InputParserUtility.GetImageOrFlashHtml(publishmentSystemInfo, value, attributes, isStlEntity);
                }
                else
                {
                    var extendAttributeName = ContentAttribute.GetExtendAttributeName(styleInfo.AttributeName);
                    var extendValues = contentInfo.GetExtendedAttribute(extendAttributeName);
                    if (!string.IsNullOrEmpty(extendValues))
                    {
                        var index = 2;
                        foreach (string extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
                        {
                            if (index == no)
                            {
                                parsedContent = InputParserUtility.GetImageOrFlashHtml(publishmentSystemInfo, extendValue, attributes, isStlEntity);
                                break;
                            }
                            index++;
                        }
                    }
                }
            }
            else if (inputType == EInputType.Video)
            {
                if (no <= 1)
                {
                    parsedContent = InputParserUtility.GetVideoHtml(publishmentSystemInfo, value, attributes, isStlEntity);
                }
                else
                {
                    var extendAttributeName = ContentAttribute.GetExtendAttributeName(styleInfo.AttributeName);
                    var extendValues = contentInfo.GetExtendedAttribute(extendAttributeName);
                    if (!string.IsNullOrEmpty(extendValues))
                    {
                        var index = 2;
                        foreach (string extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
                        {
                            if (index == no)
                            {
                                parsedContent = InputParserUtility.GetVideoHtml(publishmentSystemInfo, extendValue, attributes, isStlEntity);
                                break;
                            }
                            index++;
                        }
                    }
                }
            }
            else if (inputType == EInputType.File)
            {
                if (no <= 1)
                {
                    parsedContent = InputParserUtility.GetFileHtmlWithoutCount(publishmentSystemInfo, value, attributes, innerXml, isStlEntity);
                }
                else
                {
                    var extendAttributeName = ContentAttribute.GetExtendAttributeName(styleInfo.AttributeName);
                    var extendValues = contentInfo.GetExtendedAttribute(extendAttributeName);
                    if (!string.IsNullOrEmpty(extendValues))
                    {
                        var index = 2;
                        foreach (string extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
                        {
                            if (index == no)
                            {
                                parsedContent = InputParserUtility.GetFileHtmlWithoutCount(publishmentSystemInfo, extendValue, attributes, innerXml, isStlEntity);
                                break;
                            }
                            index++;
                        }
                    }
                }
            }
            else
            {
                parsedContent = value;
            }

            return parsedContent;
        }

        

        

        

        

        public static string GetImageOrFlashHtml(PublishmentSystemInfo publishmentSystemInfo, string imageUrl, StringDictionary attributes, bool isStlEntity)
        {
            var retval = string.Empty;
            if (!string.IsNullOrEmpty(imageUrl))
            {
                imageUrl = PageUtility.ParseNavigationUrl(publishmentSystemInfo, imageUrl);
                if (isStlEntity)
                {
                    retval = imageUrl;
                }
                else
                {
                    if (!imageUrl.ToUpper().Trim().EndsWith(".SWF"))
                    {
                        var htmlImage = new HtmlImage();
                        ControlUtils.AddAttributesIfNotExists(htmlImage, attributes);
                        htmlImage.Src = imageUrl;
                        retval = ControlUtils.GetControlRenderHtml(htmlImage);
                    }
                    else
                    {
                        var width = 100;
                        var height = 100;
                        if (attributes != null)
                        {
                            if (!string.IsNullOrEmpty(attributes["width"]))
                            {
                                try
                                {
                                    width = int.Parse(attributes["width"]);
                                }
                                catch { }
                            }
                            if (!string.IsNullOrEmpty(attributes["height"]))
                            {
                                try
                                {
                                    height = int.Parse(attributes["height"]);
                                }
                                catch { }
                            }
                        }
                        retval = $@"
<object classid=""clsid:D27CDB6E-AE6D-11cf-96B8-444553540000"" codebase=""http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,29,0"" width=""{width}"" height=""{height}"">
                <param name=""movie"" value=""{imageUrl}"">
                <param name=""quality"" value=""high"">
                <param name=""wmode"" value=""transparent"">
                <embed src=""{imageUrl}"" width=""{width}"" height=""{height}"" quality=""high"" pluginspage=""http://www.macromedia.com/go/getflashplayer"" type=""application/x-shockwave-flash"" wmode=""transparent""></embed></object>
";
                    }
                }
            }
            return retval;
        }

        public static string GetVideoHtml(PublishmentSystemInfo publishmentSystemInfo, string videoUrl, StringDictionary attributes, bool isStlEntity)
        {
            var retval = string.Empty;
            if (!string.IsNullOrEmpty(videoUrl))
            {
                videoUrl = PageUtility.ParseNavigationUrl(publishmentSystemInfo, videoUrl);
                if (isStlEntity)
                {
                    retval = videoUrl;
                }
                else
                {
                    retval = $@"
<embed src=""{SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, SiteFilesAssets.BrPlayer.Swf)}"" allowfullscreen=""true"" flashvars=""controlbar=over&autostart={true
                        .ToString().ToLower()}&image={string.Empty}&file={videoUrl}"" width=""{450}"" height=""{350}""/>
";
                }
            }
            return retval;
        }

        public static string GetFileHtmlWithCount(PublishmentSystemInfo publishmentSystemInfo, int nodeId, int contentId, string fileUrl, StringDictionary attributes, string innerXml, bool isStlEntity)
        {
            var retval = string.Empty;
            if (!string.IsNullOrEmpty(fileUrl))
            {
                if (isStlEntity)
                {
                    retval = ActionsDownload.GetUrl(publishmentSystemInfo.Additional.ApiUrl, publishmentSystemInfo.PublishmentSystemId, nodeId, contentId, fileUrl);
                }
                else
                {
                    var stlAnchor = new HtmlAnchor();
                    ControlUtils.AddAttributesIfNotExists(stlAnchor, attributes);
                    stlAnchor.HRef = ActionsDownload.GetUrl(publishmentSystemInfo.Additional.ApiUrl, publishmentSystemInfo.PublishmentSystemId, nodeId, contentId, fileUrl);
                    if (string.IsNullOrEmpty(innerXml))
                    {
                        stlAnchor.InnerHtml = PageUtils.GetFileNameFromUrl(fileUrl);
                    }
                    else
                    {
                        stlAnchor.InnerHtml = innerXml;
                    }

                    retval = ControlUtils.GetControlRenderHtml(stlAnchor);
                }
            }
            return retval;
        }

        public static string GetFileHtmlWithoutCount(PublishmentSystemInfo publishmentSystemInfo, string fileUrl, StringDictionary attributes, string innerXml, bool isStlEntity)
        {
            if (publishmentSystemInfo != null)
            {
                var retval = string.Empty;
                if (!string.IsNullOrEmpty(fileUrl))
                {
                    if (isStlEntity)
                    {
                        retval = ActionsDownload.GetUrl(publishmentSystemInfo.Additional.ApiUrl, publishmentSystemInfo.PublishmentSystemId, fileUrl);
                    }
                    else
                    {
                        var stlAnchor = new HtmlAnchor();
                        ControlUtils.AddAttributesIfNotExists(stlAnchor, attributes);
                        stlAnchor.HRef = ActionsDownload.GetUrl(publishmentSystemInfo.Additional.ApiUrl, publishmentSystemInfo.PublishmentSystemId, fileUrl);
                        if (string.IsNullOrEmpty(innerXml))
                        {
                            stlAnchor.InnerHtml = PageUtils.GetFileNameFromUrl(fileUrl);
                        }
                        else
                        {
                            stlAnchor.InnerHtml = innerXml;
                        }

                        retval = ControlUtils.GetControlRenderHtml(stlAnchor);
                    }
                }
                return retval;
            }
            return string.Empty;
        }

        
    }
}
