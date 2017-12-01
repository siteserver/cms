namespace SiteServer.CMS.Model
{
	public class InnerLinkInfo
	{
	    public InnerLinkInfo()
		{
            InnerLinkName = string.Empty;
			PublishmentSystemId = 0;
            LinkUrl = string.Empty;
		}

        public InnerLinkInfo(string innerLinkName, int publishmentSystemId, string linkUrl) 
		{
            InnerLinkName = innerLinkName;
			PublishmentSystemId = publishmentSystemId;
            LinkUrl = linkUrl;
		}

        public int Id { get; set; }

        public string InnerLinkName { get; set; }

	    public int PublishmentSystemId { get; set; }

	    public string LinkUrl { get; set; }

	    public string InnerString { get; set; }
	}
}
