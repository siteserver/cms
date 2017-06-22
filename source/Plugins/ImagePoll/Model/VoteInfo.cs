using System;

namespace ImagePoll.Model
{
    public class VoteInfo
    {
        public int Id { get; set; }
        public int SiteId { get; set; }
        public int KeywordId { get; set; }
        public bool IsDisabled { get; set; }
        public int UserCount { get; set; }
        public int PvCount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Summary { get; set; }
        public string ContentImageUrl { get; set; }
        public string ContentDescription { get; set; }
        public string ContentIsImageOption { get; set; }
        public string ContentIsCheckBox { get; set; }
        public string ContentResultVisible { get; set; }
        public string EndTitle { get; set; }
        public string EndImageUrl { get; set; }
        public string EndSummary { get; set; }
    }
}
