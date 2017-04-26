using System;

namespace BaiRong.Core.Model
{
	public class ContentCheckInfo
	{
	    public ContentCheckInfo()
		{
            CheckId = 0;
            TableName = string.Empty;
			PublishmentSystemId = 0;
            NodeId = 0;
            ContentId = 0;
            IsAdmin = false;
            UserName = string.Empty;
            IsChecked = false;
            CheckedLevel = 0;
            CheckDate = DateTime.Now;
            Reasons = string.Empty;
		}

        public ContentCheckInfo(int checkId, string tableName, int publishmentSystemId, int nodeId, int contentId, bool isAdmin, string userName, bool isChecked, int checkedLevel, DateTime checkDate, string reasons) 
		{
            CheckId = checkId;
            TableName = tableName;
            PublishmentSystemId = publishmentSystemId;
            NodeId = nodeId;
            ContentId = contentId;
            IsAdmin = isAdmin;
            UserName = userName;
            IsChecked = isChecked;
            CheckedLevel = checkedLevel;
            CheckDate = checkDate;
            Reasons = reasons;
		}

        public int CheckId { get; set; }

	    public string TableName { get; set; }

	    public int PublishmentSystemId { get; set; }

	    public int NodeId { get; set; }

	    public int ContentId { get; set; }

	    public bool IsAdmin { get; set; }

	    public string UserName { get; set; }

	    public bool IsChecked { get; set; }

	    public int CheckedLevel { get; set; }

	    public DateTime CheckDate { get; set; }

	    public string Reasons { get; set; }
	}
}
