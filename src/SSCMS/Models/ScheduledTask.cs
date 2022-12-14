using System;
using System.Collections.Generic;
using Datory;
using Datory.Annotations;
using SSCMS.Enums;

namespace SSCMS.Models
{
    [DataTable("siteserver_ScheduledTask")]
    public class ScheduledTask : Entity
    {
        [DataColumn]
        public string Title { get; set; }

        [DataColumn]
        public string Description { get; set; }

        [DataColumn]
        public int AdminId { get; set; }

        [DataColumn]
        public TaskType TaskType { get; set; }

        [DataColumn]
        public TaskInterval TaskInterval { get; set; }

        [DataColumn]
        public int Every { get; set; }

        [DataColumn]
        public List<int> Weeks { get; set; }

        [DataColumn]
        public DateTime StartDate { get; set; }

        [DataColumn]
        public bool IsNoticeSuccess { get; set; }

        [DataColumn]
        public bool IsNoticeFailure { get; set; }

        [DataColumn]
        public int NoticeFailureCount { get; set; }

        [DataColumn]
        public bool IsNoticeMobile { get; set; }

        [DataColumn]
        public string NoticeMobile { get; set; }

        [DataColumn]
        public bool IsNoticeMail { get; set; }

        [DataColumn]
        public string NoticeMail { get; set; }

        [DataColumn]
        public bool IsDisabled { get; set; }

        [DataColumn]
        public int Timeout { get; set; }

        [DataColumn]
        public bool IsRunning { get; set; }

        [DataColumn]
        public DateTime? LatestStartDate { get; set; }

        [DataColumn]
        public DateTime? LatestEndDate { get; set; }

        [DataColumn]
        public bool IsLatestSuccess { get; set; }

        [DataColumn]
        public int LatestFailureCount { get; set; }

        [DataColumn]
        public string LatestErrorMessage { get; set; }

        [DataColumn]
        public DateTime? ScheduledDate { get; set; }

        public List<int> CreateSiteIds { get; set; }
        public CreateType CreateType { get; set; }
        public string PingHost { get; set; }
        public int PublishSiteId { get; set; }
        public int PublishChannelId { get; set; }
        public int PublishContentId { get; set; }
    }
}
