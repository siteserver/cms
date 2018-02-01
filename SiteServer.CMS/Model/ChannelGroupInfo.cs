namespace SiteServer.CMS.Model
{
	public class ChannelGroupInfo
	{
	    public ChannelGroupInfo()
		{
            GroupName = string.Empty;
            SiteId = 0;
            Taxis = 0;
			Description = string.Empty;
		}

		public ChannelGroupInfo(string groupName, int siteId, int taxis, string description) 
		{
            GroupName = groupName;
            SiteId = siteId;
            Taxis = taxis;
			Description = description;
		}

        public int Id { get; set; }

        public string GroupName { get; set; }

	    public int SiteId { get; set; }

	    public int Taxis { get; set; }

	    public string Description { get; set; }
	}
}
