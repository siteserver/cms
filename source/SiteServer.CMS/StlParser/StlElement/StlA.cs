using System.Collections.Generic;
using System.Text;
using System.Web.UI.HtmlControls;
using BaiRong.Core;
using BaiRong.Core.Model.Attributes;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parsers;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "获取链接", Description = "通过 stl:a 标签在模板中创建链接，系统将根据所处上下文计算链接地址")]
    public class StlA
    {
        public const string ElementName = "stl:a";

        public const string AttributeId = "id";
        public const string AttributeChannelIndex = "channelIndex";
        public const string AttributeChannelName = "channelName";
        public const string AttributeParent = "parent";
        public const string AttributeUpLevel = "upLevel";
        public const string AttributeTopLevel = "topLevel";
        public const string AttributeContext = "context";
        public const string AttributeHref = "href";
        public const string AttributeHost = "host";
        public const string AttributeQueryString = "queryString"; 

        public static SortedList<string, string> AttributeList => new SortedList<string, string>
        {
            {AttributeId, "唯一标识符"},
            {AttributeChannelIndex, "栏目索引"},
            {AttributeChannelName, "栏目名称"},
            {AttributeParent, "显示父栏目"},
            {AttributeUpLevel, "上级栏目的级别"},
            {AttributeTopLevel, "从首页向下的栏目级别"},
            {AttributeContext, "所处上下文"},
            {AttributeHref, "链接地址"},
            {AttributeHost, "链接域名"},
            {AttributeQueryString, "链接参数"} 
        };

        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
        {
            var stlAnchor = new HtmlAnchor();
            var htmlId = string.Empty;
            var channelIndex = string.Empty;
            var channelName = string.Empty;
            var upLevel = 0;
            var topLevel = -1;
            const bool removeTarget = false;
            var href = string.Empty;
            var queryString = string.Empty;
            var host = string.Empty; 

            foreach (var name in contextInfo.Attributes.Keys)
            {
                var value = contextInfo.Attributes[name];
                if (StringUtils.EqualsIgnoreCase(name, AttributeId))
                {
                    htmlId = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeChannelIndex))
                {
                    channelIndex = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                    if (!string.IsNullOrEmpty(channelIndex))
                    {
                        contextInfo.ContextType = EContextType.Channel;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeChannelName))
                {
                    channelName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                    if (!string.IsNullOrEmpty(channelName))
                    {
                        contextInfo.ContextType = EContextType.Channel;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeParent))
                {
                    if (TranslateUtils.ToBool(value))
                    {
                        upLevel = 1;
                        contextInfo.ContextType = EContextType.Channel;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeUpLevel))
                {
                    upLevel = TranslateUtils.ToInt(value);
                    if (upLevel > 0)
                    {
                        contextInfo.ContextType = EContextType.Channel;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeTopLevel))
                {
                    topLevel = TranslateUtils.ToInt(value);
                    if (topLevel >= 0)
                    {
                        contextInfo.ContextType = EContextType.Channel;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeContext))
                {
                    contextInfo.ContextType = EContextTypeUtils.GetEnumType(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeHref))
                {
                    href = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeQueryString))
                {
                    queryString = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeHost))
                {
                    host = value;
                } 
                else
                {
                    ControlUtils.AddAttributeIfNotExists(stlAnchor, name, value);
                }
            } 

            var parsedContent = ParseImpl(pageInfo, contextInfo, stlAnchor, htmlId, channelIndex, channelName, upLevel, topLevel, removeTarget, href, queryString, host);

            return parsedContent;
        }

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, HtmlAnchor stlAnchor, string htmlId, string channelIndex, string channelName, int upLevel, int topLevel, bool removeTarget, string href, string queryString, string host)
        {
            if (!string.IsNullOrEmpty(htmlId) && !string.IsNullOrEmpty(contextInfo.ContainerClientId))
            {
                htmlId = contextInfo.ContainerClientId + "_" + htmlId;
            }
            stlAnchor.ID = htmlId;

            var url = string.Empty;
            var onclick = string.Empty;
            if (!string.IsNullOrEmpty(href))
            {
                url = PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemInfo, href);

                var innerBuilder = new StringBuilder(contextInfo.InnerXml);
                StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                stlAnchor.InnerHtml = innerBuilder.ToString();
            }
            else
            {
                if (contextInfo.ContextType == EContextType.Undefined)
                {
                    contextInfo.ContextType = contextInfo.ContentId != 0 ? EContextType.Content : EContextType.Channel;
                }
                if (contextInfo.ContextType == EContextType.Content)//获取内容Url
                {
                    if (contextInfo.ContentInfo != null)
                    {
                        url = PageUtility.GetContentUrl(pageInfo.PublishmentSystemInfo, contextInfo.ContentInfo);
                    }
                    else
                    {
                        var nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelId);
                        url = PageUtility.GetContentUrl(pageInfo.PublishmentSystemInfo, nodeInfo, contextInfo.ContentId, false);
                    }
                    if (string.IsNullOrEmpty(contextInfo.InnerXml))
                    {
                        var title = contextInfo.ContentInfo?.Title;
                        title = ContentUtility.FormatTitle(contextInfo.ContentInfo?.GetExtendedAttribute("BackgroundContentAttribute.TitleFormatString"), title);

                        if (pageInfo.PublishmentSystemInfo.Additional.IsContentTitleBreakLine)
                        {
                            title = title.Replace("  ", string.Empty);
                        }

                        stlAnchor.InnerHtml = title;
                    }
                    else
                    {
                        var innerBuilder = new StringBuilder(contextInfo.InnerXml);
                        StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                        stlAnchor.InnerHtml = innerBuilder.ToString();
                    }
                }
                else if (contextInfo.ContextType == EContextType.Channel)//获取栏目Url
                {
                    contextInfo.ChannelId = StlDataUtility.GetNodeIdByLevel(pageInfo.PublishmentSystemId, contextInfo.ChannelId, upLevel, topLevel);
                    contextInfo.ChannelId = StlDataUtility.GetNodeIdByChannelIdOrChannelIndexOrChannelName(pageInfo.PublishmentSystemId, contextInfo.ChannelId, channelIndex, channelName);
                    var channel = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelId);

                    url = PageUtility.GetChannelUrl(pageInfo.PublishmentSystemInfo, channel);
                    if (contextInfo.InnerXml.Trim().Length == 0)
                    {
                        stlAnchor.InnerHtml = channel.NodeName;
                    }
                    else
                    {
                        var innerBuilder = new StringBuilder(contextInfo.InnerXml);
                        StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                        stlAnchor.InnerHtml = innerBuilder.ToString();
                    }
                }
            }

            if (url.Equals(PageUtils.UnclickedUrl))
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

            stlAnchor.HRef = url;

            if (!string.IsNullOrEmpty(onclick))
            {
                stlAnchor.Attributes.Add("onclick", onclick);
            }

            if (removeTarget)
            {
                stlAnchor.Target = string.Empty;
            }

            // 如果是实体标签，则只返回url
            if (contextInfo.IsCurlyBrace)
            {
                return stlAnchor.HRef;
            }
            else
            {
                return ControlUtils.GetControlRenderHtml(stlAnchor);
            }
        }
    }
}
