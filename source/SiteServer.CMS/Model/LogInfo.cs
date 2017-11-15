using System;

namespace SiteServer.CMS.Model
{
	public class LogInfo
	{
	    public LogInfo()
		{
            Id = 0;
            PublishmentSystemId = 0;
            ChannelId = 0;
            ContentId = 0;
            UserName = string.Empty;
            IpAddress = string.Empty;
            AddDate = DateTime.Now;
            Action = string.Empty;
            Summary = string.Empty;
		}

        public LogInfo(int id, int publishmentSystemId, int channelId, int contentId, string userName, string ipAddress, DateTime addDate, string action, string summary) 
		{
            Id = id;
            PublishmentSystemId = publishmentSystemId;
            ChannelId = channelId;
            ContentId = contentId;
            UserName = userName;
            IpAddress = ipAddress;
            AddDate = addDate;
            Action = action;
            Summary = summary;
		}

        public int Id { get; set; }

	    public int PublishmentSystemId { get; set; }

	    public int ChannelId { get; set; }

	    public int ContentId { get; set; }

	    public string UserName { get; set; }

	    public string IpAddress { get; set; }

	    public DateTime AddDate { get; set; }

	    public string Action { get; set; }

	    public string Summary { get; set; }
	}
}
