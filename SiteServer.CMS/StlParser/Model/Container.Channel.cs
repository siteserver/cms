using System;
using System.Collections.Specialized;
using System.Text;
using SiteServer.CMS.Model.Attributes;
using SiteServer.Utils;

namespace SiteServer.CMS.StlParser.Model
{
    public partial class Container
    {
        public class Channel
        {
            public static readonly string SqlColumns = $"{ChannelAttribute.Id}, {ChannelAttribute.SiteId}, {ChannelAttribute.AddDate}, {ChannelAttribute.Taxis}";

            public int ItemIndex { get; set; }

            public int Id { get; set; }

            public int SiteId { get; set; }

            public DateTime? AddDate { get; set; }

            public int Taxis { get; set; }
        }

    }
}