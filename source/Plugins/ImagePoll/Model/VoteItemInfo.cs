namespace ImagePoll.Model
{
    public class VoteItemInfo
    {
        public int Id { get; set; }
        public int VoteId { get; set; }
        public int SiteId { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string NavigationUrl { get; set; }
        public int VoteNum { get; set; }
    }
}
