using System;


namespace SiteServer.Abstractions
{
    [Serializable]
    public class ContentMin: IContentMin
    {
        public int Id { get; set; }

        public int ChannelId { get; set; }

        public string IsTop { get; set; }

        public DateTime? AddDate { get; set; }

        public DateTime? LastEditDate { get; set; }

        public int Taxis { get; set; }

        public int Hits { get; set; }

        public int HitsByDay { get; set; }

        public int HitsByWeek { get; set; }

        public int HitsByMonth { get; set; }
    }
}
