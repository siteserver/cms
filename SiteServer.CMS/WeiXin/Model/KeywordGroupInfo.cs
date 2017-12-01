namespace SiteServer.CMS.WeiXin.Model
{
	public class KeywordGroupInfo
	{
	    public KeywordGroupInfo()
		{
            GroupId = 0;
            PublishmentSystemId = 0;
            GroupName = string.Empty;
            Taxis = 0;
		}

        public KeywordGroupInfo(int groupId, int publishmentSystemId, string groupName, int taxis)
		{
            GroupId = groupId;
            PublishmentSystemId = publishmentSystemId;
            GroupName = groupName;
            Taxis = taxis;
		}

        public int GroupId { get; set; }

	    public int PublishmentSystemId { get; set; }

	    public string GroupName { get; set; }

	    public int Taxis { get; set; }
	}
}
