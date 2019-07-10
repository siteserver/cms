using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public interface IContentCheckRepository : IRepository
    {
        Task<int> InsertAsync(ContentCheck checkInfo);

        Task<IEnumerable<ContentCheck>> GetCheckInfoListAsync(string tableName, int contentId);
    }
}