namespace SiteServer.CMS.StlParser.Model
{
	public class ContentItemInfo
	{
	    public ContentItemInfo(int channelId, int contentId, int itemIndex)
		{
            ChannelId = channelId;
		    ContentId = contentId;
            ItemIndex = itemIndex;
		}

        public int ChannelId { get; private set; }

        public int ContentId { get; private set; }

        public int ItemIndex { get; private set; }

	    public void Reload(int channelId, int contentId, int itemIndex)
        {
            ChannelId = channelId;
            ContentId = contentId;
            ItemIndex = itemIndex;
        }
	}
}
