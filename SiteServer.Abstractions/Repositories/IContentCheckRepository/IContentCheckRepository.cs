using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;


namespace SiteServer.Abstractions
{
    public interface IContentCheckRepository : IRepository
    {
        Task<int> InsertAsync(ContentCheck checkInfo);

        Task<IEnumerable<ContentCheck>> GetCheckInfoListAsync(string tableName, int contentId);
    }
}