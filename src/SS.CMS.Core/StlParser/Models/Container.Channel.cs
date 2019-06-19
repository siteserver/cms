using System;
using SS.CMS.Core.Models.Attributes;

namespace SS.CMS.Core.StlParser.Models
{
    public partial class Container
    {
        public static class Channel
        {
            public static readonly string SqlColumns = $"{ChannelAttribute.Id}, {ChannelAttribute.SiteId}, {ChannelAttribute.CreationDate}, {ChannelAttribute.Taxis}";

            public static readonly string[] Columns = new string[]
            {
                ChannelAttribute.Id,
                ChannelAttribute.SiteId,
                ChannelAttribute.CreationDate,
                ChannelAttribute.Taxis
            };
        }
    }
}