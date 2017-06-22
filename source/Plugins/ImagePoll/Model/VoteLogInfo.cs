using System;

namespace ImagePoll.Model
{
    public class VoteLogInfo
    {
        public int Id { get; set; }
        public int SiteId { get; set; }
        public int VoteId { get; set; }
        public string ItemIdCollection { get; set; }
        public string IpAddress { get; set; }
        public string CookieSn { get; set; }
        public string WxOpenId { get; set; }
        public string UserName { get; set; }
        public DateTime AddDate { get; set; }
    }
}
