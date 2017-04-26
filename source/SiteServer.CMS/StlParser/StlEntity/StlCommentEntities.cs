using System.Collections.Specialized;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlEntity
{
	public class StlCommentEntities
	{
        private StlCommentEntities()
		{
		}

        public const string EntityName = "Comment";        //评论实体

        private static string DiggGood = "DiggGood";       //支持

        public static ListDictionary AttributeList
        {
            get
            {
                var attributes = new ListDictionary();
                attributes.Add("ID", "评论ID");
                attributes.Add("AddDate", "评论时间");
                attributes.Add("UserName", "评论人");
                attributes.Add("GoodCount", "支持数目");
                attributes.Add(DiggGood, "支持");
                attributes.Add("Content", "评论正文");
                attributes.Add(StlParserUtility.ItemIndex, "评论排序");                

                return attributes;
            }
        }

        internal static string Parse(string stlEntity, PageInfo pageInfo, ContextInfo contextInfo)
        {
            var parsedContent = string.Empty;

            if (contextInfo.ItemContainer == null || contextInfo.ItemContainer.CommentItem == null) return string.Empty;

            try
            {
                var entityName = StlParserUtility.GetNameFromEntity(stlEntity);

                var type = entityName.Substring(9, entityName.Length - 10).ToLower();

                var commentID = SqlUtils.EvalInt(contextInfo.ItemContainer.CommentItem, "ID");
                var nodeID = SqlUtils.EvalInt(contextInfo.ItemContainer.CommentItem, "NodeID");
                var contentID = SqlUtils.EvalInt(contextInfo.ItemContainer.CommentItem, "ContentID");
                var goodCount = SqlUtils.EvalInt(contextInfo.ItemContainer.CommentItem, "GoodCount");
                var userName = SqlUtils.EvalString(contextInfo.ItemContainer.CommentItem, "UserName");
                var isChecked = TranslateUtils.ToBool(SqlUtils.EvalString(contextInfo.ItemContainer.CommentItem, "IsChecked"));
                var addDate = SqlUtils.EvalDateTime(contextInfo.ItemContainer.CommentItem, "AddDate");
                var content = SqlUtils.EvalString(contextInfo.ItemContainer.CommentItem, "Content");

                if (type == "id")
                {
                    parsedContent = commentID.ToString();
                }
                else if (type == "adddate")
                {
                    parsedContent = DateUtils.Format(addDate, string.Empty);
                }
                else if (type == "username")
                {
                    parsedContent = string.IsNullOrEmpty(userName) ? "匿名" : userName;
                }
                else if (type == "goodcount")
                {
                    parsedContent = goodCount.ToString();
                }
                else if (type == "content")
                {
                    parsedContent = content;
                }
                else if (StringUtils.StartsWithIgnoreCase(type, StlParserUtility.ItemIndex))
                {
                    parsedContent = StlParserUtility.ParseItemIndex(contextInfo.ItemContainer.CommentItem.ItemIndex, type, contextInfo).ToString();
                }
            }
            catch { }

            return parsedContent;
        }
	}
}
