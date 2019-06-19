namespace SS.CMS.Repositories
{
    public partial interface ISiteRepository
    {
        int GetSiteIdByIsRoot();

        int GetSiteIdBySiteDir(string siteDir);
    }
}
