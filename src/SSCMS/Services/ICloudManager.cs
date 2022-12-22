using System;
using System.Threading.Tasks;
using SSCMS.Dto;
using SSCMS.Enums;

namespace SSCMS.Services
{
    public interface ICloudManager: ICensorManager, ISpellManager, IMailManager, ISmsManager, IStorageManager, IVodManager
    {
        Task<bool> IsAuthenticationAsync();
        
        Task SetAuthenticationAsync(int userId, string userName, string mobile, string token);

        Task SetCloudTypeAsync(CloudType cloudType, DateTime expirationDate);

        Task<CloudType> GetCloudTypeAsync();

        Task RemoveAuthenticationAsync();

        Task<string> GetThemeDownloadUrlAsync(string userName, string name);

        Task<string> GetExtensionDownloadUrlAsync(string userName, string name, string version);

        Task<OssCredentials> GetOssCredentialsAsync();

        Task<bool> IsImagesAsync();

        Task BackupAsync(long size);

        Task RestoreAsync(string restoreId, string backupId);

        int GetRestoreProgress(string restoreId);
    }
}
