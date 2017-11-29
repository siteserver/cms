using System;

namespace SiteServer.Plugin.Models
{
    public interface IContentInfo : IAttributes
    {
        int Id { get; set; }

        int NodeId { get; set; }

        int PublishmentSystemId { get; set; }

        string AddUserName { get; set; }

        string LastEditUserName { get; set; }

        string WritingUserName { get; set; }

        DateTime LastEditDate { get; set; }

        int Taxis { get; set; }

        string ContentGroupNameCollection { get; set; }

        string Tags { get; set; }

        int SourceId { get; set; }

        int ReferenceId { get; set; }

        bool IsChecked { get; set; }

        int CheckedLevel { get; set; }

        int Comments { get; set; }

        int Photos { get; set; }

        int Hits { get; set; }

        int HitsByDay { get; set; }

        int HitsByWeek { get; set; }

        int HitsByMonth { get; set; }

        DateTime LastHitsDate { get; set; }

        string Title { get; set; }

        bool IsTop { get; set; }

        bool IsRecommend { get; set; }

        bool IsHot { get; set; }

        bool IsColor { get; set; }

        DateTime AddDate { get; set; }
    }
}
