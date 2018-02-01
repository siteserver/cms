using System;
using System.Collections.Generic;
using System.Linq;
using NuGet.Packaging;
using NuGet.Versioning;
using SiteServer.Plugin;

namespace SiteServer.CMS.Packaging
{
    public class PackageMetadata : IMetadata
    {
        private readonly Dictionary<string, string> _metadata;
        private readonly IReadOnlyCollection<PackageDependencyGroup> _dependencyGroups;
        private readonly IReadOnlyCollection<FrameworkSpecificGroup> _frameworkReferenceGroups;
        private readonly IReadOnlyCollection<NuGet.Packaging.Core.PackageType> _packageTypes;

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
            Id = GetValue("id", string.Empty);
            Version = GetValue("version", string.Empty);
            IconUrl = GetValue(PackageMetadataStrings.IconUrl, (Uri)null);
            ProjectUrl = GetValue(PackageMetadataStrings.ProjectUrl, (Uri)null);
            LicenseUrl = GetValue(PackageMetadataStrings.LicenseUrl, (Uri)null);
            Copyright = GetValue(PackageMetadataStrings.Copyright, (string)null);
            Description = GetValue(PackageMetadataStrings.Description, (string)null);
            ReleaseNotes = GetValue(PackageMetadataStrings.ReleaseNotes, (string)null);
            RequireLicenseAcceptance = GetValue(PackageMetadataStrings.RequireLicenseAcceptance, false);
            Summary = GetValue(PackageMetadataStrings.Summary, (string)null);
            Title = GetValue(PackageMetadataStrings.Title, (string)null);
            Tags = GetValue(PackageMetadataStrings.Tags, (string)null);
            Language = GetValue(PackageMetadataStrings.Language, (string)null);
            Owners = GetValue(PackageMetadataStrings.Owners, (string)null);
            Authors = new List<string>(GetValue(PackageMetadataStrings.Authors, Owners ?? string.Empty).Split(',').Select(author => author.Trim()));

            if (Version == "$version$")
            {
                Version = PackageUtils.VersionDev;
            }

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

        public string Id { get; private set; }
        public string Version { get; private set; }
        public Uri IconUrl { get; private set; }
        public Uri ProjectUrl { get; private set; }
        public Uri LicenseUrl { get; private set; }
        public string Copyright { get; private set; }
        public string Description { get; private set; }
        public string ReleaseNotes { get; private set; }
        public bool RequireLicenseAcceptance { get; private set; }
        public string Summary { get; private set; }
        public string Title { get; private set; }
        public string Tags { get; private set; }
        public List<string> Authors { get; private set; }
        public string Owners { get; private set; }
        public string Language { get; private set; }

        public NuGetVersion NuGetVersion { get; private set; }
        public NuGetVersion MinClientVersion { get; set; }

        public string GetValueFromMetadata(string key)
        {
            return GetValue(key, (string)null);
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
            var value = GetValue(key, (string)null);
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
                nuspecReader.GetFrameworkReferenceGroups(),
                nuspecReader.GetPackageTypes(),
                nuspecReader.GetMinClientVersion()
           );
        }
    }
}
