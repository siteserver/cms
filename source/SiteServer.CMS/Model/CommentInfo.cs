using System;
using BaiRong.Core;
using BaiRong.Core.Data;

namespace SiteServer.CMS.Model
{
    public class CommentInfo
    {
        public CommentInfo()
        {
            Id = 0;
            PublishmentSystemId = 0;
            NodeId = 0;
            ContentId = 0;
            GoodCount = 0;
            UserName = string.Empty;
            IsChecked = false;
            AddDate = DateTime.Now;
            Content = string.Empty;
        }

        public CommentInfo(int id, int publishmentSystemId, int nodeId, int contentId, int goodCount, string userName, bool isChecked, DateTime addDate, string content)
        {
            Id = id;
            PublishmentSystemId = publishmentSystemId;
            NodeId = nodeId;
            ContentId = contentId;
            GoodCount = goodCount;
            UserName = userName;
            IsChecked = isChecked;
            AddDate = addDate;
            Content = content;
        }

        public CommentInfo(object dataItem)
		{
            Id = SqlUtils.EvalInt(dataItem, "ID");
            PublishmentSystemId = SqlUtils.EvalInt(dataItem, "PublishmentSystemID");
            NodeId = SqlUtils.EvalInt(dataItem, "NodeID");
            ContentId = SqlUtils.EvalInt(dataItem, "ContentID");
            GoodCount = SqlUtils.EvalInt(dataItem, "GoodCount");
            UserName = SqlUtils.EvalString(dataItem, "UserName");
		    IsChecked = SqlUtils.EvalBool(dataItem, "IsChecked");
            AddDate = SqlUtils.EvalDateTime(dataItem, "AddDate");
            Content = SqlUtils.EvalString(dataItem, "Content");
        }

        public int Id { get; set; }

        public int PublishmentSystemId { get; set; }

        public int NodeId { get; set; }

        public int ContentId { get; set; }

        public int GoodCount { get; set; }

        public string UserName { get; set; }

        public bool IsChecked { get; set; }

        public DateTime AddDate { get; set; }

        public string Content { get; set; }
    }
}
