namespace SiteServer.CMS.Model
{
	public class ContentTagInfo
	{
	    public ContentTagInfo()
	    {
	        Id = 0;
		    TagName = string.Empty;
			SiteId = 0;
		    UseNum = 0;
		}

		public ContentTagInfo(int id, string tagName, int siteId, int useNum)
		{
		    Id = id;
			TagName = tagName;
            SiteId = siteId;
		    UseNum = useNum;
		}

        public int Id { get; set; }

        public string TagName { get; set; }

	    public int SiteId { get; set; }

	    public int UseNum { get; set; }
	}
}
