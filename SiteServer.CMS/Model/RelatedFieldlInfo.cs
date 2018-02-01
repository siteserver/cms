namespace SiteServer.CMS.Model
{
	public class RelatedFieldInfo
	{
	    public RelatedFieldInfo()
		{
            Id = 0;
            Title = string.Empty;
			SiteId = 0;
            TotalLevel = 0;
            Prefixes = string.Empty;
            Suffixes = string.Empty;
		}

        public RelatedFieldInfo(int id, string title, int siteId, int totalLevel, string prefixes, string suffixes)
		{
            Id = id;
            Title = title;
            SiteId = siteId;
            TotalLevel = totalLevel;
            Prefixes = prefixes;
            Suffixes = suffixes;
		}

        public int Id { get; set; }

	    public string Title { get; set; }

	    public int SiteId { get; set; }

	    public int TotalLevel { get; set; }

	    public string Prefixes { get; set; }

	    public string Suffixes { get; set; }
	}
}
