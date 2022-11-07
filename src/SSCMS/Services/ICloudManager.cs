using System.Threading.Tasks;

namespace SSCMS.Services
{
    public interface ICloudManager: ICensorManager, ISpellManager, IMailManager, ISmsManager, IStorageManager, IVodManager
    {
        Task SetAuthentication(string userName, string token);

        Task RemoveAuthentication();
    }
}
