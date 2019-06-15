using System;
using SS.CMS.Core.Models.Attributes;

namespace SS.CMS.Core.StlParser.Models
{
    public partial class Container
    {
        public static class Content
        {
            public readonly static string SqlColumns = $"{ContentAttribute.Id}, {ContentAttribute.SiteId}, {ContentAttribute.ChannelId}, {ContentAttribute.IsTop}, {ContentAttribute.AddDate}, {ContentAttribute.LastEditDate}, {ContentAttribute.Taxis}, {ContentAttribute.Hits}, {ContentAttribute.HitsByDay}, {ContentAttribute.HitsByWeek}, {ContentAttribute.HitsByMonth}";
        }

    }
}