using System;
using SS.CMS.Core.Models.Attributes;

namespace SS.CMS.Core.StlParser.Models
{
    public partial class Container
    {
        public class Channel
        {
            public static readonly string SqlColumns = $"{ChannelAttribute.Id}, {ChannelAttribute.SiteId}, {ChannelAttribute.AddDate}, {ChannelAttribute.Taxis}";

            public static readonly string[] Columns = new string[]
            {
                ChannelAttribute.Id,
                ChannelAttribute.SiteId,
                ChannelAttribute.AddDate,
                ChannelAttribute.Taxis
            };

            public int ItemIndex { get; set; }

            public int Id { get; set; }

            public int SiteId { get; set; }

            public DateTime? AddDate { get; set; }

            public int Taxis { get; set; }
        }

    }
}