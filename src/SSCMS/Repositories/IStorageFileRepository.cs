using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public interface IStorageFileRepository : IRepository
    {
        Task<int> InsertAsync(StorageFile storageFile);

        Task DeleteAsync(string key);

        Task<List<StorageFile>> GetStorageFileListAsync();

        Task<StorageFile> GetStorageFileByKeyAsync(string key);
    }
}