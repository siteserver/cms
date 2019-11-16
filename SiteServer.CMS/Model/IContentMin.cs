using System;

namespace SiteServer.CMS.Model
{
    public interface IContentMin
    {
        int Id { get; set; }

        int ChannelId { get; set; }

        string IsTop { get; set; }

        DateTime? AddDate { get; set; }

        DateTime? LastEditDate { get; set; }

        int Taxis { get; set; }

        int Hits { get; set; }

        int HitsByDay { get; set; }

        int HitsByWeek { get; set; }

        int HitsByMonth { get; set; }
    }
}
