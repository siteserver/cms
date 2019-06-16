using SS.CMS.Data;

namespace SS.CMS.Abstractions.Repositories
{
    public partial interface IContentRepository : IRepository
    {
        string GetContentTableName(int siteId);
    }
}
