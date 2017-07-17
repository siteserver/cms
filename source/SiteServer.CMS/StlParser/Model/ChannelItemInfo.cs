namespace SiteServer.CMS.StlParser.Model
{
	public class ChannelItemInfo
	{
	    public ChannelItemInfo(int channelId, int itemIndex)
		{
            ChannelId = channelId;
            ItemIndex = itemIndex;
		}

        public int ChannelId { get; private set; }

        public int ItemIndex { get; private set; }

	    public void Reload(int channelId, int itemIndex)
        {
            ChannelId = channelId;
            ItemIndex = itemIndex;
        }
	}
}
