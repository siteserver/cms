using System.Collections.Generic;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlEntity
{
    [Stl(Usage = "评论实体", Description = "通过 {comment.} 实体在模板中显示评论值")]
    public class StlCommentEntities
	{
        private StlCommentEntities()
		{
		}

        public const string EntityName = "comment";

        private const string Id = "Id";
        private const string AddDate = "AddDate";
        private const string UserName = "UserName";
        private const string DisplayName = "DisplayName";
        private const string GoodCount = "GoodCount";
        private const string DiggGood = "DiggGood";
        private const string Content = "Content";
        private const string ItemIndex = "ItemIndex";

        public static SortedList<string, string> AttributeList => new SortedList<string, string>
	    {
	        {Id, "评论Id"},
	        {AddDate, "评论时间"},
	        {UserName, "评论人用户名"},
            {DisplayName, "评论人姓名"},
            {GoodCount, "支持数目"},
	        {DiggGood, "支持"},
	        {Content, "评论正文"},
	        {ItemIndex, "评论排序"}
	    };

        internal static string Parse(string stlEntity, PageInfo pageInfo, ContextInfo contextInfo)
        {
            var parsedContent = string.Empty;

            if (contextInfo.ItemContainer == null || contextInfo.ItemContainer.CommentItem == null) return string.Empty;

            try
            {
                var entityName = StlParserUtility.GetNameFromEntity(stlEntity);

                var type = entityName.Substring(9, entityName.Length - 10);

                var commentId = SqlUtils.EvalInt(contextInfo.ItemContainer.CommentItem, "ID");
                var goodCount = SqlUtils.EvalInt(contextInfo.ItemContainer.CommentItem, "GoodCount");
                var userName = SqlUtils.EvalString(contextInfo.ItemContainer.CommentItem, "UserName");
                var addDate = SqlUtils.EvalDateTime(contextInfo.ItemContainer.CommentItem, "AddDate");
                var content = SqlUtils.EvalString(contextInfo.ItemContainer.CommentItem, "Content");

                if (StringUtils.EqualsIgnoreCase(type, Id))
                {
                    parsedContent = commentId.ToString();
                }
                else if (StringUtils.EqualsIgnoreCase(type, AddDate))
                {
                    parsedContent = DateUtils.Format(addDate, string.Empty);
                }
                else if (StringUtils.EqualsIgnoreCase(type, UserName))
                {
                    parsedContent = string.IsNullOrEmpty(userName) ? "匿名" : userName;
                }
                else if (StringUtils.EqualsIgnoreCase(type, DisplayName))
                {
                    parsedContent = string.IsNullOrEmpty(userName) ? "匿名" : BaiRongDataProvider.UserDao.GetDisplayName(userName);
                }
                else if (StringUtils.EqualsIgnoreCase(type, GoodCount))
                {
                    parsedContent = goodCount.ToString();
                }
                else if (StringUtils.EqualsIgnoreCase(type, Content))
                {
                    parsedContent = content;
                }
                else if (StringUtils.StartsWithIgnoreCase(type, ItemIndex))
                {
                    parsedContent = StlParserUtility.ParseItemIndex(contextInfo.ItemContainer.CommentItem.ItemIndex, type, contextInfo).ToString();
                }
            }
            catch
            {
                // ignored
            }

            return parsedContent;
        }
	}
}
