namespace SSCMS
{
    public partial interface IPluginManager
    {
        bool UpdatePackage(string idWithVersion, PackageType packageType, out string errorMessage);
    }
}
