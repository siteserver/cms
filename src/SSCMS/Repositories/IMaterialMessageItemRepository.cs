using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Enums;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public interface IMaterialMessageItemRepository : IRepository
    {
        Task<int> InsertAsync(MaterialMessageItem item);

        Task DeleteAllAsync(int messageId);

        Task<List<MaterialMessageItem>> GetAllAsync(int messageId);

        Task<bool> IsDeletable(MaterialType materialType, int materialId);
    }
}