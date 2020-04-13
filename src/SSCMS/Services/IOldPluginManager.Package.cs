using SSCMS.Enums;

namespace SSCMS.Services
{
    public partial interface IOldPluginManager
    {
        bool UpdatePackage(string idWithVersion, PackageType packageType, out string errorMessage);
    }
}
