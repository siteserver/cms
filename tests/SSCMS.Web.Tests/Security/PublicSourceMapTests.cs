using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace SSCMS.Web.Tests.Security
{
    public class PublicSourceMapTests
    {
        [Fact]
        public void WwwrootDoesNotPublishSourceMaps()
        {
            var wwwroot = FindRepositoryDirectory("src/SSCMS.Web/wwwroot");

            var mapFiles = Directory.GetFiles(wwwroot, "*.map", SearchOption.AllDirectories);

            Assert.Empty(mapFiles.Select(path => Path.GetRelativePath(wwwroot, path)));
        }

        [Fact]
        public void PublicAssetsDoNotReferenceSourceMaps()
        {
            var wwwroot = FindRepositoryDirectory("src/SSCMS.Web/wwwroot");

            var references = Directory
                .GetFiles(wwwroot, "*.*", SearchOption.AllDirectories)
                .Where(path => path.EndsWith(".js", StringComparison.OrdinalIgnoreCase) ||
                               path.EndsWith(".css", StringComparison.OrdinalIgnoreCase))
                .Where(ReferencesExistingSourceMap)
                .Select(path => Path.GetRelativePath(wwwroot, path))
                .ToList();

            Assert.Empty(references);
        }

        private static bool ReferencesExistingSourceMap(string path)
        {
            var source = File.ReadAllText(path);
            var match = Regex.Match(source, @"sourceMappingURL=(?<url>[^\s*]+)");
            if (!match.Success) return false;

            var sourceMapPath = Path.Combine(Path.GetDirectoryName(path)!, match.Groups["url"].Value);
            return File.Exists(sourceMapPath);
        }

        private static string FindRepositoryDirectory(string relativePath)
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);
            while (directory != null)
            {
                var candidate = Path.Combine(directory.FullName, relativePath);
                if (Directory.Exists(candidate))
                {
                    return candidate;
                }

                directory = directory.Parent;
            }

            throw new DirectoryNotFoundException(relativePath);
        }
    }
}
