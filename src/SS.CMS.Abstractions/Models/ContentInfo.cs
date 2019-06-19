using System;
using System.Collections.Generic;
using SS.CMS.Data;

namespace SS.CMS.Models
{
    public class ContentInfo : Entity
    {
        public ContentInfo() { }

        public ContentInfo(IDictionary<string, object> dict) : base(dict)
        {

        }

        [TableColumn]
        public int ChannelId { get; set; }

        [TableColumn]
        public int SiteId { get; set; }

        [TableColumn]
        public string AddUserName { get; set; }

        [TableColumn]
        public string LastEditUserName { get; set; }

        [TableColumn]
        public int UserId { get; set; }

        [TableColumn]
        public int Taxis { get; set; }

        [TableColumn]
        public string GroupNameCollection { get; set; }

        [TableColumn]
        public string Tags { get; set; }

        [TableColumn]
        public int SourceId { get; set; }

        [TableColumn]
        public int ReferenceId { get; set; }

        [TableColumn]
        public bool IsChecked { get; set; }

        [TableColumn]
        public int CheckedLevel { get; set; }

        [TableColumn]
        public int Hits { get; set; }

        [TableColumn]
        public int HitsByDay { get; set; }

        [TableColumn]
        public int HitsByWeek { get; set; }

        [TableColumn]
        public int HitsByMonth { get; set; }

        [TableColumn]
        public DateTimeOffset? LastHitsDate { get; set; }

        [TableColumn]
        public int Downloads { get; set; }

        [TableColumn]
        public string Title { get; set; }

        [TableColumn]
        public bool IsTop { get; set; }

        [TableColumn]
        public bool IsRecommend { get; set; }

        [TableColumn]
        public bool IsHot { get; set; }

        [TableColumn]
        public bool IsColor { get; set; }

        [TableColumn]
        public DateTimeOffset? AddDate { get; set; }

        [TableColumn]
        public string LinkUrl { get; set; }

        [TableColumn]
        public string SubTitle { get; set; }

        [TableColumn]
        public string ImageUrl { get; set; }

        [TableColumn]
        public string VideoUrl { get; set; }

        [TableColumn]
        public string FileUrl { get; set; }

        [TableColumn]
        public string Author { get; set; }

        [TableColumn]
        public string Source { get; set; }

        [TableColumn(Text = true)]
        public string Summary { get; set; }

        [TableColumn(Text = true)]
        public string Content { get; set; }

        [TableColumn(Text = true, Extend = true)]
        public string ExtendValues { get; set; }

        public string TitleFormatString { get; set; }

        public string CheckUserName { get; set; } //审核者
        public string CheckDate { get; set; }//审核时间
        public string CheckReasons { get; set; } //审核原因
        public string TranslateContentType { get; set; }//转移内容类型
    }
}
