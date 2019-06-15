namespace SS.CMS.Abstractions.Repositories
{
    public partial interface IContentRepository
    {
        void Delete(int siteId, int contentId);
    }
}
