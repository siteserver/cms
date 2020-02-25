namespace SS.CMS.Web.Controllers.Stl
{
    public partial class ActionsDownloadController
    {
        public class GetRequest
        {
            public int? SiteId { get; set; }
            public int? ChannelId { get; set; }
            public int? ContentId { get; set; }
            public string FileUrl { get; set; }
            public string FilePath { get; set; }
        }
    }
}
