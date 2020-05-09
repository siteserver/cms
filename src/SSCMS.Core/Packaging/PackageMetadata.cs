using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NuGet.Packaging;
using NuGet.Versioning;
using SSCMS.Plugins;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Packaging
{
    public class PackageMetadata : IPackageMetadata
    {
        private readonly Dictionary<string, string> _metadata;
        private readonly IReadOnlyCollection<PackageDependencyGroup> _dependencyGroups;
        private readonly IReadOnlyCollection<FrameworkSpecificGroup> _frameworkReferenceGroups;
        private readonly IReadOnlyCollection<NuGet.Packaging.Core.PackageType> _packageTypes;

        public PackageMetadata(string directoryName)
        {
            PluginId = directoryName;
            Name = directoryName;
            IconUrl = "https://www.siteserver.cn/assets/images/favicon.png";
            Version = "0.0.0";
        }

        public PackageMetadata(IOldPlugin plugin)
        {
            PluginId = plugin.PluginId;
            Version = plugin.Version;
            IconUrl = plugin.IconUrl;
            ProjectUrl = plugin.ProjectUrl;
            LicenseUrl = plugin.LicenseUrl;
            Copyright = plugin.Copyright;
            Description = plugin.Description;
            ReleaseNotes = plugin.ReleaseNotes;
            Name = plugin.Name;
            Tags = plugin.Tags;
            Language = plugin.Language;
            Owners = plugin.Owners;
            Authors = plugin.Authors;
        }

        public PackageMetadata(
            Dictionary<string, string> metadata,
            IEnumerable<PackageDependencyGroup> dependencyGroups,
            IEnumerable<FrameworkSpecificGroup> frameworkGroups,
            IEnumerable<NuGet.Packaging.Core.PackageType> packageTypes,
            NuGetVersion minClientVersion)
        {
            _metadata = new Dictionary<string, string>(metadata, StringComparer.OrdinalIgnoreCase);
            _dependencyGroups = dependencyGroups.ToList().AsReadOnly();
            _frameworkReferenceGroups = frameworkGroups.ToList().AsReadOnly();
            _packageTypes = packageTypes.ToList().AsReadOnly();

            SetPropertiesFromMetadata();
            MinClientVersion = minClientVersion;
        }

        private void SetPropertiesFromMetadata()
        {
            PluginId = GetValue(PackageMetadataStrings.Id, string.Empty);
            Version = GetValue(PackageMetadataStrings.Version, string.Empty);
            IconUrl = GetValue(PackageMetadataStrings.IconUrl, string.Empty);
            ProjectUrl = GetValue(PackageMetadataStrings.ProjectUrl, string.Empty);
            LicenseUrl = GetValue(PackageMetadataStrings.LicenseUrl, string.Empty);
            Copyright = GetValue(PackageMetadataStrings.Copyright, string.Empty);
            Description = GetValue(PackageMetadataStrings.Description, string.Empty);
            ReleaseNotes = GetValue(PackageMetadataStrings.ReleaseNotes, string.Empty);
            Name = GetValue(PackageMetadataStrings.Name, string.Empty);
            Tags = GetValue(PackageMetadataStrings.Tags, string.Empty);
            Language = GetValue(PackageMetadataStrings.Language, string.Empty);
            Owners = GetValue(PackageMetadataStrings.Owners, string.Empty);
            Authors = GetValue(PackageMetadataStrings.Authors, string.Empty);

            if (Version.IndexOf('.') < 0)
            {
                throw new FormatException($"版本号不正确：{Version}");
            }

            NuGetVersion nugetVersion;
            if (NuGetVersion.TryParse(Version, out nugetVersion))
            {
                NuGetVersion = nugetVersion;
            }
        }

        public string PluginId { get; private set; }
        public string Version { get; private set; }
        public string IconUrl { get; private set; }
        public string ProjectUrl { get; private set; }
        public string LicenseUrl { get; private set; }
        public string Copyright { get; private set; }
        public string Description { get; private set; }
        public string ReleaseNotes { get; private set; }
        public string Name { get; private set; }
        public string Tags { get; private set; }
        public string Authors { get; private set; }
        public string Owners { get; private set; }
        public string Language { get; private set; }

        public NuGetVersion NuGetVersion { get; private set; }
        public NuGetVersion MinClientVersion { get; set; }

        public string GetValueFromMetadata(string key)
        {
            return GetValue(key, string.Empty);
        }

        public IReadOnlyCollection<PackageDependencyGroup> GetDependencyGroups()
        {
            return _dependencyGroups;
        }

        public IReadOnlyCollection<FrameworkSpecificGroup> GetFrameworkReferenceGroups()
        {
            return _frameworkReferenceGroups;
        }

        public IReadOnlyCollection<NuGet.Packaging.Core.PackageType> GetPackageTypes()
        {
            return _packageTypes;
        }

        private string GetValue(string key, string alternateValue)
        {
            string value;
            if (_metadata.TryGetValue(key, out value))
            {
                return value;
            }

            return alternateValue;
        }

        private bool GetValue(string key, bool alternateValue)
        {
            var value = GetValue(key, alternateValue.ToString());

            bool result;
            if (bool.TryParse(value, out result))
            {
                return result;
            }

            return alternateValue;
        }

        private Uri GetValue(string key, Uri alternateValue)
        {
            var value = GetValue(key, string.Empty);
            if (!string.IsNullOrEmpty(value))
            {
                Uri result;
                if (Uri.TryCreate(value, UriKind.Absolute, out result))
                {
                    return result;
                }
            }

            return alternateValue;
        }

        /// <summary>
        /// Gets package metadata from a the provided <see cref="NuspecReader"/> instance.
        /// </summary>
        /// <param name="nuspecReader">The </param>
        public static PackageMetadata FromNuspecReader(NuspecReader nuspecReader)
        {
            return new PackageMetadata(
                nuspecReader.GetMetadata().ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                nuspecReader.GetDependencyGroups(true),
                nuspecReader.GetFrameworkAssemblyGroups(),
                nuspecReader.GetPackageTypes(),
                nuspecReader.GetMinClientVersion()
           );
        }

        public static PackageMetadata GetPackageMetadataFromPackages(IPathManager pathManager, string directoryName, out string nuspecPath, out string dllDirectoryPath, out string errorMessage)
        {
            nuspecPath = string.Empty;
            dllDirectoryPath = string.Empty;
            errorMessage = string.Empty;

            var directoryPath = pathManager.GetPackagesPath(directoryName);

            foreach (var filePath in DirectoryUtils.GetFilePaths(directoryPath))
            {
                if (StringUtils.EqualsIgnoreCase(Path.GetExtension(filePath), ".zip"))
                {
                    nuspecPath = filePath;
                    break;
                }
            }

            if (string.IsNullOrEmpty(nuspecPath))
            {
                errorMessage = "配置文件不存在";
                return null;
            }

            PackageMetadata metadata;
            try
            {
                metadata = GetPackageMetadata(nuspecPath);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return null;
            }

            var packageId = metadata.PluginId;

            if (string.IsNullOrEmpty(packageId))
            {
                errorMessage = $"配置文件 {nuspecPath} 不正确";
                return null;
            }

            dllDirectoryPath = FindDllDirectoryPath(directoryPath);

            //if (!FileUtils.IsFileExists(PathUtils.Combine(dllDirectoryPath, packageId + ".dll")))
            //{
            //    errorMessage = $"插件可执行文件 {packageId}.dll 不存在";
            //    return null;
            //}

            return metadata;
        }

        //https://docs.microsoft.com/en-us/nuget/schema/target-frameworks#supported-frameworks
        private static string FindDllDirectoryPath(string packageDirectoryPath)
        {
            var dllDirectoryPath = string.Empty;

            foreach (var dirName in DirectoryUtils.GetDirectoryNames(PathUtils.Combine(packageDirectoryPath, "lib")))
            {
                if (StringUtils.StartsWithIgnoreCase(dirName, "net45") ||
                    StringUtils.StartsWithIgnoreCase(dirName, "net451") ||
                    StringUtils.StartsWithIgnoreCase(dirName, "net452") ||
                    StringUtils.StartsWithIgnoreCase(dirName, "net46") ||
                    StringUtils.StartsWithIgnoreCase(dirName, "net461") ||
                    StringUtils.StartsWithIgnoreCase(dirName, "net462"))
                {
                    dllDirectoryPath = PathUtils.Combine(packageDirectoryPath, "lib", dirName);
                    break;
                }
            }
            if (string.IsNullOrEmpty(dllDirectoryPath))
            {
                dllDirectoryPath = PathUtils.Combine(packageDirectoryPath, "lib");
            }

            return dllDirectoryPath;
        }

        private static PackageMetadata GetPackageMetadata(string configPath)
        {
            var nuspecReader = new NuspecReader(configPath);

            var rawMetadata = nuspecReader.GetMetadata();
            if (rawMetadata == null || !rawMetadata.Any()) return null;

            return PackageMetadata.FromNuspecReader(nuspecReader);
        }
    }
}
