using System;
using System.Collections.Generic;
using Datory;
using Datory.Annotations;

namespace SSCMS.Models
{
    public class Content : Entity
    {
        [DataColumn]
        public int ChannelId { get; set; }

        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public int AdminId { get; set; }

        [DataColumn]
        public int LastEditAdminId { get; set; }

        [DataColumn]
        public int UserId { get; set; }

        [DataColumn]
        public int Taxis { get; set; }

        [DataColumn]
        public List<string> GroupNames { get; set; }

        [DataColumn]
        public List<string> TagNames { get; set; }

        [DataColumn] 
        public int SourceId { get; set; }

        [DataColumn] 
        public int ReferenceId { get; set; }

        [DataColumn]
        public int TemplateId { get; set; }

        [DataColumn]
        public bool Checked { get; set; }

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
        public string SubTitle { get; set; }

        [DataColumn]
        public string ImageUrl { get; set; }

        [DataColumn]
        public string VideoUrl { get; set; }

        [DataColumn]
        public string FileUrl { get; set; }

        [DataColumn(Text = true)]
        public string Body { get; set; }

        [DataColumn(Text = true)]
        public string Summary { get; set; }

        [DataColumn]
        public string Author { get; set; }

        [DataColumn]
        public string Source { get; set; }

        [DataColumn]
        public bool Top { get; set; }

        [DataColumn]
        public bool Recommend { get; set; }

        [DataColumn]
        public bool Hot { get; set; }

        [DataColumn]
        public bool Color { get; set; }

        [DataColumn] 
        public string LinkUrl { get; set; }

        [DataColumn] 
        public DateTime? AddDate { get; set; }
    }
}
