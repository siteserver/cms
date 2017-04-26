using System;

namespace SiteServer.CMS.Model
{
	public class LogInfo
	{
        private int id;
        private int publishmentSystemID;
        private int channelID;
        private int contentID;
        private string userName;
        private string ipAddress;
        private DateTime addDate;
        private string action;
        private string summary;

		public LogInfo()
		{
            id = 0;
            publishmentSystemID = 0;
            channelID = 0;
            contentID = 0;
            userName = string.Empty;
            ipAddress = string.Empty;
            addDate = DateTime.Now;
            action = string.Empty;
            summary = string.Empty;
		}

        public LogInfo(int id, int publishmentSystemID, int channelID, int contentID, string userName, string ipAddress, DateTime addDate, string action, string summary) 
		{
            this.id = id;
            this.publishmentSystemID = publishmentSystemID;
            this.channelID = channelID;
            this.contentID = contentID;
            this.userName = userName;
            this.ipAddress = ipAddress;
            this.addDate = addDate;
            this.action = action;
            this.summary = summary;
		}

        public int ID
		{
            get { return id; }
            set { id = value; }
		}

        public int PublishmentSystemID
        {
            get { return publishmentSystemID; }
            set { publishmentSystemID = value; }
        }

        public int ChannelID
        {
            get { return channelID; }
            set { channelID = value; }
        }

        public int ContentID
        {
            get { return contentID; }
            set { contentID = value; }
        }

        public string UserName
		{
            get { return userName; }
            set { userName = value; }
		}

        public string IPAddress
		{
            get { return ipAddress; }
            set { ipAddress = value; }
		}

        public DateTime AddDate
        {
            get { return addDate; }
            set { addDate = value; }
        }

        public string Action
		{
            get { return action; }
            set { action = value; }
		}

        public string Summary
        {
            get { return summary; }
            set { summary = value; }
        }
	}
}
