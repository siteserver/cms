namespace SiteServer.CMS.Model
{
	public class CountInfo
	{
        public int Id { get; set; }

	    public string RelatedTableName { get; set; }

	    public string RelatedIdentity { get; set; }

	    public string CountType { get; set; }

        public int CountNum { get; set; }
    }
}
