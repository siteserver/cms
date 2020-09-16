using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public interface IContentCheckRepository : IRepository
    {

        Task InsertAsync(ContentCheck check);

        Task<List<ContentCheck>> GetCheckListAsync(int siteId, int channelId, int contentId);
    }
}