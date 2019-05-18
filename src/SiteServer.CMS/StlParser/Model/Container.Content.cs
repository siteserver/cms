using System;
using System.Collections.Specialized;
using System.Text;
using SiteServer.CMS.Model.Attributes;
using SiteServer.Utils;

namespace SiteServer.CMS.StlParser.Model
{
    public partial class Container
    {
        public class Content
        {
            public readonly static string SqlColumns = $"{ContentAttribute.Id}, {ContentAttribute.SiteId}, {ContentAttribute.ChannelId}, {ContentAttribute.IsTop}, {ContentAttribute.AddDate}, {ContentAttribute.LastEditDate}, {ContentAttribute.Taxis}, {ContentAttribute.Hits}, {ContentAttribute.HitsByDay}, {ContentAttribute.HitsByWeek}, {ContentAttribute.HitsByMonth}";

            public int ItemIndex { get; set; }
            public int Id { get; set; }
            public int SiteId { get; set; }
            public int ChannelId { get; set; }
            public bool Top { get; set; }
            public DateTime? AddDate { get; set; }
            public DateTime? LastEditDate { get; set; }
            public int Taxis { get; set; }
            public int Hits { get; set; }
            public int HitsByDay { get; set; }
            public int HitsByWeek { get; set; }
            public int HitsByMonth { get; set; }
        }

    }
}