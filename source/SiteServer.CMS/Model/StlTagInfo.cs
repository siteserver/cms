namespace SiteServer.CMS.Model
{
	public class StlTagInfo
	{
	    public StlTagInfo()
		{
            TagName = string.Empty;
			PublishmentSystemId = 0;
            TagDescription = string.Empty;
            TagContent = string.Empty;
		}

        public StlTagInfo(string tagName, int publishmentSystemId, string tagDescription, string tagContent) 
		{
            TagName = tagName;
			PublishmentSystemId = publishmentSystemId;
            TagDescription = tagDescription;
            TagContent = tagContent;
		}

        public int Id { get; set; }

        public string TagName { get; set; }

	    public int PublishmentSystemId { get; set; }

	    public string TagDescription { get; set; }

	    public string TagContent { get; set; }
	}
}
