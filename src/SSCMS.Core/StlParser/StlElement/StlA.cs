using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SSCMS.Core.StlParser.Attributes;
using SSCMS.Parse;
using SSCMS.Core.StlParser.Utility;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "获取链接")]
    public static class StlA
    {
        public const string ElementName = "stl:a";

        [StlAttribute(Title = "栏目索引")]
        private const string ChannelIndex = nameof(ChannelIndex);

        [StlAttribute(Title = "栏目索引")]
        private const string Index = nameof(Index);

        [StlAttribute(Title = "栏目名称")]
        private const string ChannelName = nameof(ChannelName);

        [StlAttribute(Title = "显示父栏目")]
        private const string Parent = nameof(Parent);

        [StlAttribute(Title = "上级栏目的级别")]
        private const string UpLevel = nameof(UpLevel);

        [StlAttribute(Title = "从首页向下的栏目级别")]
        private const string TopLevel = nameof(TopLevel);

        [StlAttribute(Title = "所处上下文")]
        private const string Context = nameof(Context);

        [StlAttribute(Title = "链接地址")]
        private const string Href = nameof(Href);

        [StlAttribute(Title = "链接域名")]
        private const string Host = nameof(Host);

        [StlAttribute(Title = "链接参数")]
        private const string QueryString = nameof(QueryString);

        public static async Task<object> ParseAsync(IParseManager parseManager)
        {
            var attributes = new Dictionary<string, string>();
            var channelIndex = string.Empty;
            var channelName = string.Empty;
            var upLevel = 0;
            var topLevel = -1;
            const bool removeTarget = false;
            var href = string.Empty;
            var queryString = string.Empty;
            var host = string.Empty;

            foreach (var name in parseManager.ContextInfo.Attributes.AllKeys)
            {
                var value = parseManager.ContextInfo.Attributes[name];
                if (StringUtils.EqualsIgnoreCase(name, ChannelIndex) || StringUtils.EqualsIgnoreCase(name, Index))
                {
                    channelIndex = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                    if (!string.IsNullOrEmpty(channelIndex))
                    {
                        parseManager.ContextInfo.ContextType = ParseType.Channel;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, ChannelName))
                {
                    channelName = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                    if (!string.IsNullOrEmpty(channelName))
                    {
                        parseManager.ContextInfo.ContextType = ParseType.Channel;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, Parent))
                {
                    if (TranslateUtils.ToBool(value))
                    {
                        upLevel = 1;
                        parseManager.ContextInfo.ContextType = ParseType.Channel;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, UpLevel))
                {
                    upLevel = TranslateUtils.ToInt(value);
                    if (upLevel > 0)
                    {
                        parseManager.ContextInfo.ContextType = ParseType.Channel;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, TopLevel))
                {
                    topLevel = TranslateUtils.ToInt(value);
                    if (topLevel >= 0)
                    {
                        parseManager.ContextInfo.ContextType = ParseType.Channel;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, Context))
                {
                    parseManager.ContextInfo.ContextType = TranslateUtils.ToEnum(value, ParseType.Undefined);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Href))
                {
                    href = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, QueryString))
                {
                    queryString = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Host))
                {
                    host = value;
                }
                else
                {
                    attributes[name] = value;
                }
            }

            return await ParseAsync(parseManager, channelIndex, channelName, upLevel, topLevel,
                removeTarget, href, queryString, host, attributes);
        }

        private static async Task<string> ParseAsync(IParseManager parseManager, string channelIndex,
            string channelName, int upLevel, int topLevel, bool removeTarget, string href, string queryString,
            string host, Dictionary<string, string> attributes)
        {
            var databaseManager = parseManager.DatabaseManager;
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;

            attributes.TryGetValue("id", out var htmlId);

            if (!string.IsNullOrEmpty(htmlId) && !string.IsNullOrEmpty(contextInfo.ContainerClientId))
            {
                htmlId = contextInfo.ContainerClientId + "_" + htmlId;
            }

            if (!string.IsNullOrEmpty(htmlId))
            {
                attributes["id"] = htmlId;
            }

            var innerHtml = string.Empty;

            var url = string.Empty;
            var onclick = string.Empty;
            if (!string.IsNullOrEmpty(href))
            {
                url = await parseManager.PathManager.ParseSiteUrlAsync(pageInfo.Site, href, pageInfo.IsLocal);

                var innerBuilder = new StringBuilder(contextInfo.InnerHtml);
                await parseManager.ParseInnerContentAsync(innerBuilder);
                innerHtml = innerBuilder.ToString();
            }
            else
            {
                if (contextInfo.ContextType == ParseType.Undefined)
                {
                    contextInfo.ContextType = contextInfo.ContentId != 0 ? ParseType.Content : ParseType.Channel;
                }

                if (contextInfo.ContextType == ParseType.Content) //获取内容Url
                {
                    var contentInfo = await parseManager.GetContentAsync();
                    if (contentInfo != null)
                    {
                        url = await parseManager.PathManager.GetContentUrlAsync(pageInfo.Site, contentInfo, pageInfo.IsLocal);
                    }
                    else
                    {
                        var nodeInfo = await databaseManager.ChannelRepository.GetAsync(contextInfo.ChannelId);
                        url = await parseManager.PathManager.GetContentUrlAsync(pageInfo.Site, nodeInfo, contextInfo.ContentId,
                            pageInfo.IsLocal);
                    }

                    if (string.IsNullOrEmpty(contextInfo.InnerHtml))
                    {
                        var title = contentInfo?.Title;
                        title = ContentUtility.FormatTitle(
                            contentInfo?.Get<string>("BackgroundContentAttribute.TitleFormatString"), title);

                        if (pageInfo.Site.IsContentTitleBreakLine)
                        {
                            title = title.Replace("  ", string.Empty);
                        }

                        innerHtml = title;
                    }
                    else
                    {
                        var innerBuilder = new StringBuilder(contextInfo.InnerHtml);
                        await parseManager.ParseInnerContentAsync(innerBuilder);
                        innerHtml = innerBuilder.ToString();
                    }
                }
                else if (contextInfo.ContextType == ParseType.Channel) //获取栏目Url
                {
                    var dataManager = new StlDataManager(parseManager.DatabaseManager);
                    contextInfo.ChannelId =
                        await dataManager.GetChannelIdByLevelAsync(pageInfo.SiteId, contextInfo.ChannelId, upLevel, topLevel);
                    contextInfo.ChannelId =
                        await databaseManager.ChannelRepository.GetChannelIdAsync(pageInfo.SiteId,
                            contextInfo.ChannelId, channelIndex, channelName);
                    var channel = await databaseManager.ChannelRepository.GetAsync(contextInfo.ChannelId);

                    url = await parseManager.PathManager.GetChannelUrlAsync(pageInfo.Site, channel, pageInfo.IsLocal);
                    if (string.IsNullOrWhiteSpace(contextInfo.InnerHtml))
                    {
                        innerHtml = channel.ChannelName;
                    }
                    else
                    {
                        var innerBuilder = new StringBuilder(contextInfo.InnerHtml);
                        await parseManager.ParseInnerContentAsync(innerBuilder);
                        innerHtml = innerBuilder.ToString();
                    }
                }
            }

            if (url.Equals(PageUtils.UnClickableUrl))
            {
                removeTarget = true;
            }
            else
            {
                if (!string.IsNullOrEmpty(host))
                {
                    url = PageUtils.AddProtocolToUrl(url, host);
                }

                if (!string.IsNullOrEmpty(queryString))
                {
                    url = PageUtils.AddQueryString(url, queryString);
                }
            }

            attributes["href"] = url;

            if (!string.IsNullOrEmpty(onclick))
            {
                attributes["onclick"] = onclick;
            }

            if (removeTarget)
            {
                attributes["target"] = string.Empty;
            }

            // 如果是实体标签，则只返回url
            if (contextInfo.IsStlEntity)
            {
                return url;
            }

            if (pageInfo.EditMode == EditMode.Visual)
            {
                var editable = VisualUtility.GetEditable(pageInfo, contextInfo);
                var editableAttributes = VisualUtility.GetEditableAttributes(editable);
                foreach (var key in editableAttributes.AllKeys)
                {
                    attributes[key] = editableAttributes[key];
                }
            }

            return $@"<a {TranslateUtils.ToAttributesString(attributes)}>{innerHtml}</a>";
        }
    }
}
