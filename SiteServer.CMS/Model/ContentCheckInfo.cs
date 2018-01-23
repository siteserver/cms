using System;

namespace SiteServer.CMS.Model
{
	public class ContentCheckInfo
	{
	    public ContentCheckInfo()
		{
            Id = 0;
            TableName = string.Empty;
			SiteId = 0;
            ChannelId = 0;
            ContentId = 0;
            IsAdmin = false;
            UserName = string.Empty;
            IsChecked = false;
            CheckedLevel = 0;
            CheckDate = DateTime.Now;
            Reasons = string.Empty;
		}

        public ContentCheckInfo(int id, string tableName, int siteId, int channelId, int contentId, bool isAdmin, string userName, bool isChecked, int checkedLevel, DateTime checkDate, string reasons) 
		{
            Id = id;
            TableName = tableName;
            SiteId = siteId;
            ChannelId = channelId;
            ContentId = contentId;
            IsAdmin = isAdmin;
            UserName = userName;
            IsChecked = isChecked;
            CheckedLevel = checkedLevel;
            CheckDate = checkDate;
            Reasons = reasons;
		}

        public int Id { get; set; }

	    public string TableName { get; set; }

	    public int SiteId { get; set; }

	    public int ChannelId { get; set; }

	    public int ContentId { get; set; }

	    public bool IsAdmin { get; set; }

	    public string UserName { get; set; }

	    public bool IsChecked { get; set; }

	    public int CheckedLevel { get; set; }

	    public DateTime CheckDate { get; set; }

	    public string Reasons { get; set; }
	}
}
