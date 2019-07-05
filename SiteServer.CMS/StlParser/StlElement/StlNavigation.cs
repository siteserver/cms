using System.Collections.Generic;
using System.Text;
using System.Web.UI.HtmlControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.DataCache.Stl;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlElement(Title = "显示导航", Description = "通过 stl:navigation 标签在模板中显示链接导航")]
    public class StlNavigation
    {
        private StlNavigation() { }
        public const string ElementName = "stl:navigation";

        [StlAttribute(Title = "类型")]
        private const string Type = nameof(Type);
        
        [StlAttribute(Title = "当无内容时显示的信息")]
        private const string EmptyText = nameof(EmptyText);
        
        [StlAttribute(Title = "导航提示信息")]
        private const string TipText = nameof(TipText);
        
        [StlAttribute(Title = "显示字数")]
        private const string WordNum = nameof(WordNum);
        
        [StlAttribute(Title = "是否开启键盘，↑↓←→键分别为上下左右")]
        private const string IsKeyboard = nameof(IsKeyboard);

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

        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
        {
            var stlAnchor = new HtmlAnchor();
            var type = TypeNextContent;
            var emptyText = string.Empty;
            var tipText = string.Empty;
            var wordNum = 0;
            var isKeyboard = false;

            foreach (var name in contextInfo.Attributes.AllKeys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, Type))
                {
                    type = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, EmptyText))
                {
                    emptyText = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, TipText))
                {
                    tipText = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, WordNum))
                {
                    wordNum = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsKeyboard))
                {
                    isKeyboard = TranslateUtils.ToBool(value);
                }
                else
                {
                    ControlUtils.AddAttributeIfNotExists(stlAnchor, name, value);
                }
            }

            return ParseImpl(pageInfo, contextInfo, stlAnchor, type, emptyText, tipText, wordNum, isKeyboard);
        }

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, HtmlAnchor stlAnchor, string type, string emptyText, string tipText, int wordNum, bool isKeyboard)
        {
            string parsedContent;

            string successTemplateString;
            string failureTemplateString;
            StlParserUtility.GetYesNo(contextInfo.InnerHtml, out successTemplateString, out failureTemplateString);

            if (string.IsNullOrEmpty(successTemplateString))
            {
                var nodeInfo = ChannelManager.GetChannelInfo(pageInfo.SiteId, contextInfo.ChannelId);

                if (type.ToLower().Equals(TypePreviousChannel.ToLower()) || type.ToLower().Equals(TypeNextChannel.ToLower()))
                {
                    var taxis = nodeInfo.Taxis;
                    var isNextChannel = !StringUtils.EqualsIgnoreCase(type, TypePreviousChannel);
                    //var siblingChannelId = DataProvider.ChannelDao.GetIdByParentIdAndTaxis(nodeInfo.ParentId, taxis, isNextChannel);
                    var siblingChannelId = StlChannelCache.GetIdByParentIdAndTaxis(nodeInfo.ParentId, taxis, isNextChannel);
                    if (siblingChannelId != 0)
                    {
                        var siblingNodeInfo = ChannelManager.GetChannelInfo(pageInfo.SiteId, siblingChannelId);
                        var url = PageUtility.GetChannelUrl(pageInfo.SiteInfo, siblingNodeInfo, pageInfo.IsLocal);
                        if (url.Equals(PageUtils.UnclickedUrl))
                        {
                            stlAnchor.Target = string.Empty;
                        }
                        stlAnchor.HRef = url;

                        if (string.IsNullOrEmpty(contextInfo.InnerHtml))
                        {
                            stlAnchor.InnerHtml = ChannelManager.GetChannelName(pageInfo.SiteId, siblingChannelId);
                            if (wordNum > 0)
                            {
                                stlAnchor.InnerHtml = StringUtils.MaxLengthText(stlAnchor.InnerHtml, wordNum);
                            }
                        }
                        else
                        {
                            contextInfo.ChannelId = siblingChannelId;
                            var innerBuilder = new StringBuilder(contextInfo.InnerHtml);
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
                        var tableName = ChannelManager.GetTableName(pageInfo.SiteInfo, contextInfo.ChannelId);
                        //var siblingContentId = DataProvider.ContentDao.GetContentId(tableName, contextInfo.ChannelId, taxis, isNextContent);
                        var siblingContentId = StlContentCache.GetContentId(tableName, contextInfo.ChannelId, taxis, isNextContent);
                        if (siblingContentId != 0)
                        {
                            //var siblingContentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, siblingContentId);
                            var siblingContentInfo = ContentManager.GetContentInfo(pageInfo.SiteInfo, contextInfo.ChannelId, siblingContentId);
                            var url = PageUtility.GetContentUrl(pageInfo.SiteInfo, siblingContentInfo, pageInfo.IsLocal);
                            if (url.Equals(PageUtils.UnclickedUrl))
                            {
                                stlAnchor.Target = string.Empty;
                            }
                            stlAnchor.HRef = url;

                            if (isKeyboard)
                            {
                                var keyCode = isNextContent ? 39 : 37;
                                var scriptContent = new StringBuilder();
                                pageInfo.AddPageBodyCodeIfNotExists(PageInfo.Const.Jquery);
                                scriptContent.Append($@"<script language=""javascript"" type=""text/javascript""> 
      $(document).keydown(function(event){{
        if(event.keyCode=={keyCode}){{location = '{url}';}}
      }});
</script> 
");
                                var nextOrPrevious = isNextContent ? "nextContent" : "previousContent";

                                pageInfo.BodyCodes[nextOrPrevious] = scriptContent.ToString();
                            }

                            if (string.IsNullOrEmpty(contextInfo.InnerHtml))
                            {
                                stlAnchor.InnerHtml = siblingContentInfo.Title;
                                if (wordNum > 0)
                                {
                                    stlAnchor.InnerHtml = StringUtils.MaxLengthText(stlAnchor.InnerHtml, wordNum);
                                }
                            }
                            else
                            {
                                var innerBuilder = new StringBuilder(contextInfo.InnerHtml);
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
                var nodeInfo = ChannelManager.GetChannelInfo(pageInfo.SiteId, contextInfo.ChannelId);

                var isSuccess = false;
                var theContextInfo = contextInfo.Clone();

                if (type.ToLower().Equals(TypePreviousChannel.ToLower()) || type.ToLower().Equals(TypeNextChannel.ToLower()))
                {
                    var taxis = nodeInfo.Taxis;
                    var isNextChannel = !StringUtils.EqualsIgnoreCase(type, TypePreviousChannel);
                    //var siblingChannelId = DataProvider.ChannelDao.GetIdByParentIdAndTaxis(nodeInfo.ParentId, taxis, isNextChannel);
                    var siblingChannelId = StlChannelCache.GetIdByParentIdAndTaxis(nodeInfo.ParentId, taxis, isNextChannel);
                    if (siblingChannelId != 0)
                    {
                        isSuccess = true;
                        theContextInfo.ContextType = EContextType.Channel;
                        theContextInfo.ChannelId = siblingChannelId;
                    }
                }
                else if (type.ToLower().Equals(TypePreviousContent.ToLower()) || type.ToLower().Equals(TypeNextContent.ToLower()))
                {
                    if (contextInfo.ContentId != 0)
                    {
                        var taxis = contextInfo.ContentInfo.Taxis;
                        var isNextContent = !StringUtils.EqualsIgnoreCase(type, TypePreviousContent);
                        var tableName = ChannelManager.GetTableName(pageInfo.SiteInfo, contextInfo.ChannelId);
                        //var siblingContentId = DataProvider.ContentDao.GetContentId(tableName, contextInfo.ChannelId, taxis, isNextContent);
                        var siblingContentId = StlContentCache.GetContentId(tableName, contextInfo.ChannelId, taxis, isNextContent);
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
