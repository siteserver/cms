using System;
using System.IO;
using Xunit;

namespace SSCMS.Web.Tests.Security
{
    public class UeditorXssFilterConfigTests
    {
        [Fact]
        public void UeditorEnablesXssFilteringByDefault()
        {
            var source = File.ReadAllText(FindRepositoryFile("src/SSCMS.Web/wwwroot/sitefiles/assets/lib/ueditor/editor_config.js"));

            Assert.Contains("xssFilterRules: true", source, StringComparison.Ordinal);
            Assert.Contains("inputXssFilter: true", source, StringComparison.Ordinal);
            Assert.Contains("outputXssFilter: true", source, StringComparison.Ordinal);
            Assert.DoesNotContain("xssFilterRules: false", source, StringComparison.Ordinal);
            Assert.DoesNotContain("inputXssFilter: false", source, StringComparison.Ordinal);
            Assert.DoesNotContain("outputXssFilter: false", source, StringComparison.Ordinal);
        }

        [Fact]
        public void UeditorDoesNotPublishServerConfigSamples()
        {
            Assert.False(File.Exists(FindRepositoryPath("src/SSCMS.Web/wwwroot/sitefiles/assets/lib/ueditor/config.json")));
            Assert.False(File.Exists(FindRepositoryPath("src/SSCMS.Web/wwwroot/sitefiles/assets/lib/ueditor/index.html")));
        }

        private static string FindRepositoryFile(string relativePath)
        {
            var candidate = FindRepositoryPath(relativePath);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            throw new FileNotFoundException(relativePath);
        }

        private static string FindRepositoryPath(string relativePath)
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);
            while (directory != null)
            {
                var candidate = Path.Combine(directory.FullName, relativePath);
                var parent = Path.GetDirectoryName(candidate);
                if (File.Exists(candidate) || (parent != null && Directory.Exists(parent)))
                {
                    return candidate;
                }

                directory = directory.Parent;
            }

            throw new DirectoryNotFoundException(relativePath);
        }
    }
}
