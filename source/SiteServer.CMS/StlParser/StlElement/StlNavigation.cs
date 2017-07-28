using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Xml;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "显示导航", Description = "通过 stl:navigation 标签在模板中显示链接导航")]
    public class StlNavigation
    {
        private StlNavigation() { }
        public const string ElementName = "stl:navigation";

        public const string AttributeType = "type";
        public const string AttributeEmptyText = "emptyText";
        public const string AttributeTipText = "tipText";
        public const string AttributeWordNum = "wordNum";
        public const string AttributeIsDynamic = "isDynamic";
        public const string AttributeIsKeyboard = "isKeyboard";

        public static SortedList<string, string> AttributeList => new SortedList<string, string>
        {
            {AttributeType, StringUtils.SortedListToAttributeValueString("类型", TypeList)},
            {AttributeEmptyText, "当无内容时显示的信息"},
            {AttributeTipText, "导航提示信息"},
            {AttributeWordNum, "显示字数"},
            {AttributeIsDynamic, "是否动态显示"},
            {AttributeIsKeyboard, "是否开启键盘，↑↓←→键分别为上下左右"}
        };

        public const string TypePreviousChannel = "PreviousChannel";
        public const string TypeNextChannel = "NextChannel";
        public const string TypePreviousContent = "PreviousContent";
        public const string TypeNextContent = "NextContent";

        public static SortedList<string, string> TypeList => new SortedList<string, string>
        {
            {TypePreviousChannel, "上一栏目链接"},
            {TypeNextChannel, "下一栏目链接"},
            {TypePreviousContent, "上一内容链接"},
            {TypeNextContent, "下一内容链接"}
        };

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfoRef)
        {
            string parsedContent;
            var contextInfo = contextInfoRef.Clone();
            try
            {
                var stlAnchor = new HtmlAnchor();
                var type = TypeNextContent;
                var emptyText = string.Empty;
                var tipText = string.Empty;
                var wordNum = 0;
                var isDynamic = false;
                var isKeyboard = false;

                var ie = node.Attributes?.GetEnumerator();
                if (ie != null)
                {
                    while (ie.MoveNext())
                    {
                        var attr = (XmlAttribute)ie.Current;

                        if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeType))
                        {
                            type = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeEmptyText))
                        {
                            emptyText = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeTipText))
                        {
                            tipText = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeWordNum))
                        {
                            wordNum = TranslateUtils.ToInt(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsDynamic))
                        {
                            isDynamic = TranslateUtils.ToBool(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsKeyboard))
                        {
                            isKeyboard = TranslateUtils.ToBool(attr.Value);
                        }
                        else
                        {
                            ControlUtils.AddAttributeIfNotExists(stlAnchor, attr.Name, attr.Value);
                        }
                    }
                }

                parsedContent = isDynamic ? StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo) : ParseImpl(node, pageInfo, contextInfo, stlAnchor, type, emptyText, tipText, wordNum, isKeyboard);
            }
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

            return parsedContent;
        }

        private static string ParseImpl(XmlNode node, PageInfo pageInfo, ContextInfo contextInfo, HtmlAnchor stlAnchor, string type, string emptyText, string tipText, int wordNum, bool isKeyboard)
        {
            string parsedContent;

            string successTemplateString;
            string failureTemplateString;
            StlParserUtility.GetYesOrNoTemplateString(node, pageInfo, out successTemplateString, out failureTemplateString);

            if (string.IsNullOrEmpty(successTemplateString))
            {
                var nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelId);

                if (type.ToLower().Equals(TypePreviousChannel.ToLower()) || type.ToLower().Equals(TypeNextChannel.ToLower()))
                {
                    var taxis = nodeInfo.Taxis;
                    var isNextChannel = !StringUtils.EqualsIgnoreCase(type, TypePreviousChannel);
                    var siblingNodeId = DataProvider.NodeDao.GetNodeIdByParentIdAndTaxis(nodeInfo.ParentId, taxis, isNextChannel);
                    if (siblingNodeId != 0)
                    {
                        var siblingNodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, siblingNodeId);
                        var url = PageUtility.GetChannelUrl(pageInfo.PublishmentSystemInfo, siblingNodeInfo);
                        if (url.Equals(PageUtils.UnclickedUrl))
                        {
                            stlAnchor.Target = string.Empty;
                        }
                        stlAnchor.HRef = url;

                        if (string.IsNullOrEmpty(node.InnerXml))
                        {
                            stlAnchor.InnerHtml = NodeManager.GetNodeName(pageInfo.PublishmentSystemId, siblingNodeId);
                            if (wordNum > 0)
                            {
                                stlAnchor.InnerHtml = StringUtils.MaxLengthText(stlAnchor.InnerHtml, wordNum);
                            }
                        }
                        else
                        {
                            contextInfo.ChannelId = siblingNodeId;
                            var innerBuilder = new StringBuilder(node.InnerXml);
                            StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                            stlAnchor.InnerHtml = innerBuilder.ToString();
                        }
                    }
                }
                else if (type.ToLower().Equals(TypePreviousContent.ToLower()) || type.ToLower().Equals(TypeNextContent.ToLower()))
                {
                    if (contextInfo.ContentId != 0)
                    {
                        var taxis = contextInfo.ContentInfo.Taxis;
                        var isNextContent = !StringUtils.EqualsIgnoreCase(type, TypePreviousContent);
                        var tableStyle = NodeManager.GetTableStyle(pageInfo.PublishmentSystemInfo, contextInfo.ChannelId);
                        var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, contextInfo.ChannelId);
                        var siblingContentId = BaiRongDataProvider.ContentDao.GetContentId(tableName, contextInfo.ChannelId, taxis, isNextContent);
                        if (siblingContentId != 0)
                        {
                            var siblingContentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, siblingContentId);
                            var url = PageUtility.GetContentUrl(pageInfo.PublishmentSystemInfo, siblingContentInfo);
                            if (url.Equals(PageUtils.UnclickedUrl))
                            {
                                stlAnchor.Target = string.Empty;
                            }
                            stlAnchor.HRef = url;

                            if (isKeyboard)
                            {
                                var keyCode = isNextContent ? 39 : 37;
                                var scriptContent = new StringBuilder();
                                pageInfo.AddPageScriptsIfNotExists(PageInfo.Components.Jquery);
                                scriptContent.Append($@"<script language=""javascript"" type=""text/javascript""> 
      $(document).keydown(function(event){{
        if(event.keyCode=={keyCode}){{location = '{url}';}}
      }});
</script> 
");
                                var nextOrPrevious = isNextContent ? "nextContent" : "previousContent";
                                pageInfo.SetPageScripts(nextOrPrevious, scriptContent.ToString(), true);
                            }

                            if (string.IsNullOrEmpty(node.InnerXml))
                            {
                                stlAnchor.InnerHtml = siblingContentInfo.Title;
                                if (wordNum > 0)
                                {
                                    stlAnchor.InnerHtml = StringUtils.MaxLengthText(stlAnchor.InnerHtml, wordNum);
                                }
                            }
                            else
                            {
                                var innerBuilder = new StringBuilder(node.InnerXml);
                                contextInfo.ContentId = siblingContentId;
                                StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                                stlAnchor.InnerHtml = innerBuilder.ToString();
                            }
                        }
                    }
                }

                parsedContent = string.IsNullOrEmpty(stlAnchor.HRef) ? emptyText : ControlUtils.GetControlRenderHtml(stlAnchor);
            }
            else
            {
                var nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelId);

                var isSuccess = false;
                var theContextInfo = contextInfo.Clone();

                if (type.ToLower().Equals(TypePreviousChannel.ToLower()) || type.ToLower().Equals(TypeNextChannel.ToLower()))
                {
                    var taxis = nodeInfo.Taxis;
                    var isNextChannel = !StringUtils.EqualsIgnoreCase(type, TypePreviousChannel);
                    var siblingNodeId = DataProvider.NodeDao.GetNodeIdByParentIdAndTaxis(nodeInfo.ParentId, taxis, isNextChannel);
                    if (siblingNodeId != 0)
                    {
                        isSuccess = true;
                        theContextInfo.ContextType = EContextType.Channel;
                        theContextInfo.ChannelId = siblingNodeId;
                    }
                }
                else if (type.ToLower().Equals(TypePreviousContent.ToLower()) || type.ToLower().Equals(TypeNextContent.ToLower()))
                {
                    if (contextInfo.ContentId != 0)
                    {
                        var taxis = contextInfo.ContentInfo.Taxis;
                        var isNextContent = !StringUtils.EqualsIgnoreCase(type, TypePreviousContent);
                        var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, contextInfo.ChannelId);
                        var siblingContentId = BaiRongDataProvider.ContentDao.GetContentId(tableName, contextInfo.ChannelId, taxis, isNextContent);
                        if (siblingContentId != 0)
                        {
                            isSuccess = true;
                            theContextInfo.ContextType = EContextType.Content;
                            theContextInfo.ContentId = siblingContentId;
                            theContextInfo.ContentInfo = null;
                        }
                    }
                }

                parsedContent = isSuccess ? successTemplateString : failureTemplateString;

                if (!string.IsNullOrEmpty(parsedContent))
                {
                    var innerBuilder = new StringBuilder(parsedContent);
                    StlParserManager.ParseInnerContent(innerBuilder, pageInfo, theContextInfo);

                    parsedContent = innerBuilder.ToString();
                }
            }

            parsedContent = tipText + parsedContent;

            return parsedContent;
        }
    }
}
