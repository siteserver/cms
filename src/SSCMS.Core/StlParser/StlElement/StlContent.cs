using System.Text;
using System.Threading.Tasks;
using SSCMS.Configuration;
using SSCMS.Core.StlParser.Attributes;
using SSCMS.Core.StlParser.Utility;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Parse;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "获取内容值", Description = "通过 stl:content 标签在模板中显示指定内容的属性值")]
    public static class StlContent
    {
        public const string ElementName = "stl:content";

        [StlAttribute(Title = "显示的类型")]
        private const string Type = nameof(Type);

        [StlAttribute(Title = "显示在信息前的文字")]
        private const string LeftText = nameof(LeftText);

        [StlAttribute(Title = "显示在信息后的文字")]
        private const string RightText = nameof(RightText);

        [StlAttribute(Title = "显示的格式")]
        private const string Format = nameof(Format);

        [StlAttribute(Title = "显示的格式")]
        private const string FormatString = nameof(FormatString);

        [StlAttribute(Title = "显示第几项")]
        private const string No = nameof(No);

        [StlAttribute(Title = "显示多项时的分割字符串")]
        private const string Separator = nameof(Separator);

        [StlAttribute(Title = "字符开始位置")]
        private const string StartIndex = nameof(StartIndex);

        [StlAttribute(Title = "指定字符长度")]
        private const string Length = nameof(Length);

        [StlAttribute(Title = "显示字符的数目")]
        private const string WordNum = nameof(WordNum);

        [StlAttribute(Title = "文字超出部分显示的文字")]
        private const string Ellipsis = nameof(Ellipsis);

        [StlAttribute(Title = "需要替换的文字，可以是正则表达式")]
        private const string Replace = nameof(Replace);

        [StlAttribute(Title = "替换replace的文字信息")]
        private const string To = nameof(To);

        [StlAttribute(Title = "是否清除HTML标签")]
        private const string IsClearTags = nameof(IsClearTags);

        [StlAttribute(Title = "是否将回车替换为HTML换行标签")]
        private const string IsReturnToBr = nameof(IsReturnToBr);

        [StlAttribute(Title = "是否转换为小写")]
        private const string IsLower = nameof(IsLower);

        [StlAttribute(Title = "是否转换为大写")]
        private const string IsUpper = nameof(IsUpper);

        [StlAttribute(Title = "如果是引用内容，是否获取所引用内容的值")]
        private const string IsOriginal = nameof(IsOriginal);

        [StlAttribute(Title = "是否自增长")]
        private const string IsAutoIncrease = nameof(IsAutoIncrease);
        
        public static async Task<object> ParseAsync(IParseManager parseManager)
        {
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;

            var leftText = string.Empty;
            var rightText = string.Empty;
            var format = string.Empty;
            var no = "0";
            string separator = null;
            var startIndex = 0;
            var length = 0;
            var wordNum = 0;
            var ellipsis = Constants.Ellipsis;
            var replace = string.Empty;
            var to = string.Empty;
            var isClearTags = false;
            var isReturnToBrStr = string.Empty;
            var isLower = false;
            var isUpper = false;
            var isOriginal = true;//引用的时候，默认使用原来的数据
            var type = string.Empty;
            var isAutoIncrease = pageInfo.Template.TemplateType == TemplateType.ContentTemplate;

            foreach (var name in contextInfo.Attributes.AllKeys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, Type))
                {
                    type = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, LeftText))
                {
                    leftText = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, RightText))
                {
                    rightText = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Format) || StringUtils.EqualsIgnoreCase(name, FormatString))
                {
                    format = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, No))
                {
                    no = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Separator))
                {
                    separator = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, StartIndex))
                {
                    startIndex = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Length))
                {
                    length = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, WordNum))
                {
                    wordNum = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Ellipsis))
                {
                    ellipsis = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Replace))
                {
                    replace = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, To))
                {
                    to = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsClearTags))
                {
                    isClearTags = TranslateUtils.ToBool(value, true);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsReturnToBr))
                {
                    isReturnToBrStr = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsLower))
                {
                    isLower = TranslateUtils.ToBool(value, true);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsUpper))
                {
                    isUpper = TranslateUtils.ToBool(value, true);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsOriginal))
                {
                    isOriginal = TranslateUtils.ToBool(value, true);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsAutoIncrease))
                {
                    isAutoIncrease = TranslateUtils.ToBool(value, false);
                }
            }

            var contentId = contextInfo.ContentId;
            var content = await parseManager.GetContentAsync();

            if (contextInfo.IsStlEntity && string.IsNullOrEmpty(type))
            {
                return content.ToDictionary();
            }

            var parsedContent = await ParseAsync(parseManager, leftText, rightText, format, no, separator, startIndex, length, wordNum, ellipsis, replace, to, isClearTags, isReturnToBrStr, isLower, isUpper, isOriginal, isAutoIncrease, type, content, contentId);

            var innerBuilder = new StringBuilder(parsedContent);
            await parseManager.ParseInnerContentAsync(innerBuilder);
            parsedContent = innerBuilder.ToString();

            if (!StringUtils.EqualsIgnoreCase(type, ColumnsManager.PageContent))
            {
                parsedContent = parsedContent.Replace(Constants.PagePlaceHolder, string.Empty);
            }

            return parsedContent;
        }

        private static async Task<string> ParseAsync(IParseManager parseManager, string leftText, string rightText, string format, string no, string separator, int startIndex, int length, int wordNum, string ellipsis, string replace, string to, bool isClearTags, string isReturnToBrStr, bool isLower, bool isUpper, bool isOriginal, bool isAutoIncrease, string type, Content content, int contentId)
        {
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;
            var databaseManager = parseManager.DatabaseManager;

            if (content == null) return string.Empty;

            var parsedContent = string.Empty;

            if (string.IsNullOrEmpty(type))
            {
                type = nameof(Content.Title);
            }

            var isReturnToBr = false;
            if (string.IsNullOrEmpty(isReturnToBrStr))
            {
                if (StringUtils.EqualsIgnoreCase(type, nameof(Content.Summary)))
                {
                    isReturnToBr = true;
                }
            }
            else
            {
                isReturnToBr = TranslateUtils.ToBool(isReturnToBrStr, true);
            }

            if (isOriginal)
            {
                if (content.ReferenceId > 0 && content.SourceId > 0)
                {
                    var targetChannelId = content.SourceId;
                    //var targetSiteId = databaseManager.ChannelRepository.GetSiteId(targetChannelId);
                    //var targetSiteId = await databaseManager.ChannelRepository.GetSiteIdAsync(targetChannelId);
                    //var targetSite = await databaseManager.SiteRepository.GetAsync(targetSiteId);
                    var targetChannel = await databaseManager.ChannelRepository.GetAsync(targetChannelId);

                    //var targetContentInfo = databaseManager.ContentRepository.GetContentInfo(tableStyle, tableName, content.ReferenceId);
                    var targetContentInfo = await databaseManager.ContentRepository.GetAsync(pageInfo.Site, targetChannel, content.ReferenceId);
                    if (targetContentInfo != null && targetContentInfo.ChannelId > 0)
                    {
                        //标题可以使用自己的
                        targetContentInfo.Title = content.Title;
                        content = targetContentInfo;
                    }
                }
            }

            if (!string.IsNullOrEmpty(format))
            {
                format = format.Trim();
                if (!format.StartsWith("{0"))
                {
                    format = "{0:" + format;
                }
                if (!format.EndsWith("}"))
                {
                    format = format + "}";
                }
            }

            if (contentId != 0)
            {
                if (StringUtils.EqualsIgnoreCase(type, nameof(Content.Title)))
                {
                    var channel = await databaseManager.ChannelRepository.GetAsync(content.ChannelId);
                    var relatedIdentities = databaseManager.TableStyleRepository.GetRelatedIdentities(channel);
                    var tableName = databaseManager.ChannelRepository.GetTableName(pageInfo.Site, channel);

                    var styleInfo = await databaseManager.TableStyleRepository.GetTableStyleAsync(tableName, type, relatedIdentities);

                    var inputParser = new InputParserManager(parseManager.PathManager);
                    parsedContent = await inputParser.GetContentByTableStyleAsync(content.Title, separator, pageInfo.Site, styleInfo, format, contextInfo.Attributes, contextInfo.InnerHtml, false);
                    parsedContent = InputTypeUtils.ParseString(styleInfo.InputType, parsedContent, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, format);

                    if (!isClearTags && !string.IsNullOrEmpty(content.Get<string>(ColumnsManager.GetFormatStringAttributeName(nameof(Content.Title)))))
                    {
                        parsedContent = ContentUtility.FormatTitle(content.Get<string>(ColumnsManager.GetFormatStringAttributeName(nameof(Content.Title))), parsedContent);
                    }

                    if (pageInfo.Site.IsContentTitleBreakLine)
                    {
                        parsedContent = parsedContent.Replace("  ", !contextInfo.IsInnerElement ? "<br />" : string.Empty);
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(type, nameof(Content.SubTitle)))
                {
                    parsedContent = InputTypeUtils.ParseString(InputType.Text, content.SubTitle, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, format);
                    if (pageInfo.Site.IsContentSubTitleBreakLine)
                    {
                        parsedContent = parsedContent?.Replace("  ", !contextInfo.IsInnerElement ? "<br />" : string.Empty);
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(type, nameof(Content.Summary)))
                {
                    parsedContent = InputTypeUtils.ParseString(InputType.TextArea,  content.Summary, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, format);
                }
                else if (StringUtils.EqualsIgnoreCase(type, nameof(Content.Body)) || StringUtils.EqualsIgnoreCase(type, nameof(Content)))
                {
                    parsedContent = await parseManager.PathManager.DecodeTextEditorAsync(pageInfo.Site, content.Body, pageInfo.IsLocal);

                    // if (isClearTags)
                    // {
                    //     parsedContent = StringUtils.StripTags(parsedContent);
                    // }
                    
                    // if (!string.IsNullOrEmpty(replace))
                    // {
                    //     parsedContent = StringUtils.Replace(parsedContent, replace, to);
                    // }

                    // if (wordNum > 0 && !string.IsNullOrEmpty(parsedContent))
                    // {
                    //     parsedContent = StringUtils.MaxLengthText(parsedContent, wordNum, ellipsis);
                    // }

                    // if (!string.IsNullOrEmpty(format))
                    // {
                    //     parsedContent = string.Format(format, parsedContent);
                    // }

                    parsedContent = InputTypeUtils.ParseString(InputType.TextEditor, parsedContent, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, format);

                    parsedContent = await EditorUtility.ParseAsync(pageInfo, parsedContent);
                }
                else if (StringUtils.EqualsIgnoreCase(type, nameof(ColumnsManager.PageContent)))
                {
                    //if (contextInfo.IsInnerElement)
                    // {
                    parsedContent = await parseManager.PathManager.DecodeTextEditorAsync(pageInfo.Site, content.Body, pageInfo.IsLocal);

                    // if (isClearTags)
                    // {
                    //     parsedContent = StringUtils.StripTags(parsedContent);
                    // }

                    // if (!string.IsNullOrEmpty(replace))
                    // {
                    //     parsedContent = StringUtils.Replace(parsedContent, replace, to);
                    // }

                    // if (wordNum > 0 && !string.IsNullOrEmpty(parsedContent))
                    // {
                    //     parsedContent = StringUtils.MaxLengthText(parsedContent, wordNum, ellipsis);
                    // }

                    // if (!string.IsNullOrEmpty(format))
                    // {
                    //     parsedContent = string.Format(format, parsedContent);
                    // }

                    parsedContent = InputTypeUtils.ParseString(InputType.TextEditor, parsedContent, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, format);

                    parsedContent = await EditorUtility.ParseAsync(pageInfo, parsedContent);
                }
                else if (StringUtils.EqualsIgnoreCase(type, nameof(Content.AddDate)))
                {
                    parsedContent = DateUtils.Format(content.AddDate, format);
                }
                else if (StringUtils.EqualsIgnoreCase(type, nameof(Content.LastModifiedDate)))
                {
                    parsedContent = DateUtils.Format(content.LastModifiedDate, format);
                }
                else if (StringUtils.EqualsIgnoreCase(type, nameof(Content.ImageUrl)))
                {
                    var inputParser = new InputParserManager(parseManager.PathManager);

                    if (no == "all")
                    {
                        var sbParsedContent = new StringBuilder();
                        //第一条
                        sbParsedContent.Append(contextInfo.IsStlEntity
                            ? await parseManager.PathManager.ParseSiteUrlAsync(pageInfo.Site,
                                content.ImageUrl, pageInfo.IsLocal)
                            : await inputParser.GetImageOrFlashHtmlAsync(pageInfo.Site,
                                content.ImageUrl,
                                contextInfo.Attributes, false));

                        //第n条
                        var countName = ColumnsManager.GetCountName(nameof(Content.ImageUrl));
                        var count = content.Get<int>(countName);
                        for (var i = 1; i <= count; i++)
                        {
                            var extendName = ColumnsManager.GetExtendName(nameof(Content.ImageUrl), i);
                            var extend = content.Get<string>(extendName);

                            sbParsedContent.Append(contextInfo.IsStlEntity
                                ? await parseManager.PathManager.ParseSiteUrlAsync(pageInfo.Site, extend,
                                    pageInfo.IsLocal)
                                : await inputParser.GetImageOrFlashHtmlAsync(pageInfo.Site, extend,
                                    contextInfo.Attributes, false));
                        }

                        //var extendAttributeName = ColumnsManager.GetExtendAttributeName(nameof(Content.ImageUrl));
                        //var extendValues = content.Get<string>(extendAttributeName);
                        //if (!string.IsNullOrEmpty(extendValues))
                        //{
                        //    foreach (var extendValue in ListUtils.GetStringList(extendValues))
                        //    {
                        //        var newExtendValue = extendValue;
                        //        sbParsedContent.Append(contextInfo.IsStlEntity
                        //            ? await parseManager.PathManager.ParseNavigationUrlAsync(pageInfo.Site, newExtendValue, pageInfo.IsLocal)
                        //            : await inputParser.GetImageOrFlashHtmlAsync(pageInfo.Site, newExtendValue, contextInfo.Attributes, false));
                        //    }
                        //}

                        parsedContent = sbParsedContent.ToString();
                    }
                    else
                    {
                        var num = TranslateUtils.ToInt(no);
                        if (num <= 1)
                        {
                            parsedContent = contextInfo.IsStlEntity ? await parseManager.PathManager.ParseSiteUrlAsync(pageInfo.Site, content.ImageUrl, pageInfo.IsLocal) : await inputParser.GetImageOrFlashHtmlAsync(pageInfo.Site, content.ImageUrl, contextInfo.Attributes, false);
                        }
                        else
                        {
                            var extendName = ColumnsManager.GetExtendName(nameof(Content.ImageUrl), num - 1);
                            var extend = content.Get<string>(extendName);
                            if (!string.IsNullOrEmpty(extend))
                            {
                                parsedContent = contextInfo.IsStlEntity ? await parseManager.PathManager.ParseSiteUrlAsync(pageInfo.Site, extend, pageInfo.IsLocal) : await inputParser.GetImageOrFlashHtmlAsync(pageInfo.Site, extend, contextInfo.Attributes, false);
                            }

                            //var extendAttributeName = ColumnsManager.GetExtendAttributeName(nameof(Content.ImageUrl));
                            //var extendValues = content.Get<string>(extendAttributeName);
                            //if (!string.IsNullOrEmpty(extendValues))
                            //{
                            //    var index = 2;
                            //    foreach (var extendValue in ListUtils.GetStringList(extendValues))
                            //    {
                            //        var newExtendValue = extendValue;
                            //        if (index == num)
                            //        {
                            //            parsedContent = contextInfo.IsStlEntity ? await parseManager.PathManager.ParseNavigationUrlAsync(pageInfo.Site, newExtendValue, pageInfo.IsLocal) : await inputParser.GetImageOrFlashHtmlAsync(pageInfo.Site, newExtendValue, contextInfo.Attributes, false);
                            //            break;
                            //        }
                            //        index++;
                            //    }
                            //}
                        }
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(type, nameof(Content.VideoUrl)))
                {
                    var inputParser = new InputParserManager(parseManager.PathManager);

                    if (no == "all")
                    {
                        var sbParsedContent = new StringBuilder();
                        //第一条
                        sbParsedContent.Append(await inputParser.GetVideoHtmlAsync(pageInfo.Site, content.VideoUrl, contextInfo.Attributes, contextInfo.IsStlEntity));

                        //第n条
                        var countName = ColumnsManager.GetCountName(nameof(Content.VideoUrl));
                        var count = content.Get<int>(countName);
                        for (var i = 1; i <= count; i++)
                        {
                            var extendName = ColumnsManager.GetExtendName(nameof(Content.VideoUrl), i);
                            var extend = content.Get<string>(extendName);

                            sbParsedContent.Append(await inputParser.GetVideoHtmlAsync(pageInfo.Site,
                                extend, contextInfo.Attributes, contextInfo.IsStlEntity));
                        }

                        //var extendAttributeName = ColumnsManager.GetExtendAttributeName(nameof(Content.VideoUrl));
                        //var extendValues = content.Get<string>(extendAttributeName);
                        //if (!string.IsNullOrEmpty(extendValues))
                        //{
                        //    foreach (string extendValue in ListUtils.GetStringList(extendValues))
                        //    {

                        //        sbParsedContent.Append(await inputParser.GetVideoHtmlAsync(pageInfo.Config, pageInfo.Site, extendValue, contextInfo.Attributes, contextInfo.IsStlEntity));

                        //    }
                        //}

                        parsedContent = sbParsedContent.ToString();
                    }
                    else
                    {
                        var num = TranslateUtils.ToInt(no);
                        if (num <= 1)
                        {
                            parsedContent = await inputParser.GetVideoHtmlAsync(pageInfo.Site, content.VideoUrl, contextInfo.Attributes, contextInfo.IsStlEntity);
                        }
                        else
                        {
                            var extendName = ColumnsManager.GetExtendName(nameof(Content.VideoUrl), num - 1);
                            var extend = content.Get<string>(extendName);
                            if (!string.IsNullOrEmpty(extend))
                            {
                                parsedContent = await inputParser.GetVideoHtmlAsync(pageInfo.Site, extend, contextInfo.Attributes, contextInfo.IsStlEntity);
                            }
                        }
                    }

                }
                else if (StringUtils.EqualsIgnoreCase(type, nameof(Content.FileUrl)))
                {
                    if (no == "all")
                    {
                        var sbParsedContent = new StringBuilder();
                        if (contextInfo.IsStlEntity)
                        {
                            //第一条
                            sbParsedContent.Append(content.FileUrl);

                            //第n条
                            var countName = ColumnsManager.GetCountName(nameof(Content.FileUrl));
                            var count = content.Get<int>(countName);
                            for (var i = 1; i <= count; i++)
                            {
                                var extendName = ColumnsManager.GetExtendName(nameof(Content.FileUrl), i);
                                var extend = content.Get<string>(extendName);

                                sbParsedContent.Append(extend);
                            }
                        }
                        else
                        {
                            var inputParser = new InputParserManager(parseManager.PathManager);

                            //第一条
                            sbParsedContent.Append(inputParser.GetFileHtmlWithCount(pageInfo.Site, content.ChannelId, content.Id, content.FileUrl, contextInfo.Attributes, contextInfo.InnerHtml, false, isLower, isUpper));

                            //第n条
                            var countName = ColumnsManager.GetCountName(nameof(Content.FileUrl));
                            var count = content.Get<int>(countName);
                            for (var i = 1; i <= count; i++)
                            {
                                var extendName = ColumnsManager.GetExtendName(nameof(Content.FileUrl), i);
                                var extend = content.Get<string>(extendName);

                                sbParsedContent.Append(inputParser.GetFileHtmlWithCount(pageInfo.Site,
                                    content.ChannelId, content.Id, extend, contextInfo.Attributes,
                                    contextInfo.InnerHtml,
                                    false, isLower, isUpper));
                            }
                        }

                        parsedContent = sbParsedContent.ToString();
                    }
                    else
                    {
                        var num = TranslateUtils.ToInt(no);
                        if (contextInfo.IsStlEntity)
                        {
                            if (num <= 1)
                            {
                                parsedContent = content.FileUrl;
                            }
                            else
                            {
                                var extendName = ColumnsManager.GetExtendName(nameof(Content.FileUrl), num - 1);
                                var extend = content.Get<string>(extendName);
                                if (!string.IsNullOrEmpty(extend))
                                {
                                    parsedContent = extend;
                                }
                            }

                            if (!string.IsNullOrEmpty(parsedContent))
                            {
                                parsedContent = await parseManager.PathManager.ParseSiteUrlAsync(pageInfo.Site, parsedContent, pageInfo.IsLocal);
                            }
                        }
                        else
                        {
                            var inputParser = new InputParserManager(parseManager.PathManager);

                            if (num <= 1)
                            {
                                parsedContent = inputParser.GetFileHtmlWithCount(pageInfo.Site, content.ChannelId, content.Id, content.FileUrl, contextInfo.Attributes, contextInfo.InnerHtml, false, isLower, isUpper);
                            }
                            else
                            {
                                var extendName = ColumnsManager.GetExtendName(nameof(Content.FileUrl), num - 1);
                                var extend = content.Get<string>(extendName);
                                if (!string.IsNullOrEmpty(extend))
                                {
                                    parsedContent = inputParser.GetFileHtmlWithCount(pageInfo.Site,
                                        content.ChannelId, content.Id, extend, contextInfo.Attributes,
                                        contextInfo.InnerHtml, false, isLower, isUpper);
                                }
                            }
                        }
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(type, "Images"))
                {
                    if (!string.IsNullOrEmpty(content.ImageUrl))
                    {
                      var countName = ColumnsManager.GetCountName(nameof(Content.ImageUrl));
                      parsedContent = (content.Get<int>(countName) + 1).ToString();
                    }
                    else
                    {
                      parsedContent = "0";
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(type, "Videos"))
                {
                    if (!string.IsNullOrEmpty(content.VideoUrl))
                    {
                      var countName = ColumnsManager.GetCountName(nameof(Content.VideoUrl));
                      parsedContent = (content.Get<int>(countName) + 1).ToString();
                    }
                    else
                    {
                      parsedContent = "0";
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(type, "Files"))
                {
                    if (!string.IsNullOrEmpty(content.FileUrl))
                    {
                      var countName = ColumnsManager.GetCountName(nameof(Content.FileUrl));
                      parsedContent = (content.Get<int>(countName) + 1).ToString();
                    }
                    else
                    {
                      parsedContent = "0";
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(type, nameof(ColumnsManager.NavigationUrl)))
                {
                    if (contextInfo.Content != null)
                    {
                        parsedContent = await parseManager.PathManager.GetContentUrlAsync(pageInfo.Site, contextInfo.Content, pageInfo.IsLocal);
                    }
                    else
                    {
                        var channelInfo = await databaseManager.ChannelRepository.GetAsync(contextInfo.ChannelId);
                        parsedContent = await parseManager.PathManager.GetContentUrlAsync(pageInfo.Site, channelInfo, contextInfo.ContentId, pageInfo.IsLocal);
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(type, nameof(Content.TagNames)))
                {
                    parsedContent = ListUtils.ToString(content.TagNames);
                }
                else if (StringUtils.EqualsIgnoreCase(type, nameof(Content.Hits)))
                {
                    await pageInfo.AddPageHeadCodeIfNotExistsAsync(ParsePage.Const.Jquery);
                    var apiUrl = parseManager.PathManager.GetApiHostUrl(pageInfo.Site, Constants.ApiPrefix, Constants.ApiStlPrefix, Constants.RouteStlActionsHits);
                    var elementId = StringUtils.GetElementId();
                    parsedContent = @$"
<script id=""{elementId}"" type=""text/javascript"">
$(function(){{
  $.ajax({{
    type: ""POST"",
    url: ""{apiUrl}"",
    contentType: ""application/json"",
    data: JSON.stringify({TranslateUtils.JsonSerialize(new {
        SiteId = pageInfo.SiteId,
        ChannelId = contextInfo.ChannelId,
        ContentId = contextInfo.ContentId,
        AutoIncrease = isAutoIncrease
    })}),
    dataType: ""json"",
    success: function (result) {{ $(""#{elementId}"").before(result.value)  }}
  }});
}});
</script>
";
                }
                else if (StringUtils.StartsWithIgnoreCase(type, StlParserUtility.ItemIndex) && contextInfo.ItemContainer?.ContentItem != null)
                {
                    var itemIndex = StlParserUtility.ParseItemIndex(contextInfo.ItemContainer.ContentItem.Key, type, contextInfo);
                    parsedContent = !string.IsNullOrEmpty(format) ? string.Format(format, itemIndex) : itemIndex.ToString();
                }
                else
                {
                    var channel = await databaseManager.ChannelRepository.GetAsync(content.ChannelId);

                    if (content.ContainsKey(type))
                    {
                        if (!ListUtils.ContainsIgnoreCase(ColumnsManager.MetadataAttributes.Value, type))
                        {
                            var relatedIdentities = databaseManager.TableStyleRepository.GetRelatedIdentities(channel);
                            var tableName = databaseManager.ChannelRepository.GetTableName(pageInfo.Site, channel);
                            var styleInfo = await databaseManager.TableStyleRepository.GetTableStyleAsync(tableName, type, relatedIdentities);

                            //styleInfo.IsVisible = false 表示此字段不需要显示 styleInfo.TableStyleId = 0 不能排除，因为有可能是直接辅助表字段没有添加显示样式
                            var num = TranslateUtils.ToInt(no);

                            var inputParser = new InputParserManager(parseManager.PathManager);
                            parsedContent = await inputParser.GetContentByTableStyleAsync(content, separator, pageInfo.Site, styleInfo, format, num, contextInfo.Attributes, contextInfo.InnerHtml, false);
                            parsedContent = InputTypeUtils.ParseString(styleInfo.InputType, parsedContent, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, format);
                        }
                        else
                        {
                            parsedContent = content.Get<string>(type);
                            parsedContent = InputTypeUtils.ParseString(InputType.Text, parsedContent, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, format);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(parsedContent))
                {
                    parsedContent = leftText + parsedContent + rightText;
                }
            }

            return parsedContent;
        }
    }
}
