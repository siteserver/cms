using System;
using System.Collections.Generic;
using SiteServer.Utils;

namespace SiteServer.CMS.StlParser.Model
{
    public class MinContentInfo
    {
        public int Id { get; set; }

        public int ChannelId { get; set; }

        public static List<MinContentInfo> ParseMinContentInfoList(string channelContentIdsString)
        {
            var channelContentIds = new List<MinContentInfo>();
            if (!string.IsNullOrEmpty(channelContentIdsString))
            {
                foreach (var channelContentId in TranslateUtils.StringCollectionToStringList(channelContentIdsString))
                {
                    var arr = channelContentId.Split('_');
                    if (arr.Length == 2)
                    {
                        channelContentIds.Add(new MinContentInfo
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
