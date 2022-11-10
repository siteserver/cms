using System.Threading.Tasks;

namespace SSCMS.Services
{
    public interface ICloudManager: ICensorManager, ISpellManager, IMailManager, ISmsManager, IStorageManager, IVodManager
    {
        Task<bool> IsAuthenticationAsync();
        
        Task SetAuthentication(string userName, string token);

        Task RemoveAuthentication();

        Task<string> GetThemeDownloadUrlAsync(string userName, string name);

        Task<string> GetExtensionDownloadUrlAsync(string userName, string name, string version);
    }
}
