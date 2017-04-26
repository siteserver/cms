namespace SiteServer.CMS.Model
{
	public class VoteOptionInfo
	{
        private int optionID;
        private int publishmentSystemID;
        private int nodeID;
        private int contentID;
        private string title;
        private string imageUrl;
		private string navigationUrl;
		private int voteNum;

		public VoteOptionInfo()
		{
            optionID = 0;
            publishmentSystemID = 0;
            nodeID = 0;
            contentID = 0;
            title = string.Empty;
            imageUrl = string.Empty;
			navigationUrl = string.Empty;
			voteNum = 0;
		}

        public VoteOptionInfo(int optionID, int publishmentSystemID, int nodeID, int contentID, string title, string imageUrl, string navigationUrl, int voteNum) 
		{
            this.optionID = optionID;
            this.publishmentSystemID = publishmentSystemID;
            this.nodeID = nodeID;
            this.contentID = contentID;
            this.title = title;
            this.imageUrl = imageUrl;
			this.navigationUrl = navigationUrl;
			this.voteNum = voteNum;
		}

        public int OptionID
		{
            get { return optionID; }
            set { optionID = value; }
		}

        public int PublishmentSystemID
		{
            get { return publishmentSystemID; }
            set { publishmentSystemID = value; }
		}

        public int NodeID
        {
            get { return nodeID; }
            set { nodeID = value; }
        }

        public int ContentID
        {
            get { return contentID; }
            set { contentID = value; }
        }

        public string Title
		{
			get{ return title; }
            set { title = value; }
		}

		public string ImageUrl
		{
			get{ return imageUrl; }
			set{ imageUrl = value; }
		}

		public string NavigationUrl
		{
			get{ return navigationUrl; }
			set{ navigationUrl = value; }
		}

		public int VoteNum
		{
			get{ return voteNum; }
			set{ voteNum = value; }
		}
	}
}
