using System.Collections.Specialized;
using System.Text;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Common;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Utils;

namespace SS.CMS.Core.StlParser.StlElement
{
    [StlElement(Title = "当前位置", Description = "通过 stl:location 标签在模板中插入页面的当前位置")]
    public class StlLocation
    {
        private StlLocation() { }
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

        //对“当前位置”（stl:location）元素进行解析
        public static string Parse(ParseContext parseContext)
        {
            var separator = " - ";
            var target = string.Empty;
            var linkClass = string.Empty;
            var wordNum = 0;
            var isContainSelf = true;

            foreach (var name in parseContext.Attributes.AllKeys)
            {
                var value = parseContext.Attributes[name];

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

            return ParseImpl(parseContext, separator, target, linkClass, wordNum, isContainSelf);
        }

        private static string ParseImpl(ParseContext parseContext, string separator, string target, string linkClass, int wordNum, bool isContainSelf)
        {
            if (!string.IsNullOrEmpty(parseContext.InnerHtml))
            {
                separator = parseContext.InnerHtml;
            }

            var nodeInfo = ChannelManager.GetChannelInfo(parseContext.SiteId, parseContext.ChannelId);

            var builder = new StringBuilder();

            var parentsPath = nodeInfo.ParentsPath;
            var parentsCount = nodeInfo.ParentsCount;
            if (parentsPath.Length != 0)
            {
                var nodePath = parentsPath;
                if (isContainSelf)
                {
                    nodePath = nodePath + "," + parseContext.ChannelId;
                }
                var channelIdArrayList = TranslateUtils.StringCollectionToStringList(nodePath);
                foreach (var channelIdStr in channelIdArrayList)
                {
                    var currentId = int.Parse(channelIdStr);
                    var currentNodeInfo = ChannelManager.GetChannelInfo(parseContext.SiteId, currentId);
                    if (currentId == parseContext.SiteId)
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
                        var url = parseContext.UrlManager.GetIndexPageUrl(parseContext.SiteInfo, parseContext.IsLocal);
                        if (url.Equals(PageUtils.UnclickedUrl))
                        {
                            attributes["target"] = string.Empty;
                        }
                        attributes["href"] = url;
                        var innerHtml = StringUtils.MaxLengthText(currentNodeInfo.ChannelName, wordNum);

                        TranslateUtils.AddAttributesIfNotExists(attributes, parseContext.Attributes);

                        builder.Append($@"<a {TranslateUtils.ToAttributesString(attributes)}>{innerHtml}</a>");

                        if (parentsCount > 0)
                        {
                            builder.Append(separator);
                        }
                    }
                    else if (currentId == parseContext.ChannelId)
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
                        var url = parseContext.UrlManager.GetChannelUrl(parseContext.SiteInfo, currentNodeInfo, parseContext.IsLocal);
                        if (url.Equals(PageUtils.UnclickedUrl))
                        {
                            attributes["target"] = string.Empty;
                        }
                        attributes["href"] = url;
                        var innerHtml = StringUtils.MaxLengthText(currentNodeInfo.ChannelName, wordNum);

                        TranslateUtils.AddAttributesIfNotExists(attributes, parseContext.Attributes);

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
                        var url = parseContext.UrlManager.GetChannelUrl(parseContext.SiteInfo, currentNodeInfo, parseContext.IsLocal);
                        if (url.Equals(PageUtils.UnclickedUrl))
                        {
                            attributes["target"] = string.Empty;
                        }
                        attributes["href"] = url;
                        var innerHtml = StringUtils.MaxLengthText(currentNodeInfo.ChannelName, wordNum);

                        TranslateUtils.AddAttributesIfNotExists(attributes, parseContext.Attributes);

                        builder.Append($@"<a {TranslateUtils.ToAttributesString(attributes)}>{innerHtml}</a>");

                        if (parentsCount > 0)
                        {
                            builder.Append(separator);
                        }
                    }
                }
            }

            return builder.ToString();
        }
    }
}
