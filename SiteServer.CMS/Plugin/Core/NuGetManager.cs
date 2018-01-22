using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiteServer.Utils;
using NuGet;
using NuGet.Common;
using NuGet.Packaging;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin.Model;
using ILogger = NuGet.Common.ILogger;

namespace SiteServer.CMS.Plugin.Core
{
    // https://blog.nuget.org/20130520/Play-with-packages.html
    // https://haacked.com/archive/2011/01/15/building-a-self-updating-site-using-nuget.aspx/
    // https://github.com/caleb-vear/NuSelfUpdate
    public class NuGetManager
    {
        public static string TestGetLastPackage(bool isPreviewVersion)
        {
            var packageID = "Newtonsoft.Json";

            //Connect to the official package repository
            IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2");

            IPackage p = repo.FindPackage(packageID);

            var builder = new StringBuilder();
            builder.Append(p.GetFullName()).Append("<br />");

            return builder.ToString();
        }

        public static string TestGetReleaseVersionList()
        {
            var packageID = "Newtonsoft.Json";

            //Connect to the official package repository
            IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2");

            //Get the list of all NuGet packages with ID 'EntityFramework'       
            List<IPackage> packages = repo.FindPackagesById(packageID).ToList();

            //Filter the list of packages that are not Release (Stable) versions
            packages = packages.Where(item => (item.IsReleaseVersion())).ToList();

            packages.Reverse();

            //Iterate through the list and print the full name of the pre-release packages to console
            var builder = new StringBuilder();
            foreach (IPackage p in packages)
            {
                builder.Append(p.GetFullName()).Append("<br />");
            }

            return builder.ToString();
        }

        public static void TestGetAndInstall()
        {
            //ID of the package to be looked up
            string packageID = "EntityFramework";

            //Connect to the official package repository
            IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2");

            //Initialize the package manager
            string path = PathUtils.GetPackagesPath();
            PackageManager packageManager = new PackageManager(repo, path);

            //Download and unzip the package
            packageManager.InstallPackage(packageID, SemanticVersion.Parse("5.0.0"));
        }

        public static string TestGet10()
        {
            string url = "https://www.nuget.org/api/v2/";
            IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository(url);
            var packages = repo
                .GetPackages()
                .Where(p => p.IsLatestVersion)
                .OrderByDescending(p => p.DownloadCount)
                .Take(10);

            var builder = new StringBuilder();

            foreach (IPackage package in packages)
            {
                builder.Append(package);
            }

            return builder.ToString();
        }

        //public static async Task<string> GetMetadata()
        //{
        //    Logger logger = new Logger();
        //    List<Lazy<INuGetResourceProvider>> providers = new List<Lazy<INuGetResourceProvider>>();
        //    providers.AddRange(Repository.Provider.GetCoreV3());  // Add v3 API support
        //    PackageSource packageSource = new PackageSource("https://api.nuget.org/v3/index.json");
        //    SourceRepository sourceRepository = new SourceRepository(packageSource, providers);

        //    IPackageMetadata

        //    PackageMetadataResource packageMetadataResource = await sourceRepository.GetResourceAsync<PackageMetadataResource>();
        //    IEnumerable<IPackageSearchMetadata> searchMetadata = await packageMetadataResource.GetMetadataAsync("Wyam.Core", true, true, logger, CancellationToken.None);
        //    var builder = new StringBuilder();
        //    foreach (var packageSearchMetadata in searchMetadata)
        //    {
        //        builder.Append(packageSearchMetadata.);
        //        builder.Append("//////////////////////////////////////////////////////////");
        //    }
        //    return builder.ToString();
        //}

        public static PluginMetadata GetPackageMetadata(string nuspecPath)
        {
            NuspecReader nuspecReader = new NuspecReader(nuspecPath);

            var rawMetadata = nuspecReader.GetMetadata();
            if (rawMetadata != null && rawMetadata.Any())
            {
                var metadata = PluginMetadata.FromNuspecReader(nuspecReader);
                return metadata;
            }

            return null;
        }
    }

    public class Logger : ILogger
    {
        public void LogDebug(string data)
        {
            DataProvider.RecordDao.RecordLog(data, nameof(LogDebug));
        }

        public void LogVerbose(string data)
        {
            DataProvider.RecordDao.RecordLog(data, nameof(LogVerbose));
        }

        public void LogInformation(string data)
        {
            DataProvider.RecordDao.RecordLog(data, nameof(LogInformation));
        }

        public void LogMinimal(string data)
        {
            DataProvider.RecordDao.RecordLog(data, nameof(LogMinimal));
        }

        public void LogWarning(string data)
        {
            DataProvider.RecordDao.RecordLog(data, nameof(LogWarning));
        }

        public void LogError(string data)
        {
            DataProvider.RecordDao.RecordLog(data, nameof(LogError));
        }

        public void LogInformationSummary(string data)
        {
            DataProvider.RecordDao.RecordLog(data, nameof(LogInformationSummary));
        }

        public void Log(LogLevel level, string data)
        {
            DataProvider.RecordDao.RecordLog(data, nameof(Log));
        }

        public Task LogAsync(LogLevel level, string data)
        {
            return null;
        }

        public void Log(ILogMessage message)
        {
            DataProvider.RecordDao.RecordLog(message.Message, nameof(Log));
        }

        public Task LogAsync(ILogMessage message)
        {
            return null;
        }
    }
}
