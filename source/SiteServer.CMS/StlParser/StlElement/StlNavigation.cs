using System;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Xml;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    public class StlNavigation
    {
        private StlNavigation() { }
        public const string ElementName = "stl:navigation";//显示导航

        public const string Attribute_Type = "type";							//导航的类型
        public const string Attribute_EmptyText = "emptytext";					//当无内容时显示的信息
        public const string Attribute_TipText = "tiptext";					    //导航提示信息
        public const string Attribute_WordNum = "wordnum";					    //显示字数
        public const string Attribute_IsDisplayIfEmpty = "isdisplayifempty";    //当没链接时是否显示
        public const string Attribute_IsDynamic = "isdynamic";                  //是否动态显示
        public const string Attribute_IsKeyboard = "iskeyboard";                //是否开启键盘，←→↑↓键分别为上（下）一内容（栏目）链接

        public const string Type_PreviousChannel = "PreviousChannel";			//上一栏目链接
        public const string Type_NextChannel = "NextChannel";					//下一栏目链接
        public const string Type_PreviousContent = "PreviousContent";			//上一内容链接
        public const string Type_NextContent = "NextContent";					//下一内容链接

        public static ListDictionary AttributeList
        {
            get
            {
                var attributes = new ListDictionary();
                attributes.Add(Attribute_Type, "显示的类型");
                attributes.Add(Attribute_EmptyText, "当无内容时显示的信息");
                attributes.Add(Attribute_TipText, "导航提示信息");
                attributes.Add(Attribute_WordNum, "显示字数");
                attributes.Add(Attribute_IsDisplayIfEmpty, "当没链接时是否显示");
                attributes.Add(Attribute_IsDynamic, "是否动态显示");
                attributes.Add(Attribute_IsKeyboard, "是否开启键盘");
                return attributes;
            }
        }

        //对“导航”（stl:navigation）元素进行解析
        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfoRef)
        {
            var parsedContent = string.Empty;
            var contextInfo = contextInfoRef.Clone();
            try
            {
                var stlAnchor = new HtmlAnchor();
                var ie = node.Attributes.GetEnumerator();
                var type = Type_NextContent;
                var emptyText = string.Empty;
                var tipText = string.Empty;
                var wordNum = 0;
                var isDisplayIfEmpty = false;
                var isDynamic = false;
                var isKeyboard = false;

                while (ie.MoveNext())
                {
                    var attr = (XmlAttribute)ie.Current;
                    var attributeName = attr.Name.ToLower();
                    if (attributeName.Equals(Attribute_Type))
                    {
                        type = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_EmptyText))
                    {
                        emptyText = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_TipText))
                    {
                        tipText = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_WordNum))
                    {
                        wordNum = TranslateUtils.ToInt(attr.Value);
                    }
                    else if (attributeName.Equals(Attribute_IsDisplayIfEmpty))
                    {
                        isDisplayIfEmpty = TranslateUtils.ToBool(attr.Value);
                    }
                    else if (attributeName.Equals(Attribute_IsDynamic))
                    {
                        isDynamic = TranslateUtils.ToBool(attr.Value);
                    }
                    else if (attributeName.Equals(Attribute_IsKeyboard))
                    {
                        isKeyboard = TranslateUtils.ToBool(attr.Value);
                    }
                    else
                    {
                        ControlUtils.AddAttributeIfNotExists(stlAnchor, attributeName, attr.Value);
                    }
                }

                if (isDynamic)
                {
                    parsedContent = StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo);
                }
                else
                {
                    parsedContent = ParseImpl(node, pageInfo, contextInfo, stlAnchor, type, emptyText, tipText, wordNum, isDisplayIfEmpty, isKeyboard);
                }
            }
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

            return parsedContent;
        }

        private static string ParseImpl(XmlNode node, PageInfo pageInfo, ContextInfo contextInfo, HtmlAnchor stlAnchor, string type, string emptyText, string tipText, int wordNum, bool isDisplayIfEmpty, bool isKeyboard)
        {
            var parsedContent = string.Empty;

            var successTemplateString = string.Empty;
            var failureTemplateString = string.Empty;

            StlParserUtility.GetYesOrNoTemplateString(node, pageInfo, out successTemplateString, out failureTemplateString);

            if (string.IsNullOrEmpty(successTemplateString))
            {
                var nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelID);

                if (type.ToLower().Equals(Type_PreviousChannel.ToLower()) || type.ToLower().Equals(Type_NextChannel.ToLower()))
                {
                    var taxis = nodeInfo.Taxis;
                    var isNextChannel = true;
                    if (StringUtils.EqualsIgnoreCase(type, Type_PreviousChannel))
                    {
                        isNextChannel = false;
                    }
                    var siblingNodeID = DataProvider.NodeDao.GetNodeIdByParentIdAndTaxis(nodeInfo.ParentId, taxis, isNextChannel);
                    if (siblingNodeID != 0)
                    {
                        var siblingNodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, siblingNodeID);
                        var url = PageUtility.GetChannelUrl(pageInfo.PublishmentSystemInfo, siblingNodeInfo);
                        if (url.Equals(PageUtils.UnclickedUrl))
                        {
                            stlAnchor.Target = string.Empty;
                        }
                        stlAnchor.HRef = url;

                        if (string.IsNullOrEmpty(node.InnerXml))
                        {
                            stlAnchor.InnerHtml = NodeManager.GetNodeName(pageInfo.PublishmentSystemId, siblingNodeID);
                            if (wordNum > 0)
                            {
                                stlAnchor.InnerHtml = StringUtils.MaxLengthText(stlAnchor.InnerHtml, wordNum);
                            }
                        }
                        else
                        {
                            contextInfo.ChannelID = siblingNodeID;
                            var innerBuilder = new StringBuilder(node.InnerXml);
                            StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                            stlAnchor.InnerHtml = innerBuilder.ToString();
                        }
                    }
                }
                else if (type.ToLower().Equals(Type_PreviousContent.ToLower()) || type.ToLower().Equals(Type_NextContent.ToLower()))
                {
                    if (contextInfo.ContentID != 0)
                    {
                        var taxis = contextInfo.ContentInfo.Taxis;
                        var isNextContent = true;
                        if (StringUtils.EqualsIgnoreCase(type, Type_PreviousContent))
                        {
                            isNextContent = false;
                        }
                        var tableStyle = NodeManager.GetTableStyle(pageInfo.PublishmentSystemInfo, contextInfo.ChannelID);
                        var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, contextInfo.ChannelID);
                        var siblingContentID = BaiRongDataProvider.ContentDao.GetContentId(tableName, contextInfo.ChannelID, taxis, isNextContent);
                        if (siblingContentID != 0)
                        {
                            var siblingContentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, siblingContentID);
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
                                contextInfo.ContentID = siblingContentID;
                                StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                                stlAnchor.InnerHtml = innerBuilder.ToString();
                            }
                        }
                    }
                }

                if (string.IsNullOrEmpty(stlAnchor.HRef))
                {
                    if (isDisplayIfEmpty)
                    {
                        if (!string.IsNullOrEmpty(node.InnerXml))
                        {
                            var innerBuilder = new StringBuilder(node.InnerXml);
                            StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                            parsedContent = innerBuilder.ToString();
                        }
                        else
                        {
                            parsedContent = emptyText;
                        }
                    }
                    else
                    {
                        parsedContent = emptyText;
                    }
                }
                else
                {
                    parsedContent = ControlUtils.GetControlRenderHtml(stlAnchor);
                }
            }
            else
            {
                var nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelID);

                var isSuccess = false;
                var theContextInfo = contextInfo.Clone();

                if (type.ToLower().Equals(Type_PreviousChannel.ToLower()) || type.ToLower().Equals(Type_NextChannel.ToLower()))
                {
                    var taxis = nodeInfo.Taxis;
                    var isNextChannel = true;
                    if (StringUtils.EqualsIgnoreCase(type, Type_PreviousChannel))
                    {
                        isNextChannel = false;
                    }
                    var siblingNodeID = DataProvider.NodeDao.GetNodeIdByParentIdAndTaxis(nodeInfo.ParentId, taxis, isNextChannel);
                    if (siblingNodeID != 0)
                    {
                        isSuccess = true;
                        theContextInfo.ContextType = EContextType.Channel;
                        theContextInfo.ChannelID = siblingNodeID;
                    }
                }
                else if (type.ToLower().Equals(Type_PreviousContent.ToLower()) || type.ToLower().Equals(Type_NextContent.ToLower()))
                {
                    if (contextInfo.ContentID != 0)
                    {
                        var taxis = contextInfo.ContentInfo.Taxis;
                        var isNextContent = true;
                        if (StringUtils.EqualsIgnoreCase(type, Type_PreviousContent))
                        {
                            isNextContent = false;
                        }
                        var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, contextInfo.ChannelID);
                        var siblingContentID = BaiRongDataProvider.ContentDao.GetContentId(tableName, contextInfo.ChannelID, taxis, isNextContent);
                        if (siblingContentID != 0)
                        {
                            isSuccess = true;
                            theContextInfo.ContextType = EContextType.Content;
                            theContextInfo.ContentID = siblingContentID;
                            theContextInfo.ContentInfo = null;
                        }
                    }
                }

                if (isSuccess)
                {
                    parsedContent = successTemplateString;
                }
                else
                {
                    parsedContent = failureTemplateString;
                }

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
