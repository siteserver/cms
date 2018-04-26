using System;

namespace SiteServer.CMS.Model
{
	public class SpecialInfo
	{
	    public int Id { get; set; }
	    public int SiteId { get; set; }
	    public string Title { get; set; }
	    public string Url { get; set; }
	    public DateTime AddDate { get; set; }
    }
}
