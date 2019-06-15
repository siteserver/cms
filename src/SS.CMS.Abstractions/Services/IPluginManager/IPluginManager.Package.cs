using System;
using System.IO;
using System.Linq;
using SS.CMS.Abstractions.Enums;

namespace SS.CMS.Abstractions.Services
{
    public partial interface IPluginManager
    {
        IPackageMetadata GetPackageMetadataFromPluginDirectory(string directoryName, out string errorMessage);

        IPackageMetadata GetPackageMetadataFromPackages(string directoryName, out string nuspecPath, out string dllDirectoryPath, out string errorMessage);

        void DownloadPackage(string packageId, string version);

        bool UpdatePackage(string idWithVersion, PackageType packageType, out string errorMessage);
    }
}