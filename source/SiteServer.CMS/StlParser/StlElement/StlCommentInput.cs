using System;
using System.Collections.Specialized;
using System.Xml;
using BaiRong.Core;
using SiteServer.CMS.StlControls;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    public class StlCommentInput
    {
        private StlCommentInput() { }
        public const string ElementName = "stl:commentinput";

        public const string AttributePageNum = "pagenum";					            //每页显示的评论数目
        public const string AttributeIsAnonymous = "isanonymous";                       //是否允许匿名评论

        public static ListDictionary AttributeList => new ListDictionary
        {
            {AttributePageNum, "每页显示的评论数目"},
            {AttributeIsAnonymous, "是否允许匿名评论"}
        };

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
        {
            string parsedContent;
            try
            {
                var ie = node.Attributes?.GetEnumerator();

                var pageNum = 10;
                var isAnonymous = true;

                if (ie != null)
                {
                    while (ie.MoveNext())
                    {
                        var attr = (XmlAttribute)ie.Current;
                        var attributeName = attr.Name.ToLower();
                        if (attributeName.Equals(AttributePageNum))
                        {
                            pageNum = TranslateUtils.ToInt(attr.Value, 10);
                        }
                        else if (attributeName.Equals(AttributeIsAnonymous))
                        {
                            isAnonymous = TranslateUtils.ToBool(attr.Value);
                        }
                    }
                }

                parsedContent = ParseImpl(pageInfo, pageNum, isAnonymous);
            }
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

            return parsedContent;
        }

        private static string ParseImpl(PageInfo pageInfo, int pageNum, bool isAnonymous)
        {
            pageInfo.AddPageScriptsIfNotExists(PageInfo.Components.Vue);
            pageInfo.AddPageScriptsIfNotExists(PageInfo.Components.JsCookie);
            pageInfo.AddPageScriptsIfNotExists(PageInfo.Components.StlClient);

            var commentInput = new CommentInput
            {
                ApiUrl = pageInfo.ApiUrl,
                IsAnonymous = isAnonymous,
                PageNum = pageNum,
                ApiActionsAddUrl =
                    Controllers.Comments.ActionsAdd.GetUrl(pageInfo.ApiUrl, pageInfo.PublishmentSystemId, pageInfo.PageNodeId, pageInfo.PageContentId),
                ApiActionsDeleteUrl =
                    Controllers.Comments.ActionsDelete.GetUrl(pageInfo.ApiUrl, pageInfo.PublishmentSystemId, pageInfo.PageNodeId, pageInfo.PageContentId),
                ApiActionsGoodUrl =
                    Controllers.Comments.ActionsGood.GetUrl(pageInfo.ApiUrl, pageInfo.PublishmentSystemId, pageInfo.PageNodeId, pageInfo.PageContentId),
                ApiGetUrl =
                    Controllers.Comments.Get.GetUrl(pageInfo.ApiUrl, pageInfo.PublishmentSystemId, pageInfo.PageNodeId, pageInfo.PageContentId),
                ApiActionsLogoutUrl = Controllers.Users.ActionsLogout.GetUrl(pageInfo.ApiUrl),
                HomeUrl = pageInfo.HomeUrl,
                IsDelete = false
            };

            return ControlUtils.GetControlRenderHtml(commentInput);
        }
    }
}
