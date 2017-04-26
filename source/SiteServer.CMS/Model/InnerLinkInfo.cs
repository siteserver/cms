namespace SiteServer.CMS.Model
{
	public class InnerLinkInfo
	{
		private string innerLinkName;
		private int publishmentSystemID;
		private string linkUrl;

		public InnerLinkInfo()
		{
            innerLinkName = string.Empty;
			publishmentSystemID = 0;
            linkUrl = string.Empty;
		}

        public InnerLinkInfo(string innerLinkName, int publishmentSystemID, string linkUrl) 
		{
            this.innerLinkName = innerLinkName;
			this.publishmentSystemID = publishmentSystemID;
            this.linkUrl = linkUrl;
		}

        public string InnerLinkName
		{
            get { return innerLinkName; }
            set { innerLinkName = value; }
		}

		public int PublishmentSystemID
		{
            get { return publishmentSystemID; }
            set { publishmentSystemID = value; }
		}

        public string LinkUrl
		{
            get { return linkUrl; }
            set { linkUrl = value; }
		}

        private string innerString;
        public string InnerString
        {
            get { return innerString; }
            set { innerString = value; }
        }
	}
}
