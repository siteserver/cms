namespace SiteServer.CMS.Model
{
	public class RelatedFieldInfo
	{
        private int relatedFieldID;
        private string relatedFieldName;
		private int publishmentSystemID;
        private int totalLevel;
        private string prefixes;
        private string suffixes;

		public RelatedFieldInfo()
		{
            relatedFieldID = 0;
            relatedFieldName = string.Empty;
			publishmentSystemID = 0;
            totalLevel = 0;
            prefixes = string.Empty;
            suffixes = string.Empty;
		}

        public RelatedFieldInfo(int relatedFieldID, string relatedFieldName, int publishmentSystemID, int totalLevel, string prefixes, string suffixes)
		{
            this.relatedFieldID = relatedFieldID;
            this.relatedFieldName = relatedFieldName;
            this.publishmentSystemID = publishmentSystemID;
            this.totalLevel = totalLevel;
            this.prefixes = prefixes;
            this.suffixes = suffixes;
		}

        public int RelatedFieldID
		{
            get { return relatedFieldID; }
            set { relatedFieldID = value; }
		}

        public string RelatedFieldName
        {
            get { return relatedFieldName; }
            set { relatedFieldName = value; }
        }

		public int PublishmentSystemID
		{
			get{ return publishmentSystemID; }
			set{ publishmentSystemID = value; }
		}

        public int TotalLevel
        {
            get { return totalLevel; }
            set { totalLevel = value; }
        }

        public string Prefixes
        {
            get { return prefixes; }
            set { prefixes = value; }
        }

        public string Suffixes
        {
            get { return suffixes; }
            set { suffixes = value; }
        }
	}
}
