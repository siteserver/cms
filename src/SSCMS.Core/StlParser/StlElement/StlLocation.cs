using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSCMS.Core.StlParser.Attributes;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "当前位置", Description = "通过 stl:location 标签在模板中插入页面的当前位置")]
    public static class StlLocation
    {
        public const string ElementName = "stl:location";

        [StlAttribute(Title = "当前位置分隔符")]
        private const string Separator = nameof(Separator);

        [StlAttribute(Title = "打开窗口的目标")]
        private const string Target = nameof(Target);

        [StlAttribute(Title = "链接CSS样式")]
        private const string LinkClass = nameof(LinkClass);

        [StlAttribute(Title = "链接字数")]
        private const string WordNum = nameof(WordNum);

        [StlAttribute(Title = "是否包含当前栏目")]
        private const string IsContainSelf = nameof(IsContainSelf);

        public static async Task<object> ParseAsync(IParseManager parseManager)
        {
            var separator = " - ";
            var target = string.Empty;
            var linkClass = string.Empty;
            var wordNum = 0;
            var isContainSelf = true;

            foreach (var name in parseManager.ContextInfo.Attributes.AllKeys)
            {
                var value = parseManager.ContextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, Separator))
                {
                    separator = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Target))
                {
                    target = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, LinkClass))
                {
                    linkClass = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, WordNum))
                {
                    wordNum = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsContainSelf))
                {
                    isContainSelf = TranslateUtils.ToBool(value);
                }
            }

            return await ParseAsync(parseManager, separator, target, linkClass, wordNum, isContainSelf);
        }

        private static async Task<string> ParseAsync(IParseManager parseManager, string separator, string target, string linkClass, int wordNum, bool isContainSelf)
        {
            var databaseManager = parseManager.DatabaseManager;
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;

            if (!string.IsNullOrEmpty(contextInfo.InnerHtml))
            {
                separator = contextInfo.InnerHtml;
            }

            var channel = await databaseManager.ChannelRepository.GetAsync(contextInfo.ChannelId);

            var builder = new StringBuilder();

            var parentsCount = channel.ParentsCount;
            var nodePath = ListUtils.GetIntList(channel.ParentsPath);
            if (isContainSelf)
            {
                nodePath.Add(contextInfo.ChannelId);
            }
            foreach (var currentId in nodePath.Distinct())
            {
                var currentNodeInfo = await databaseManager.ChannelRepository.GetAsync(currentId);
                if (currentId == pageInfo.SiteId)
                {
                    var attributes = new NameValueCollection();
                    if (!string.IsNullOrEmpty(target))
                    {
                        attributes["target"] = target;
                    }
                    if (!string.IsNullOrEmpty(linkClass))
                    {
                        attributes["class"] = linkClass;
                    }
                    var url = await parseManager.PathManager.GetIndexPageUrlAsync(pageInfo.Site, pageInfo.IsLocal);
                    if (url.Equals(PageUtils.UnClickableUrl))
                    {
                        attributes["target"] = string.Empty;
                    }
                    attributes["href"] = url;
                    var innerHtml = StringUtils.MaxLengthText(currentNodeInfo.ChannelName, wordNum);

                    TranslateUtils.AddAttributesIfNotExists(attributes, contextInfo.Attributes);

                    builder.Append($@"<a {TranslateUtils.ToAttributesString(attributes)}>{innerHtml}</a>");

                    if (parentsCount > 0)
                    {
                        builder.Append(separator);
                    }
                }
                else if (currentId == contextInfo.ChannelId)
                {
                    var attributes = new NameValueCollection();
                    if (!string.IsNullOrEmpty(target))
                    {
                        attributes["target"] = target;
                    }
                    if (!string.IsNullOrEmpty(linkClass))
                    {
                        attributes["class"] = linkClass;
                    }
                    var url = await parseManager.PathManager.GetChannelUrlAsync(pageInfo.Site, currentNodeInfo, pageInfo.IsLocal);
                    if (url.Equals(PageUtils.UnClickableUrl))
                    {
                        attributes["target"] = string.Empty;
                    }
                    attributes["href"] = url;
                    var innerHtml = StringUtils.MaxLengthText(currentNodeInfo.ChannelName, wordNum);

                    TranslateUtils.AddAttributesIfNotExists(attributes, contextInfo.Attributes);

                    builder.Append($@"<a {TranslateUtils.ToAttributesString(attributes)}>{innerHtml}</a>");
                }
                else
                {
                    var attributes = new NameValueCollection();
                    if (!string.IsNullOrEmpty(target))
                    {
                        attributes["target"] = target;
                    }
                    if (!string.IsNullOrEmpty(linkClass))
                    {
                        attributes["class"] = linkClass;
                    }
                    var url = await parseManager.PathManager.GetChannelUrlAsync(pageInfo.Site, currentNodeInfo, pageInfo.IsLocal);
                    if (url.Equals(PageUtils.UnClickableUrl))
                    {
                        attributes["target"] = string.Empty;
                    }
                    attributes["href"] = url;
                    var innerHtml = StringUtils.MaxLengthText(currentNodeInfo.ChannelName, wordNum);

                    TranslateUtils.AddAttributesIfNotExists(attributes, contextInfo.Attributes);

                    builder.Append($@"<a {TranslateUtils.ToAttributesString(attributes)}>{innerHtml}</a>");

                    if (parentsCount > 0)
                    {
                        builder.Append(separator);
                    }
                }
            }

            return builder.ToString();
        }
    }
}
