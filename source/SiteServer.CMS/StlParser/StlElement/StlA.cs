using System;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Xml;
using BaiRong.Core;
using BaiRong.Core.Model.Attributes;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parser;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    public class StlA
    {
        private StlA() { }
        public const string ElementName = "stl:a";//获取链接

        public const string AttributeId = "id";							    //唯一标识符
        public const string AttributeChannelIndex = "channelindex";		    //栏目索引
        public const string AttributeChannelName = "channelname";			//栏目名称
        public const string AttributeParent = "parent";					    //显示父栏目
        public const string AttributeUpLevel = "uplevel";					//上级栏目的级别
        public const string AttributeTopLevel = "toplevel";				    //从首页向下的栏目级别
        public const string AttributeContext = "context";                   //所处上下文
        public const string AttributeHref = "href";						    //链接地址
        public const string AttributeHost = "host";                         //链接域名
        public const string AttributeQueryString = "querystring";           //链接参数
        public const string AttributeIsDynamic = "isdynamic";               //是否动态显示

        public static ListDictionary AttributeList => new ListDictionary
        {
            {AttributeId, "唯一标识符"},
            {AttributeChannelIndex, "栏目索引"},
            {AttributeChannelName, "栏目名称"},
            {AttributeParent, "显示父栏目"},
            {AttributeUpLevel, "上级栏目的级别"},
            {AttributeTopLevel, "从首页向下的栏目级别"},
            {AttributeContext, "所处上下文"},
            {AttributeHref, "链接地址"},
            {AttributeQueryString, "链接参数"},
            {AttributeIsDynamic, "是否动态显示"}
        };

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfoRef)
        {
            string parsedContent;
            var contextInfo = contextInfoRef.Clone();

            try
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
                var isDynamic = false;
                var host = string.Empty;

                var ie = node.Attributes?.GetEnumerator();
                if (ie != null)
                {
                    while (ie.MoveNext())
                    {
                        var attr = (XmlAttribute)ie.Current;
                        var attributeName = attr.Name.ToLower();
                        if (attributeName.Equals(AttributeId))
                        {
                            htmlId = attr.Value;
                        }
                        else if (attributeName.Equals(AttributeChannelIndex))
                        {
                            channelIndex = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                            if (!string.IsNullOrEmpty(channelIndex))
                            {
                                contextInfo.ContextType = EContextType.Channel;
                            }
                        }
                        else if (attributeName.Equals(AttributeChannelName))
                        {
                            channelName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                            if (!string.IsNullOrEmpty(channelName))
                            {
                                contextInfo.ContextType = EContextType.Channel;
                            }
                        }
                        else if (attributeName.Equals(AttributeParent))
                        {
                            if (TranslateUtils.ToBool(attr.Value))
                            {
                                upLevel = 1;
                                contextInfo.ContextType = EContextType.Channel;
                            }
                        }
                        else if (attributeName.Equals(AttributeUpLevel))
                        {
                            upLevel = TranslateUtils.ToInt(attr.Value);
                            if (upLevel > 0)
                            {
                                contextInfo.ContextType = EContextType.Channel;
                            }
                        }
                        else if (attributeName.Equals(AttributeTopLevel))
                        {
                            topLevel = TranslateUtils.ToInt(attr.Value);
                            if (topLevel >= 0)
                            {
                                contextInfo.ContextType = EContextType.Channel;
                            }
                        }
                        else if (attributeName.Equals(AttributeContext))
                        {
                            contextInfo.ContextType = EContextTypeUtils.GetEnumType(attr.Value);
                        }
                        else if (attributeName.Equals(AttributeHref))
                        {
                            href = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                        }
                        else if (attributeName.Equals(AttributeQueryString))
                        {
                            queryString = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                        }
                        else if (attributeName.Equals(AttributeIsDynamic))
                        {
                            isDynamic = TranslateUtils.ToBool(attr.Value, false);
                        }
                        else if (attributeName.Equals(AttributeHost))
                        {
                            host = attr.Value;
                        }
                        else
                        {
                            ControlUtils.AddAttributeIfNotExists(stlAnchor, attributeName, attr.Value);
                        }
                    }
                }

                parsedContent = isDynamic ? StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo) : ParseImpl(pageInfo, contextInfo, node, stlAnchor, htmlId, channelIndex, channelName, upLevel, topLevel, removeTarget, href, queryString, host);
            }
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

            return parsedContent;
        }

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, XmlNode node, HtmlAnchor stlAnchor, string htmlId, string channelIndex, string channelName, int upLevel, int topLevel, bool removeTarget, string href, string queryString, string host)
        {
            if (!string.IsNullOrEmpty(htmlId) && !string.IsNullOrEmpty(contextInfo.ContainerClientID))
            {
                htmlId = contextInfo.ContainerClientID + "_" + htmlId;
            }
            stlAnchor.ID = htmlId;

            var url = string.Empty;
            var onclick = string.Empty;
            if (!string.IsNullOrEmpty(href))
            {
                url = PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemInfo, href);

                var innerBuilder = new StringBuilder(node.InnerXml);
                StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                stlAnchor.InnerHtml = innerBuilder.ToString();
            }
            else
            {
                if (contextInfo.ContextType == EContextType.Undefined)
                {
                    contextInfo.ContextType = contextInfo.ContentID != 0 ? EContextType.Content : EContextType.Channel;
                }
                if (contextInfo.ContextType == EContextType.Content)//获取内容Url
                {
                    if (contextInfo.ContentInfo != null)
                    {
                        url = PageUtility.GetContentUrl(pageInfo.PublishmentSystemInfo, contextInfo.ContentInfo);
                    }
                    else
                    {
                        var nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelID);
                        url = PageUtility.GetContentUrl(pageInfo.PublishmentSystemInfo, nodeInfo, contextInfo.ContentID, false);
                    }
                    if (string.IsNullOrEmpty(node.InnerXml))
                    {
                        var title = StringUtils.MaxLengthText(contextInfo.ContentInfo?.Title, contextInfo.TitleWordNum);
                        title = ContentUtility.FormatTitle(contextInfo.ContentInfo?.Attributes[BackgroundContentAttribute.TitleFormatString], title);

                        if (pageInfo.PublishmentSystemInfo.Additional.IsContentTitleBreakLine)
                        {
                            title = title.Replace("  ", string.Empty);
                        }

                        stlAnchor.InnerHtml = title;
                    }
                    else
                    {
                        var innerBuilder = new StringBuilder(node.InnerXml);
                        StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                        stlAnchor.InnerHtml = innerBuilder.ToString();
                    }
                }
                else if (contextInfo.ContextType == EContextType.Channel)//获取栏目Url
                {
                    contextInfo.ChannelID = StlDataUtility.GetNodeIdByLevel(pageInfo.PublishmentSystemId, contextInfo.ChannelID, upLevel, topLevel);
                    contextInfo.ChannelID = StlCacheManager.NodeId.GetNodeIdByChannelIdOrChannelIndexOrChannelName(pageInfo.PublishmentSystemId, contextInfo.ChannelID, channelIndex, channelName);
                    var channel = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelID);

                    url = PageUtility.GetChannelUrl(pageInfo.PublishmentSystemInfo, channel);
                    if (node.InnerXml.Trim().Length == 0)
                    {
                        stlAnchor.InnerHtml = channel.NodeName;
                    }
                    else
                    {
                        var innerBuilder = new StringBuilder(node.InnerXml);
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

            return ControlUtils.GetControlRenderHtml(stlAnchor);
        }
    }
}
