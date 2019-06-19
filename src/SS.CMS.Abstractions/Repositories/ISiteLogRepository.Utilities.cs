namespace SS.CMS.Repositories
{
    public partial interface ISiteLogRepository
    {
        void AddSiteLog(int siteId, int channelId, int contentId, string ipAddress, string adminName, string action, string summary);
    }
}
