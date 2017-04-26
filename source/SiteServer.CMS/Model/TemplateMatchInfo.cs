namespace SiteServer.CMS.Model
{
	public class TemplateMatchInfo
	{
	    public TemplateMatchInfo()
		{
            NodeId = 0;
			PublishmentSystemId = 0;
			ChannelTemplateId = 0;
			ContentTemplateId = 0;
            FilePath = string.Empty;
            ChannelFilePathRule = string.Empty;
            ContentFilePathRule = string.Empty;
		}

        public TemplateMatchInfo(int nodeId, int publishmentSystemId, int channelTemplateId, int contentTemplateId, string filePath, string channelFilePathRule, string contentFilePathRule) 
		{
            NodeId = nodeId;
            PublishmentSystemId = publishmentSystemId;
            ChannelTemplateId = channelTemplateId;
            ContentTemplateId = contentTemplateId;
            FilePath = filePath;
            ChannelFilePathRule = channelFilePathRule;
            ContentFilePathRule = contentFilePathRule;
		}

        public int NodeId { get; set; }

	    public int PublishmentSystemId { get; set; }

	    public int ChannelTemplateId { get; set; }

	    public int ContentTemplateId { get; set; }

	    public string FilePath { get; set; }

	    public string ChannelFilePathRule { get; set; }

	    public string ContentFilePathRule { get; set; }
	}
}
