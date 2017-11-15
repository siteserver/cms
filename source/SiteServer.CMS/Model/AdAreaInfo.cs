using System;

namespace SiteServer.CMS.Model
{
	public class AdAreaInfo
	{
	    public AdAreaInfo()
		{
            AdAreaId = 0;
			PublishmentSystemId = 0;
            AdAreaName = string.Empty;
            Width = 0;
            Height = 0;
            Summary = string.Empty;
            IsEnabled = true;
            AddDate = DateTime.Now;
			 
		}

        public AdAreaInfo(int adAreaId, int publishmentSystemId, string adAreaName, int width, int height, string summary, bool isEnabled, DateTime addDate) 
		{
            AdAreaId = adAreaId;
            PublishmentSystemId = publishmentSystemId;
            AdAreaName = adAreaName;
            Width = width;
            Height = height;
            Summary = summary;
            IsEnabled = isEnabled;
            AddDate = addDate;
		}

        public int AdAreaId { get; set; }

	    public int PublishmentSystemId { get; set; }

	    public string AdAreaName { get; set; }

	    public int Width { get; set; }

	    public int Height { get; set; }

	    public string Summary { get; set; }

	    public bool IsEnabled { get; set; }

	    public DateTime AddDate { get; set; }
	}
}
