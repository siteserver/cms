using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;

namespace SS.CMS.Abstractions
{
    public interface ILibraryGroupRepository : IRepository
    {
        Task<int> InsertAsync(LibraryGroup group);

        Task<bool> UpdateAsync(LibraryGroup group);

        Task<bool> DeleteAsync(LibraryType type, int groupId);

        Task<List<LibraryGroup>> GetAllAsync(LibraryType type);

        Task<LibraryGroup> GetAsync(int groupId);

        Task<bool> IsExistsAsync(LibraryType type, string groupName);
    }
}