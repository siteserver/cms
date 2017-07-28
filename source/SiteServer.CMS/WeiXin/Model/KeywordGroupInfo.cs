namespace SiteServer.CMS.WeiXin.Model
{
	public class KeywordGroupInfo
	{
        private int groupID;
        private int publishmentSystemID;
        private string groupName;
        private int taxis;

		public KeywordGroupInfo()
		{
            groupID = 0;
            publishmentSystemID = 0;
            groupName = string.Empty;
            taxis = 0;
		}

        public KeywordGroupInfo(int groupID, int publishmentSystemID, string groupName, int taxis)
		{
            this.groupID = groupID;
            this.publishmentSystemID = publishmentSystemID;
            this.groupName = groupName;
            this.taxis = taxis;
		}

        public int GroupID
        {
            get { return groupID; }
            set { groupID = value; }
        }

        public int PublishmentSystemID
        {
            get { return publishmentSystemID; }
            set { publishmentSystemID = value; }
        }

        public string GroupName
        {
            get { return groupName; }
            set { groupName = value; }
        }

        public int Taxis
        {
            get { return taxis; }
            set { taxis = value; }
        }
	}
}
