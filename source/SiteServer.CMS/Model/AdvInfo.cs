using System;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Model
{
    public class AdvInfo 
	{
        private int advID;
        private int publishmentSystemID;
		private int adAreaID;
        private string advName;
	    private string summary;
        private bool isEnabled;
        private bool isDateLimited;
		private DateTime startDate;
		private DateTime endDate;
        private EAdvLevelType levelType; 
        private int level;
        private bool isWeight;
        private int weight;
        private EAdvRotateType rotateType;
        private int rotateInterval;
        private string nodeIDCollectionToChannel;
        private string nodeIDCollectionToContent;
        private string fileTemplateIDCollection;

		public AdvInfo()
		{
            advID = 0;
            publishmentSystemID = 0;
            adAreaID=0;
            advName = string.Empty;
            summary = string.Empty ;
			isEnabled = true;
			isDateLimited = false;
			startDate = DateTime.Now;
			endDate = DateTime.Now.AddMonths(1);
            levelType = EAdvLevelType.Hold;
            level=0;
            isWeight=false;
            weight=0;
            rotateType = EAdvRotateType.Equality;
            rotateInterval = 0;
            nodeIDCollectionToChannel = string.Empty;
            nodeIDCollectionToContent = string.Empty;
            fileTemplateIDCollection = string.Empty;
		}

        public AdvInfo(int advID, int publishmentSystemID, int adAreaID, string advName, string summary, bool isEnabled, bool isDateLimited, DateTime startDate, DateTime endDate, EAdvLevelType levelType, int level, bool isWeight, int weight, EAdvRotateType rotateType, int rotateInterval, string nodeIDCollectionToChannel, string nodeIDCollectionToContent, string fileTemplateIDCollection) 
		{
            this.advID = advID;
            this.publishmentSystemID = publishmentSystemID;
            this.adAreaID =adAreaID;
            this.advName = advName;
            this.summary = summary;
            this.isEnabled = isEnabled;
            this.isDateLimited = isDateLimited;
            this.startDate = startDate;
            this.endDate = endDate;
            this.levelType = levelType;
            this.level= level;
            this.isWeight = isWeight;
            this.weight = weight;
            this.rotateType = rotateType;
            this.rotateInterval = rotateInterval;
            this.nodeIDCollectionToChannel =nodeIDCollectionToChannel;
            this.nodeIDCollectionToContent = nodeIDCollectionToContent;
            this.fileTemplateIDCollection = fileTemplateIDCollection;
		}
         
        public int AdvID
		{
            get { return advID; }
            set { advID = value; }
		}

        public int PublishmentSystemID
        {
            get { return publishmentSystemID; }
            set { publishmentSystemID = value; }
        }

        public int AdAreaID
        {
            get { return adAreaID; }
            set { adAreaID = value; }
        }

        public string AdvName
        {
            get { return advName; }
            set { advName = value; }
        }
         
        public string Summary
        {
            get { return summary; }
            set { summary = value; }
        }
          
        public bool IsEnabled
        {
            get { return isEnabled; }
            set { isEnabled = value; }
        }

        public bool IsDateLimited
		{
			get{ return isDateLimited; }
			set{ isDateLimited = value; }
		}

		public DateTime StartDate
		{
			get{ return startDate; }
			set{ startDate = value; }
		}

		public DateTime EndDate
		{
			get{ return endDate; }
			set{ endDate = value; }
		}

        public EAdvLevelType LevelType
        {
            get { return levelType; }
            set { levelType = value; }
        }

        public int Level 
        {
            get { return level; }
            set { level = value; }
        }

        public bool IsWeight
        {
            get { return isWeight; }
            set { isWeight = value; }
        }

        public int Weight
        {
            get { return weight; }
            set { weight = value; }
        }

        public EAdvRotateType RotateType
        {
            get { return rotateType; }
            set { rotateType = value; }
        }

        public int RotateInterval
        {
            get { return rotateInterval; }
            set { rotateInterval = value; }
            
        }

        public string NodeIDCollectionToChannel
        {
            get { return nodeIDCollectionToChannel; }
            set { nodeIDCollectionToChannel = value; }
        }

        public string NodeIDCollectionToContent
        {
            get { return nodeIDCollectionToContent; }
            set { nodeIDCollectionToContent = value; }
        }

        public string FileTemplateIDCollection
        {
            get { return fileTemplateIDCollection ; }
            set { fileTemplateIDCollection = value; }
        }
	}
}
