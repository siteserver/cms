using System.Collections.Generic;

namespace SiteServer.Abstractions
{
    public class ChannelContentId
    {
        public int ChannelId { get; set; }

        public int Id { get; set; }

        public static List<ChannelContentId> ParseMinContentInfoList(string channelContentIdsString)
        {
            var channelContentIds = new List<ChannelContentId>();
            if (!string.IsNullOrEmpty(channelContentIdsString))
            {
                foreach (var channelContentId in StringUtils.GetStringList(channelContentIdsString))
                {
                    var arr = channelContentId.Split('_');
                    if (arr.Length == 2)
                    {
                        channelContentIds.Add(new ChannelContentId
                        {
                            ChannelId = TranslateUtils.ToInt(arr[0]),
                            Id = TranslateUtils.ToInt(arr[1])
                        });
                    }
                }
            }

            return channelContentIds;
        }
    }
}
