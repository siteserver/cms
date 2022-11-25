using System;
using System.Threading.Tasks;
using SSCMS.Enums;

namespace SSCMS.Services
{
    public interface ICloudManager: ICensorManager, ISpellManager, IMailManager, ISmsManager, IStorageManager, IVodManager
    {
        Task<bool> IsAuthenticationAsync();
        
        Task SetAuthenticationAsync(string userName, string token);

        Task SetCloudTypeAsync(CloudType cloudType, DateTime expirationDate);

        Task<CloudType> GetCloudTypeAsync();

        Task RemoveAuthenticationAsync();

        Task<string> GetThemeDownloadUrlAsync(string userName, string name);

        Task<string> GetExtensionDownloadUrlAsync(string userName, string name, string version);

        Task<bool> IsImagesAsync();
    }
}
