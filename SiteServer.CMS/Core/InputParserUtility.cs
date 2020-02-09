using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Web.UI.HtmlControls;
using Datory.Utils;
using SiteServer.CMS.Api.Sys.Stl;
using SiteServer.CMS.Context;
using SiteServer.Abstractions;
using SiteServer.CMS.Repositories;


namespace SiteServer.CMS.Core
{
    public static class InputParserUtility
    {
        public static async Task<string> GetContentByTableStyleAsync(string content, Site site, TableStyle style)
        {
            if (!string.IsNullOrEmpty(content))
            {
                return await GetContentByTableStyleAsync(content, ",", site, style, string.Empty, null, string.Empty, false);
            }
            return string.Empty;
        }

        public static async Task<string> GetContentByTableStyleAsync(string content, string separator, Site site, TableStyle style, string formatString, NameValueCollection attributes, string innerHtml, bool isStlEntity)
        {
            var parsedContent = content;

            var inputType = style.InputType;

            if (inputType == InputType.Date)
            {
                var dateTime = TranslateUtils.ToDateTime(content);
                if (dateTime != Constants.SqlMinValue)
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
                if (dateTime != Constants.SqlMinValue)
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
                var selectedTexts = new List<string>();
                var selectedValues = Utilities.GetStringList(content);
                var styleItems = style.Items;
                if (styleItems != null)
                {
                    foreach (var itemInfo in styleItems)
                    {
                        if (selectedValues.Contains(itemInfo.Value))
                        {
                            selectedTexts.Add(isStlEntity ? itemInfo.Value : itemInfo.Label);
                        }
                    }
                }
                
                parsedContent = separator == null ? Utilities.ToString(selectedTexts) : Utilities.ToString(selectedTexts, separator);
            }
            //else if (style.InputType == InputType.TextArea)
            //{
            //    parsedContent = StringUtils.ReplaceNewlineToBR(parsedContent);
            //}
            else if (inputType == InputType.TextEditor)
            {
                parsedContent = ContentUtility.TextEditorContentDecode(site, parsedContent, true);
            }
            else if (inputType == InputType.Image)
            {
                parsedContent = GetImageOrFlashHtml(site, parsedContent, attributes, isStlEntity);
            }
            else if (inputType == InputType.Video)
            {
                parsedContent = await GetVideoHtmlAsync(site, parsedContent, attributes, isStlEntity);
            }
            else if (inputType == InputType.File)
            {
                parsedContent = await GetFileHtmlWithoutCountAsync(site, parsedContent, attributes, innerHtml, isStlEntity, false, false);
            }

            return parsedContent;
        }

        public static async Task<string> GetContentByTableStyleAsync(Content content, string separator, Site site, TableStyle style, string formatString, int no, NameValueCollection attributes, string innerHtml, bool isStlEntity)
        {
            var value = content.Get<string>(style.AttributeName);
            var parsedContent = string.Empty;

            var inputType = style.InputType;

            if (inputType == InputType.Date)
            {
                var dateTime = TranslateUtils.ToDateTime(value);
                if (dateTime != Constants.SqlMinValue)
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
                if (dateTime != Constants.SqlMinValue)
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
                var selectedTexts = new List<string>();
                var selectedValues = Utilities.GetStringList(value);
                var styleItems = style.Items;
                if (styleItems != null)
                {
                    foreach (var itemInfo in styleItems)
                    {
                        if (selectedValues.Contains(itemInfo.Value))
                        {
                            selectedTexts.Add(isStlEntity ? itemInfo.Value : itemInfo.Label);
                        }
                    }
                }
                
                parsedContent = separator == null ? Utilities.ToString(selectedTexts) : Utilities.ToString(selectedTexts, separator);
            }
            else if (inputType == InputType.TextEditor)
            {
                parsedContent = ContentUtility.TextEditorContentDecode(site, value, true);
            }
            else if (inputType == InputType.Image)
            {
                if (no <= 1)
                {
                    parsedContent = GetImageOrFlashHtml(site, value, attributes, isStlEntity);
                }
                else
                {
                    var extendAttributeName = ContentAttribute.GetExtendAttributeName(style.AttributeName);
                    var extendValues = content.Get<string>(extendAttributeName);
                    if (!string.IsNullOrEmpty(extendValues))
                    {
                        var index = 2;
                        foreach (string extendValue in Utilities.GetStringList(extendValues))
                        {
                            if (index == no)
                            {
                                parsedContent = GetImageOrFlashHtml(site, extendValue, attributes, isStlEntity);
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
                    parsedContent = await GetVideoHtmlAsync(site, value, attributes, isStlEntity);
                }
                else
                {
                    var extendAttributeName = ContentAttribute.GetExtendAttributeName(style.AttributeName);
                    var extendValues = content.Get<string>(extendAttributeName);
                    if (!string.IsNullOrEmpty(extendValues))
                    {
                        var index = 2;
                        foreach (string extendValue in Utilities.GetStringList(extendValues))
                        {
                            if (index == no)
                            {
                                parsedContent = await GetVideoHtmlAsync(site, extendValue, attributes, isStlEntity);
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
                    parsedContent = await GetFileHtmlWithoutCountAsync(site, value, attributes, innerHtml, isStlEntity, false, false);
                }
                else
                {
                    var extendAttributeName = ContentAttribute.GetExtendAttributeName(style.AttributeName);
                    var extendValues = content.Get<string>(extendAttributeName);
                    if (!string.IsNullOrEmpty(extendValues))
                    {
                        var index = 2;
                        foreach (string extendValue in Utilities.GetStringList(extendValues))
                        {
                            if (index == no)
                            {
                                parsedContent = await GetFileHtmlWithoutCountAsync(site, extendValue, attributes, innerHtml, isStlEntity, false, false);
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

        public static string GetImageOrFlashHtml(Site site, string imageUrl, NameValueCollection attributes, bool isStlEntity)
        {
            var retVal = string.Empty;
            if (!string.IsNullOrEmpty(imageUrl))
            {
                imageUrl = PageUtility.ParseNavigationUrl(site, imageUrl, false);
                if (isStlEntity)
                {
                    retVal = imageUrl;
                }
                else
                {
                    if (!imageUrl.ToUpper().Trim().EndsWith(".SWF"))
                    {
                        var htmlImage = new HtmlImage();
                        ControlUtils.AddAttributesIfNotExists(htmlImage, attributes);
                        htmlImage.Src = imageUrl;
                        retVal = ControlUtils.GetControlRenderHtml(htmlImage);
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
                        retVal = $@"
<object classid=""clsid:D27CDB6E-AE6D-11cf-96B8-444553540000"" codebase=""http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,29,0"" width=""{width}"" height=""{height}"">
                <param name=""movie"" value=""{imageUrl}"">
                <param name=""quality"" value=""high"">
                <param name=""wmode"" value=""transparent"">
                <embed src=""{imageUrl}"" width=""{width}"" height=""{height}"" quality=""high"" pluginspage=""http://www.macromedia.com/go/getflashplayer"" type=""application/x-shockwave-flash"" wmode=""transparent""></embed></object>
";
                    }
                }
            }
            return retVal;
        }

        public static async Task<string> GetVideoHtmlAsync(Site site, string videoUrl, NameValueCollection attributes, bool isStlEntity)
        {
            var retVal = string.Empty;
            if (!string.IsNullOrEmpty(videoUrl))
            {
                videoUrl = PageUtility.ParseNavigationUrl(site, videoUrl, false);
                if (isStlEntity)
                {
                    retVal = videoUrl;
                }
                else
                {
                    var config = await DataProvider.ConfigRepository.GetAsync();

                    retVal = $@"
<embed src=""{SiteFilesAssets.GetUrl(config.GetApiUrl(), SiteFilesAssets.BrPlayer.Swf)}"" allowfullscreen=""true"" flashvars=""controlbar=over&autostart={true
                        .ToString().ToLower()}&image={string.Empty}&file={videoUrl}"" width=""{450}"" height=""{350}""/>
";
                }
            }
            return retVal;
        }

        public static async Task<string> GetFileHtmlWithCountAsync(Site site, int channelId, int contentId, string fileUrl, NameValueCollection attributes, string innerHtml, bool isStlEntity, bool isLower, bool isUpper)
        {
            if (site == null || string.IsNullOrEmpty(fileUrl)) return string.Empty;

            var config = await DataProvider.ConfigRepository.GetAsync();

            string retVal;
            if (isStlEntity)
            {
                retVal = ApiRouteActionsDownload.GetUrl(config.GetApiUrl(), site.Id, channelId, contentId,
                    fileUrl);
            }
            else
            {
                var stlAnchor = new HtmlAnchor();
                ControlUtils.AddAttributesIfNotExists(stlAnchor, attributes);
                stlAnchor.HRef = ApiRouteActionsDownload.GetUrl(config.GetApiUrl(), site.Id, channelId,
                    contentId, fileUrl);
                stlAnchor.InnerHtml = string.IsNullOrEmpty(innerHtml)
                    ? PageUtils.GetFileNameFromUrl(fileUrl)
                    : innerHtml;
                if (isLower)
                {
                    stlAnchor.InnerHtml = stlAnchor.InnerHtml.ToLower();
                }
                if (isUpper)
                {
                    stlAnchor.InnerHtml = stlAnchor.InnerHtml.ToUpper();
                }

                retVal = ControlUtils.GetControlRenderHtml(stlAnchor);
            }

            return retVal;
        }

        public static async Task<string> GetFileHtmlWithoutCountAsync(Site site, string fileUrl, NameValueCollection attributes, string innerHtml, bool isStlEntity, bool isLower, bool isUpper)
        {
            if (site == null || string.IsNullOrEmpty(fileUrl)) return string.Empty;

            var config = await DataProvider.ConfigRepository.GetAsync();

            string retVal;
            if (isStlEntity)
            {
                retVal = ApiRouteActionsDownload.GetUrl(config.GetApiUrl(), site.Id, fileUrl);
            }
            else
            {
                var stlAnchor = new HtmlAnchor();
                ControlUtils.AddAttributesIfNotExists(stlAnchor, attributes);
                stlAnchor.HRef = ApiRouteActionsDownload.GetUrl(config.GetApiUrl(), site.Id, fileUrl);
                stlAnchor.InnerHtml = string.IsNullOrEmpty(innerHtml) ? PageUtils.GetFileNameFromUrl(fileUrl) : innerHtml;

                if (isLower)
                {
                    stlAnchor.InnerHtml = stlAnchor.InnerHtml.ToLower();
                }
                if (isUpper)
                {
                    stlAnchor.InnerHtml = stlAnchor.InnerHtml.ToUpper();
                }

                retVal = ControlUtils.GetControlRenderHtml(stlAnchor);
            }

            return retVal;
        }
    }
}
