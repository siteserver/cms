using System.Threading.Tasks;
using Datory;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public partial interface IContentRepository : IRepository
    {
        Task<Repository<Content>> GetRepositoryAsync(string tableName);
    }
}
