using System;
using System.Collections.Generic;
using SS.CMS.Data;

namespace SS.CMS.Models
{
    [Serializable]
    public class Content : Entity
    {
        public Content() { }

        public Content(IDictionary<string, object> dict) : base(dict)
        {

        }

        [DataColumn]
        public int ChannelId { get; set; }

        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public int UserId { get; set; }

        [DataColumn]
        public int LastModifiedUserId { get; set; }

        [DataColumn]
        public int Taxis { get; set; }

        [DataColumn]
        public string GroupNameCollection { get; set; }

        [DataColumn]
        public string Tags { get; set; }

        [DataColumn]
        public int SourceId { get; set; }

        [DataColumn]
        public int ReferenceId { get; set; }

        [DataColumn]
        public bool IsChecked { get; set; }

        [DataColumn]
        public int CheckedLevel { get; set; }

        [DataColumn]
        public int Hits { get; set; }

        [DataColumn]
        public int HitsByDay { get; set; }

        [DataColumn]
        public int HitsByWeek { get; set; }

        [DataColumn]
        public int HitsByMonth { get; set; }

        [DataColumn]
        public DateTime? LastHitsDate { get; set; }

        [DataColumn]
        public int Downloads { get; set; }

        [DataColumn]
        public string Title { get; set; }

        [DataColumn]
        public bool IsTop { get; set; }

        [DataColumn]
        public bool IsRecommend { get; set; }

        [DataColumn]
        public bool IsHot { get; set; }

        [DataColumn]
        public bool IsColor { get; set; }

        [DataColumn]
        public DateTime? AddDate { get; set; }

        [DataColumn]
        public string LinkUrl { get; set; }

        [DataColumn]
        public string SubTitle { get; set; }

        [DataColumn]
        public string ImageUrl { get; set; }

        [DataColumn]
        public string VideoUrl { get; set; }

        [DataColumn]
        public string FileUrl { get; set; }

        [DataColumn]
        public string Author { get; set; }

        [DataColumn]
        public string Source { get; set; }

        [DataColumn(Text = true)]
        public string Summary { get; set; }

        [DataColumn(Text = true)]
        public string Body { get; set; }

        [DataColumn(Text = true, Extend = true)]
        public string ExtendValues { get; set; }

        public string TitleFormatString { get; set; }

        public int CheckUserId { get; set; } //审核者
        public string CheckDate { get; set; }//审核时间
        public string CheckReasons { get; set; } //审核原因
        public string TranslateContentType { get; set; }//转移内容类型
    }
}
