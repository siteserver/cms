using System;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Model
{
	public class TrackingInfo
	{
	    public TrackingInfo()
		{
			TrackingId = 0;
            PublishmentSystemId = 0;
            TrackerType = ETrackerType.Page;
			LastAccessDateTime = DateTime.MinValue;
            PageUrl = string.Empty;
			PageNodeId = 0;
            PageContentId = 0;
			Referrer = string.Empty;
			IpAddress = string.Empty;
			OperatingSystem = string.Empty;
			Browser = string.Empty;
			AccessDateTime = DateTime.Now;
		}

        public TrackingInfo(int trackingId, int publishmentSystemId, ETrackerType trackerType, DateTime lastAccessDateTime, string pageUrl, int pageNodeId, int pageContentId, string referrer, string ipAddress, string operatingSystem, string browser, DateTime accessDateTime) 
		{
            TrackingId = trackingId;
            PublishmentSystemId = publishmentSystemId;
            TrackerType = trackerType;
			LastAccessDateTime = lastAccessDateTime;
            PageUrl = pageUrl;
            PageNodeId = pageNodeId;
            PageContentId = pageContentId;
			Referrer = referrer;
			IpAddress = ipAddress;
			OperatingSystem = operatingSystem;
			Browser = browser;
			AccessDateTime = accessDateTime;
		}

        public int TrackingId { get; set; }

	    public int PublishmentSystemId { get; set; }

	    public ETrackerType TrackerType { get; set; }

	    public DateTime LastAccessDateTime { get; set; }

	    public string PageUrl { get; set; }

	    public int PageNodeId { get; set; }

	    public int PageContentId { get; set; }

	    public string Referrer { get; set; }

	    public string IpAddress { get; set; }

	    public string OperatingSystem { get; set; }

	    public string Browser { get; set; }

	    public DateTime AccessDateTime { get; set; }
	}
}
