namespace SiteServer.CMS.Model
{
	public class TagInfo
	{
	    public TagInfo()
		{
            Id = 0;
            SiteId = 0;
            ContentIdCollection = string.Empty;
            Tag = string.Empty;
            UseNum = 0;
		}

        public TagInfo(int id, int siteId, string contentIdCollection, string tag, int useNum) 
		{
            Id = id;
            SiteId = siteId;
            ContentIdCollection = contentIdCollection;
            Tag = tag;
            UseNum = useNum;
		}

        public int Id { get; set; }

	    public int SiteId { get; set; }

	    public string ContentIdCollection { get; set; }

	    public string Tag { get; set; }

	    public int UseNum { get; set; }

	    public int Level { get; set; } = 0;
	}
}
