namespace SSCMS
{
    public class SiteSummary
    {
        public int Id { get; set; }
        public string SiteName { get; set; }
        public string SiteDir { get; set; }
        public string TableName { get; set; }
        public bool Root { get; set; }
        public int ParentId { get; set; }
        public int Taxis { get; set; }
    }
}
