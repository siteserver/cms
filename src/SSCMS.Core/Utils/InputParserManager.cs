using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using SSCMS.Configuration;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Parse;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Utils
{
    public class InputParserManager
    {
        private readonly IPathManager _pathManager;

        public InputParserManager(IPathManager pathManager)
        {
            _pathManager = pathManager;
        }

        public async Task<string> GetContentByTableStyleAsync(string content, string separator, Site site, TableStyle style, string formatString, NameValueCollection attributes, string innerHtml, bool isStlEntity)
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
                var selectedValues = ListUtils.GetStringList(content);
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
                
                parsedContent = separator == null ? ListUtils.ToString(selectedTexts) : ListUtils.ToString(selectedTexts, separator);
            }
            //else if (style.InputType == InputType.TextArea)
            //{
            //    parsedContent = StringUtils.ReplaceNewlineToBR(parsedContent);
            //}
            else if (inputType == InputType.TextEditor)
            {
                parsedContent = await _pathManager.DecodeTextEditorAsync(site, parsedContent, true);
            }
            else if (inputType == InputType.Image)
            {
                parsedContent = await GetImageOrFlashHtmlAsync(site, parsedContent, attributes, isStlEntity);
            }
            else if (inputType == InputType.Video)
            {
                parsedContent = await GetVideoHtmlAsync(site, parsedContent, attributes, isStlEntity);
            }
            else if (inputType == InputType.File)
            {
                parsedContent = GetFileHtmlWithoutCount(site, parsedContent, attributes, innerHtml, isStlEntity, false, false);
            }

            return parsedContent;
        }

        public async Task<string> GetContentByTableStyleAsync(Content content, string separator, Site site, TableStyle style, string formatString, int no, NameValueCollection attributes, string innerHtml, bool isStlEntity)
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
                var selectedValues = ListUtils.GetStringList(value);
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
                
                parsedContent = separator == null ? ListUtils.ToString(selectedTexts) : ListUtils.ToString(selectedTexts, separator);
            }
            else if (inputType == InputType.TextEditor)
            {
                parsedContent = await _pathManager.DecodeTextEditorAsync(site, value, true);
            }
            else if (inputType == InputType.Image)
            {
                if (no <= 1)
                {
                    parsedContent = await GetImageOrFlashHtmlAsync(site, value, attributes, isStlEntity);
                }
                else
                {
                    var extendName = ColumnsManager.GetExtendName(style.AttributeName, no - 1);
                    var extend = content.Get<string>(extendName);
                    parsedContent = await GetImageOrFlashHtmlAsync(site, extend, attributes, isStlEntity);
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
                    var extendName = ColumnsManager.GetExtendName(style.AttributeName, no - 1);
                    var extend = content.Get<string>(extendName);
                    parsedContent = await GetVideoHtmlAsync(site, extend, attributes, isStlEntity);
                }
            }
            else if (inputType == InputType.File)
            {
                if (no <= 1)
                {
                    parsedContent = GetFileHtmlWithoutCount(site, value, attributes, innerHtml, isStlEntity, false, false);
                }
                else
                {
                    var extendName = ColumnsManager.GetExtendName(style.AttributeName, no - 1);
                    var extend = content.Get<string>(extendName);
                    parsedContent = GetFileHtmlWithoutCount(site, extend, attributes, innerHtml, isStlEntity, false, false);
                }
            }
            else
            {
                parsedContent = value;
            }

            return parsedContent;
        }

        public async Task<string> GetImageOrFlashHtmlAsync(Site site, string imageUrl, NameValueCollection attributes, bool isStlEntity)
        {
            var retVal = string.Empty;
            if (!string.IsNullOrEmpty(imageUrl))
            {
                imageUrl = await _pathManager.ParseSiteUrlAsync(site, imageUrl, false);
                if (isStlEntity)
                {
                    retVal = imageUrl;
                }
                else
                {
                    if (!imageUrl.ToUpper().Trim().EndsWith(".SWF"))
                    {
                        var imageAttributes = new NameValueCollection();
                        TranslateUtils.AddAttributesIfNotExists(imageAttributes, attributes);
                        imageAttributes["src"] = imageUrl;

                        retVal = $@"<img {TranslateUtils.ToAttributesString(imageAttributes)}>";
                    }
                    else
                    {
                        var width = 100;
                        var height = 100;
                        if (attributes != null)
                        {
                            if (!string.IsNullOrEmpty(attributes["width"]))
                            {
                                width = TranslateUtils.ToInt(attributes["width"]);
                            }
                            if (!string.IsNullOrEmpty(attributes["height"]))
                            {
                                height = TranslateUtils.ToInt(attributes["height"]);
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

        public async Task<string> GetVideoHtmlAsync(Site site, string videoUrl, NameValueCollection attributes, bool isStlEntity)
        {
            var retVal = string.Empty;
            if (!string.IsNullOrEmpty(videoUrl))
            {
                videoUrl = await _pathManager.ParseSiteUrlAsync(site, videoUrl, false);
                if (isStlEntity)
                {
                    retVal = videoUrl;
                }
                else
                {
                    var url = _pathManager.GetSiteFilesUrl(site, Resources.BrPlayer.Swf);
                    retVal = $@"
<embed src=""{url}"" allowfullscreen=""true"" flashvars=""controlbar=over&autostart={StringUtils.ToLower(true
                        .ToString())}&image={string.Empty}&file={videoUrl}"" width=""{450}"" height=""{350}""/>
";
                }
            }
            return retVal;
        }

        public string GetFileHtmlWithCount(Site site, int channelId, int contentId, string fileUrl, NameValueCollection attributes, string innerHtml, bool isStlEntity, bool isLower, bool isUpper)
        {
            if (site == null || string.IsNullOrEmpty(fileUrl)) return string.Empty;

            string retVal;
            if (isStlEntity)
            {
                retVal = _pathManager.GetDownloadApiUrl(site, channelId, contentId,
                    fileUrl);
            }
            else
            {
                var linkAttributes = new NameValueCollection();
                TranslateUtils.AddAttributesIfNotExists(linkAttributes, attributes);
                linkAttributes["href"] = _pathManager.GetDownloadApiUrl(site, channelId,
                    contentId, fileUrl);

                innerHtml = string.IsNullOrEmpty(innerHtml)
                    ? PageUtils.GetFileNameFromUrl(fileUrl)
                    : innerHtml;

                if (isLower)
                {
                    innerHtml = StringUtils.ToLower(innerHtml);
                }
                if (isUpper)
                {
                    innerHtml = StringUtils.ToUpper(innerHtml);
                }

                retVal = $@"<a {TranslateUtils.ToAttributesString(linkAttributes)}>{innerHtml}</a>";
            }

            return retVal;
        }

        public string GetFileHtmlWithoutCount(Site site, string fileUrl, NameValueCollection attributes, string innerHtml, bool isStlEntity, bool isLower, bool isUpper)
        {
            if (site == null || string.IsNullOrEmpty(fileUrl)) return string.Empty;

            string retVal;
            if (isStlEntity)
            {
                retVal = _pathManager.GetDownloadApiUrl(site, fileUrl);
            }
            else
            {
                var linkAttributes = new NameValueCollection();
                TranslateUtils.AddAttributesIfNotExists(linkAttributes, attributes);
                linkAttributes["href"] = _pathManager.GetDownloadApiUrl( site, fileUrl);
                innerHtml = string.IsNullOrEmpty(innerHtml) ? PageUtils.GetFileNameFromUrl(fileUrl) : innerHtml;

                if (isLower)
                {
                    innerHtml = StringUtils.ToLower(innerHtml);
                }
                if (isUpper)
                {
                    innerHtml = StringUtils.ToUpper(innerHtml);
                }

                retVal = $@"<a {TranslateUtils.ToAttributesString(linkAttributes)}>{innerHtml}</a>";
            }

            return retVal;
        }
    }
}
