using System.Collections.Generic;
using BaiRong.Core;
using SiteServer.CMS.StlControls;
using SiteServer.CMS.StlParser.Model;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "评论提交及显示", Description = "通过 stl:commentInput 标签在模板中实现评论提交及显示功能")]
    public class StlCommentInput
    {
        private StlCommentInput() { }
        public const string ElementName = "stl:commentInput";

        public const string AttributePageNum = "pageNum";
        public const string AttributeIsAnonymous = "isAnonymous";

        public static SortedList<string, string> AttributeList => new SortedList<string, string>
        {
            {AttributePageNum, "每页显示的评论数目"},
            {AttributeIsAnonymous, "是否允许匿名评论"}
        };

        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
        {
            var pageNum = 10;
            var isAnonymous = true;

            // 实体标签的话不解析
            if(contextInfo.IsCurlyBrace)
            {
                return string.Empty;
            }

            foreach (var name in contextInfo.Attributes.Keys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, AttributePageNum))
                {
                    pageNum = TranslateUtils.ToInt(value, 10);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeIsAnonymous))
                {
                    isAnonymous = TranslateUtils.ToBool(value);
                }
            }

            return ParseImpl(pageInfo, pageNum, isAnonymous);
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
