using System;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Model
{
    public class AdvInfo 
	{
	    public AdvInfo()
		{
            AdvId = 0;
            PublishmentSystemId = 0;
            AdAreaId=0;
            AdvName = string.Empty;
            Summary = string.Empty ;
			IsEnabled = true;
			IsDateLimited = false;
			StartDate = DateTime.Now;
			EndDate = DateTime.Now.AddMonths(1);
            LevelType = EAdvLevelType.Hold;
            Level=0;
            IsWeight=false;
            Weight=0;
            RotateType = EAdvRotateType.Equality;
            RotateInterval = 0;
            NodeIdCollectionToChannel = string.Empty;
            NodeIdCollectionToContent = string.Empty;
            FileTemplateIdCollection = string.Empty;
		}

        public AdvInfo(int advId, int publishmentSystemId, int adAreaId, string advName, string summary, bool isEnabled, bool isDateLimited, DateTime startDate, DateTime endDate, EAdvLevelType levelType, int level, bool isWeight, int weight, EAdvRotateType rotateType, int rotateInterval, string nodeIdCollectionToChannel, string nodeIdCollectionToContent, string fileTemplateIdCollection) 
		{
            AdvId = advId;
            PublishmentSystemId = publishmentSystemId;
            AdAreaId =adAreaId;
            AdvName = advName;
            Summary = summary;
            IsEnabled = isEnabled;
            IsDateLimited = isDateLimited;
            StartDate = startDate;
            EndDate = endDate;
            LevelType = levelType;
            Level= level;
            IsWeight = isWeight;
            Weight = weight;
            RotateType = rotateType;
            RotateInterval = rotateInterval;
            NodeIdCollectionToChannel =nodeIdCollectionToChannel;
            NodeIdCollectionToContent = nodeIdCollectionToContent;
            FileTemplateIdCollection = fileTemplateIdCollection;
		}
         
        public int AdvId { get; set; }

	    public int PublishmentSystemId { get; set; }

	    public int AdAreaId { get; set; }

	    public string AdvName { get; set; }

	    public string Summary { get; set; }

	    public bool IsEnabled { get; set; }

	    public bool IsDateLimited { get; set; }

	    public DateTime StartDate { get; set; }

	    public DateTime EndDate { get; set; }

	    public EAdvLevelType LevelType { get; set; }

	    public int Level { get; set; }

	    public bool IsWeight { get; set; }

	    public int Weight { get; set; }

	    public EAdvRotateType RotateType { get; set; }

	    public int RotateInterval { get; set; }

	    public string NodeIdCollectionToChannel { get; set; }

	    public string NodeIdCollectionToContent { get; set; }

	    public string FileTemplateIdCollection { get; set; }
	}
}
