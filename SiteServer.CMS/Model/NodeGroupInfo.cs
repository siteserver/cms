namespace SiteServer.CMS.Model
{
	public class NodeGroupInfo
	{
	    public NodeGroupInfo()
		{
			NodeGroupName = string.Empty;
			PublishmentSystemId = 0;
            Taxis = 0;
			Description = string.Empty;
		}

		public NodeGroupInfo(string nodeGroupName, int publishmentSystemId, int taxis, string description) 
		{
			NodeGroupName = nodeGroupName;
			PublishmentSystemId = publishmentSystemId;
            Taxis = taxis;
			Description = description;
		}

        public int Id { get; set; }

        public string NodeGroupName { get; set; }

	    public int PublishmentSystemId { get; set; }

	    public int Taxis { get; set; }

	    public string Description { get; set; }
	}
}
