namespace SiteServer.CMS.Model
{
	public class TemplateMatchInfo
	{
	    public TemplateMatchInfo()
	    {
	        Id = 0;
            ChannelId = 0;
			SiteId = 0;
			ChannelTemplateId = 0;
			ContentTemplateId = 0;
            FilePath = string.Empty;
            ChannelFilePathRule = string.Empty;
            ContentFilePathRule = string.Empty;
		}

        public TemplateMatchInfo(int id, int channelId, int siteId, int channelTemplateId, int contentTemplateId, string filePath, string channelFilePathRule, string contentFilePathRule)
        {
            Id = id;
            ChannelId = channelId;
            SiteId = siteId;
            ChannelTemplateId = channelTemplateId;
            ContentTemplateId = contentTemplateId;
            FilePath = filePath;
            ChannelFilePathRule = channelFilePathRule;
            ContentFilePathRule = contentFilePathRule;
		}

        public int Id { get; set; }

        public int ChannelId { get; set; }

	    public int SiteId { get; set; }

	    public int ChannelTemplateId { get; set; }

	    public int ContentTemplateId { get; set; }

	    public string FilePath { get; set; }

	    public string ChannelFilePathRule { get; set; }

	    public string ContentFilePathRule { get; set; }
	}
}
