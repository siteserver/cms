namespace SiteServer.CMS.Model
{
	public class ContentGroupInfo
	{
	    public ContentGroupInfo()
		{
			ContentGroupName = string.Empty;
			PublishmentSystemId = 0;
            Taxis = 0;
			Description = string.Empty;
		}

		public ContentGroupInfo(string contentGroupName, int publishmentSystemId, int taxis, string description) 
		{
			ContentGroupName = contentGroupName;
			PublishmentSystemId = publishmentSystemId;
            Taxis = taxis;
			Description = description;
		}

		public string ContentGroupName { get; set; }

	    public int PublishmentSystemId { get; set; }

	    public int Taxis { get; set; }

	    public string Description { get; set; }
	}
}
