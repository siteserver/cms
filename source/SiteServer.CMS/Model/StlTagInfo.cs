namespace SiteServer.CMS.Model
{
	public class StlTagInfo
	{
        private string tagName;
		private int publishmentSystemID;
        private string tagDescription;
        private string tagContent;

		public StlTagInfo()
		{
            tagName = string.Empty;
			publishmentSystemID = 0;
            tagDescription = string.Empty;
            tagContent = string.Empty;
		}

        public StlTagInfo(string tagName, int publishmentSystemID, string tagDescription, string tagContent) 
		{
            this.tagName = tagName;
			this.publishmentSystemID = publishmentSystemID;
            this.tagDescription = tagDescription;
            this.tagContent = tagContent;
		}

        public string TagName
		{
            get { return tagName; }
            set { tagName = value; }
		}

		public int PublishmentSystemID
		{
			get{ return publishmentSystemID; }
			set{ publishmentSystemID = value; }
		}

        public string TagDescription
		{
            get { return tagDescription; }
            set { tagDescription = value; }
		}

        public string TagContent
		{
            get { return tagContent; }
            set { tagContent = value; }
		}
	}
}
