using System.Collections;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Model;
using SiteServer.Plugin;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Core
{
    public class InputParserUtility
    {
        private InputParserUtility()
        {
        }

        public static string GetContentByTableStyle(string content, PublishmentSystemInfo publishmentSystemInfo, ETableStyle tableStyle, TableStyleInfo styleInfo)
        {
            if (!string.IsNullOrEmpty(content))
            {
                return GetContentByTableStyle(content, ",", publishmentSystemInfo, tableStyle, styleInfo, string.Empty, null, string.Empty, false);
            }
            return string.Empty;
        }

        public static string GetContentByTableStyle(string content, string separator, PublishmentSystemInfo publishmentSystemInfo, ETableStyle tableStyle, TableStyleInfo styleInfo, string formatString, Dictionary<string, string> attributes, string innerXml, bool isStlEntity)
        {
            var parsedContent = content;

            var inputType = InputTypeUtils.GetEnumType(styleInfo.InputType);

            if (inputType == InputType.Date)
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
            else if (inputType == InputType.DateTime)
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
            else if (inputType == InputType.CheckBox || inputType == InputType.Radio || inputType == InputType.SelectMultiple || inputType == InputType.SelectOne)//选择类型
            {
                var selectedTexts = new ArrayList();
                var selectedValues = TranslateUtils.StringCollectionToStringList(content);
                var styleItems = styleInfo.StyleItems ??
                                 BaiRongDataProvider.TableStyleDao.GetStyleItemInfoList(styleInfo.TableStyleId);
                foreach (var itemInfo in styleItems)
                {
                    if (selectedValues.Contains(itemInfo.ItemValue))
                    {
                        selectedTexts.Add(isStlEntity ? itemInfo.ItemValue : itemInfo.ItemTitle);
                    }
                }
                parsedContent = separator == null ? TranslateUtils.ObjectCollectionToString(selectedTexts) : TranslateUtils.ObjectCollectionToString(selectedTexts, separator);
            }
            //else if (styleInfo.InputType == InputType.TextArea)
            //{
            //    parsedContent = StringUtils.ReplaceNewlineToBR(parsedContent);
            //}
            else if (inputType == InputType.TextEditor)
            {
                /****获取编辑器中内容，解析@符号，添加了远程路径处理 20151103****/
                parsedContent = StringUtility.TextEditorContentDecode(parsedContent, publishmentSystemInfo, true);
            }
            else if (inputType == InputType.Image)
            {
                parsedContent = GetImageOrFlashHtml(publishmentSystemInfo, parsedContent, attributes, isStlEntity);
            }
            else if (inputType == InputType.Video)
            {
                parsedContent = GetVideoHtml(publishmentSystemInfo, parsedContent, attributes, isStlEntity);
            }
            else if (inputType == InputType.File)
            {
                parsedContent = GetFileHtmlWithoutCount(publishmentSystemInfo, parsedContent, attributes, innerXml, isStlEntity);
            }

            return parsedContent;
        }

        public static string GetContentByTableStyle(ContentInfo contentInfo, string separator, PublishmentSystemInfo publishmentSystemInfo, ETableStyle tableStyle, TableStyleInfo styleInfo, string formatString, int no, Dictionary<string, string> attributes, string innerXml, bool isStlEntity)
        {
            var value = contentInfo.GetExtendedAttribute(styleInfo.AttributeName);
            var parsedContent = string.Empty;

            var inputType = InputTypeUtils.GetEnumType(styleInfo.InputType);

            if (inputType == InputType.Date)
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
            else if (inputType == InputType.DateTime)
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
            else if (inputType == InputType.CheckBox || inputType == InputType.Radio || inputType == InputType.SelectMultiple || inputType == InputType.SelectOne)//选择类型
            {
                var selectedTexts = new ArrayList();
                var selectedValues = TranslateUtils.StringCollectionToStringList(value);
                var styleItems = styleInfo.StyleItems ??
                                 BaiRongDataProvider.TableStyleDao.GetStyleItemInfoList(styleInfo.TableStyleId);
                foreach (var itemInfo in styleItems)
                {
                    if (selectedValues.Contains(itemInfo.ItemValue))
                    {
                        selectedTexts.Add(isStlEntity ? itemInfo.ItemValue : itemInfo.ItemTitle);
                    }
                }
                parsedContent = separator == null ? TranslateUtils.ObjectCollectionToString(selectedTexts) : TranslateUtils.ObjectCollectionToString(selectedTexts, separator);
            }
            else if (inputType == InputType.TextEditor)
            {
                /****获取编辑器中内容，解析@符号，添加了远程路径处理 20151103****/
                parsedContent = StringUtility.TextEditorContentDecode(value, publishmentSystemInfo, true);
            }
            else if (inputType == InputType.Image)
            {
                if (no <= 1)
                {
                    parsedContent = GetImageOrFlashHtml(publishmentSystemInfo, value, attributes, isStlEntity);
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
                                parsedContent = GetImageOrFlashHtml(publishmentSystemInfo, extendValue, attributes, isStlEntity);
                                break;
                            }
                            index++;
                        }
                    }
                }
            }
            else if (inputType == InputType.Video)
            {
                if (no <= 1)
                {
                    parsedContent = GetVideoHtml(publishmentSystemInfo, value, attributes, isStlEntity);
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
                                parsedContent = GetVideoHtml(publishmentSystemInfo, extendValue, attributes, isStlEntity);
                                break;
                            }
                            index++;
                        }
                    }
                }
            }
            else if (inputType == InputType.File)
            {
                if (no <= 1)
                {
                    parsedContent = GetFileHtmlWithoutCount(publishmentSystemInfo, value, attributes, innerXml, isStlEntity);
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
                                parsedContent = GetFileHtmlWithoutCount(publishmentSystemInfo, extendValue, attributes, innerXml, isStlEntity);
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

        public static string GetImageOrFlashHtml(PublishmentSystemInfo publishmentSystemInfo, string imageUrl, Dictionary<string, string> attributes, bool isStlEntity)
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
                                catch
                                {
                                    // ignored
                                }
                            }
                            if (!string.IsNullOrEmpty(attributes["height"]))
                            {
                                try
                                {
                                    height = int.Parse(attributes["height"]);
                                }
                                catch
                                {
                                    // ignored
                                }
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

        public static string GetVideoHtml(PublishmentSystemInfo publishmentSystemInfo, string videoUrl, Dictionary<string, string> attributes, bool isStlEntity)
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

        public static string GetFileHtmlWithCount(PublishmentSystemInfo publishmentSystemInfo, int nodeId, int contentId, string fileUrl, Dictionary<string, string> attributes, string innerXml, bool isStlEntity)
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
                    stlAnchor.InnerHtml = string.IsNullOrEmpty(innerXml) ? PageUtils.GetFileNameFromUrl(fileUrl) : innerXml;

                    retval = ControlUtils.GetControlRenderHtml(stlAnchor);
                }
            }
            return retval;
        }

        public static string GetFileHtmlWithoutCount(PublishmentSystemInfo publishmentSystemInfo, string fileUrl, Dictionary<string, string> attributes, string innerXml, bool isStlEntity)
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
                        stlAnchor.InnerHtml = string.IsNullOrEmpty(innerXml) ? PageUtils.GetFileNameFromUrl(fileUrl) : innerXml;

                        retval = ControlUtils.GetControlRenderHtml(stlAnchor);
                    }
                }
                return retval;
            }
            return string.Empty;
        }

        
    }
}
