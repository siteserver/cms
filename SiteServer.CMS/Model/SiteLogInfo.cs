using System;

namespace SiteServer.CMS.Model
{
	public class SiteLogInfo
    {
	    public SiteLogInfo()
		{
            Id = 0;
            SiteId = 0;
            ChannelId = 0;
            ContentId = 0;
            UserName = string.Empty;
            IpAddress = string.Empty;
            AddDate = DateTime.Now;
            Action = string.Empty;
            Summary = string.Empty;
		}

        public SiteLogInfo(int id, int site, int channelId, int contentId, string userName, string ipAddress, DateTime addDate, string action, string summary) 
		{
            Id = id;
            SiteId = site;
            ChannelId = channelId;
            ContentId = contentId;
            UserName = userName;
            IpAddress = ipAddress;
            AddDate = addDate;
            Action = action;
            Summary = summary;
		}

        public int Id { get; set; }

	    public int SiteId { get; set; }

	    public int ChannelId { get; set; }

	    public int ContentId { get; set; }

	    public string UserName { get; set; }

	    public string IpAddress { get; set; }

	    public DateTime AddDate { get; set; }

	    public string Action { get; set; }

	    public string Summary { get; set; }
	}
}
