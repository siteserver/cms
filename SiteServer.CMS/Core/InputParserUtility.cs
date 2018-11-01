using System.Collections;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using SiteServer.CMS.Api;
using SiteServer.CMS.Api.Sys.Stl;
using SiteServer.Utils;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.Plugin;

namespace SiteServer.CMS.Core
{
    public static class InputParserUtility
    {
        public static string GetContentByTableStyle(string content, SiteInfo siteInfo, TableStyleInfo styleInfo)
        {
            if (!string.IsNullOrEmpty(content))
            {
                return GetContentByTableStyle(content, ",", siteInfo, styleInfo, string.Empty, null, string.Empty, false);
            }
            return string.Empty;
        }

        public static string GetContentByTableStyle(string content, string separator, SiteInfo siteInfo, TableStyleInfo styleInfo, string formatString, NameValueCollection attributes, string innerHtml, bool isStlEntity)
        {
            var parsedContent = content;

            var inputType = styleInfo.InputType;

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
                var styleItems = styleInfo.StyleItems;
                if (styleItems != null)
                {
                    foreach (var itemInfo in styleItems)
                    {
                        if (selectedValues.Contains(itemInfo.ItemValue))
                        {
                            selectedTexts.Add(isStlEntity ? itemInfo.ItemValue : itemInfo.ItemTitle);
                        }
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
                parsedContent = ContentUtility.TextEditorContentDecode(siteInfo, parsedContent, true);
            }
            else if (inputType == InputType.Image)
            {
                parsedContent = GetImageOrFlashHtml(siteInfo, parsedContent, attributes, isStlEntity);
            }
            else if (inputType == InputType.Video)
            {
                parsedContent = GetVideoHtml(siteInfo, parsedContent, attributes, isStlEntity);
            }
            else if (inputType == InputType.File)
            {
                parsedContent = GetFileHtmlWithoutCount(siteInfo, parsedContent, attributes, innerHtml, isStlEntity);
            }

            return parsedContent;
        }

        public static string GetContentByTableStyle(ContentInfo contentInfo, string separator, SiteInfo siteInfo, TableStyleInfo styleInfo, string formatString, int no, NameValueCollection attributes, string innerHtml, bool isStlEntity)
        {
            var value = contentInfo.GetString(styleInfo.AttributeName);
            var parsedContent = string.Empty;

            var inputType = styleInfo.InputType;

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
                var styleItems = styleInfo.StyleItems;
                if (styleItems != null)
                {
                    foreach (var itemInfo in styleItems)
                    {
                        if (selectedValues.Contains(itemInfo.ItemValue))
                        {
                            selectedTexts.Add(isStlEntity ? itemInfo.ItemValue : itemInfo.ItemTitle);
                        }
                    }
                }
                
                parsedContent = separator == null ? TranslateUtils.ObjectCollectionToString(selectedTexts) : TranslateUtils.ObjectCollectionToString(selectedTexts, separator);
            }
            else if (inputType == InputType.TextEditor)
            {
                parsedContent = ContentUtility.TextEditorContentDecode(siteInfo, value, true);
            }
            else if (inputType == InputType.Image)
            {
                if (no <= 1)
                {
                    parsedContent = GetImageOrFlashHtml(siteInfo, value, attributes, isStlEntity);
                }
                else
                {
                    var extendAttributeName = ContentAttribute.GetExtendAttributeName(styleInfo.AttributeName);
                    var extendValues = contentInfo.GetString(extendAttributeName);
                    if (!string.IsNullOrEmpty(extendValues))
                    {
                        var index = 2;
                        foreach (string extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
                        {
                            if (index == no)
                            {
                                parsedContent = GetImageOrFlashHtml(siteInfo, extendValue, attributes, isStlEntity);
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
                    parsedContent = GetVideoHtml(siteInfo, value, attributes, isStlEntity);
                }
                else
                {
                    var extendAttributeName = ContentAttribute.GetExtendAttributeName(styleInfo.AttributeName);
                    var extendValues = contentInfo.GetString(extendAttributeName);
                    if (!string.IsNullOrEmpty(extendValues))
                    {
                        var index = 2;
                        foreach (string extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
                        {
                            if (index == no)
                            {
                                parsedContent = GetVideoHtml(siteInfo, extendValue, attributes, isStlEntity);
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
                    parsedContent = GetFileHtmlWithoutCount(siteInfo, value, attributes, innerHtml, isStlEntity);
                }
                else
                {
                    var extendAttributeName = ContentAttribute.GetExtendAttributeName(styleInfo.AttributeName);
                    var extendValues = contentInfo.GetString(extendAttributeName);
                    if (!string.IsNullOrEmpty(extendValues))
                    {
                        var index = 2;
                        foreach (string extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
                        {
                            if (index == no)
                            {
                                parsedContent = GetFileHtmlWithoutCount(siteInfo, extendValue, attributes, innerHtml, isStlEntity);
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

        public static string GetImageOrFlashHtml(SiteInfo siteInfo, string imageUrl, NameValueCollection attributes, bool isStlEntity)
        {
            var retval = string.Empty;
            if (!string.IsNullOrEmpty(imageUrl))
            {
                imageUrl = PageUtility.ParseNavigationUrl(siteInfo, imageUrl, false);
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

        public static string GetVideoHtml(SiteInfo siteInfo, string videoUrl, NameValueCollection attributes, bool isStlEntity)
        {
            var retval = string.Empty;
            if (!string.IsNullOrEmpty(videoUrl))
            {
                videoUrl = PageUtility.ParseNavigationUrl(siteInfo, videoUrl, false);
                if (isStlEntity)
                {
                    retval = videoUrl;
                }
                else
                {
                    retval = $@"
<embed src=""{SiteFilesAssets.GetUrl(ApiManager.ApiUrl, SiteFilesAssets.BrPlayer.Swf)}"" allowfullscreen=""true"" flashvars=""controlbar=over&autostart={true
                        .ToString().ToLower()}&image={string.Empty}&file={videoUrl}"" width=""{450}"" height=""{350}""/>
";
                }
            }
            return retval;
        }

        public static string GetFileHtmlWithCount(SiteInfo siteInfo, int channelId, int contentId, string fileUrl, NameValueCollection attributes, string innerHtml, bool isStlEntity)
        {
            var retval = string.Empty;
            if (!string.IsNullOrEmpty(fileUrl))
            {
                if (isStlEntity)
                {
                    retval = ApiRouteActionsDownload.GetUrl(ApiManager.ApiUrl, siteInfo.Id, channelId, contentId, fileUrl);
                }
                else
                {
                    var stlAnchor = new HtmlAnchor();
                    ControlUtils.AddAttributesIfNotExists(stlAnchor, attributes);
                    stlAnchor.HRef = ApiRouteActionsDownload.GetUrl(ApiManager.ApiUrl, siteInfo.Id, channelId, contentId, fileUrl);
                    stlAnchor.InnerHtml = string.IsNullOrEmpty(innerHtml) ? PageUtils.GetFileNameFromUrl(fileUrl) : innerHtml;

                    retval = ControlUtils.GetControlRenderHtml(stlAnchor);
                }
            }
            return retval;
        }

        public static string GetFileHtmlWithoutCount(SiteInfo siteInfo, string fileUrl, NameValueCollection attributes, string innerHtml, bool isStlEntity)
        {
            if (siteInfo != null)
            {
                var retval = string.Empty;
                if (!string.IsNullOrEmpty(fileUrl))
                {
                    if (isStlEntity)
                    {
                        retval = ApiRouteActionsDownload.GetUrl(ApiManager.ApiUrl, siteInfo.Id, fileUrl);
                    }
                    else
                    {
                        var stlAnchor = new HtmlAnchor();
                        ControlUtils.AddAttributesIfNotExists(stlAnchor, attributes);
                        stlAnchor.HRef = ApiRouteActionsDownload.GetUrl(ApiManager.ApiUrl, siteInfo.Id, fileUrl);
                        stlAnchor.InnerHtml = string.IsNullOrEmpty(innerHtml) ? PageUtils.GetFileNameFromUrl(fileUrl) : innerHtml;

                        retval = ControlUtils.GetControlRenderHtml(stlAnchor);
                    }
                }
                return retval;
            }
            return string.Empty;
        }

        
    }
}
