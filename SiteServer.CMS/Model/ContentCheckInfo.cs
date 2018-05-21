using System;

namespace SiteServer.CMS.Model
{
	public class ContentCheckInfo
	{
	    public ContentCheckInfo(int id, string tableName, int siteId, int channelId, int contentId, string userName, bool isChecked, int checkedLevel, DateTime checkDate, string reasons) 
		{
            Id = id;
            TableName = tableName;
            SiteId = siteId;
            ChannelId = channelId;
            ContentId = contentId;
            UserName = userName;
            IsChecked = isChecked;
            CheckedLevel = checkedLevel;
            CheckDate = checkDate;
            Reasons = reasons;
		}

        public int Id { get; }

        public string TableName { get; }

        public int SiteId { get; }

        public int ChannelId { get; }

        public int ContentId { get; }

        public string UserName { get; }

        public bool IsChecked { get; }

        public int CheckedLevel { get; }

        public DateTime CheckDate { get; }

        public string Reasons { get; }
    }
}
