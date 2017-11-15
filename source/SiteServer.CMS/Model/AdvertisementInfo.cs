using System;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Model
{
	public class AdvertisementInfo
	{
	    public AdvertisementInfo()
		{
			AdvertisementName = string.Empty;
			PublishmentSystemId = 0;
			AdvertisementType = EAdvertisementType.FloatImage;
			IsDateLimited = false;
			StartDate = DateTime.Now;
			EndDate = DateTime.Now;
			AddDate = DateTime.Now;
			NodeIdCollectionToChannel = string.Empty;
            NodeIdCollectionToContent = string.Empty;
            FileTemplateIdCollection = string.Empty;
            Settings = string.Empty;
		}

        public AdvertisementInfo(string advertisementName, int publishmentSystemId, EAdvertisementType advertisementType, bool isDateLimited, DateTime startDate, DateTime endDate, DateTime addDate, string nodeIdCollectionToChannel, string nodeIdCollectionToContent, string fileTemplateIdCollection, string settings) 
		{
			AdvertisementName = advertisementName;
			PublishmentSystemId = publishmentSystemId;
			AdvertisementType = advertisementType;
			IsDateLimited = isDateLimited;
			StartDate = startDate;
			EndDate = endDate;
			AddDate = addDate;
            NodeIdCollectionToChannel = nodeIdCollectionToChannel;
            NodeIdCollectionToContent = nodeIdCollectionToContent;
            FileTemplateIdCollection = fileTemplateIdCollection;
            Settings = settings;
		}

        public int Id { get; set; }

        public string AdvertisementName { get; set; }

	    public int PublishmentSystemId { get; set; }

	    public EAdvertisementType AdvertisementType { get; set; }

	    public bool IsDateLimited { get; set; }

	    public DateTime StartDate { get; set; }

	    public DateTime EndDate { get; set; }

	    public DateTime AddDate { get; set; }

	    public string NodeIdCollectionToChannel { get; set; }

	    public string NodeIdCollectionToContent { get; set; }

	    public string FileTemplateIdCollection { get; set; }

	    public string Settings { get; set; }
	}
}
