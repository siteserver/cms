using System;
using System.IO;
using Xunit;

namespace SSCMS.Web.Tests.Security
{
    public class CloudTokenStorageTests
    {
        [Fact]
        public void CloudAccessTokenIsNotPersistedInLocalStorage()
        {
            var cloudJsPath = FindRepositoryFile("src/SSCMS.Web/wwwroot/sitefiles/assets/js/cloud.js");
            var source = File.ReadAllText(cloudJsPath);

            Assert.DoesNotContain("localStorage.getItem(CLOUD_ACCESS_TOKEN_NAME)", source, StringComparison.Ordinal);
            Assert.DoesNotContain("localStorage.setItem(CLOUD_ACCESS_TOKEN_NAME", source, StringComparison.Ordinal);
            Assert.Contains("sessionStorage.getItem(CLOUD_ACCESS_TOKEN_NAME)", source, StringComparison.Ordinal);
            Assert.Contains("sessionStorage.setItem(CLOUD_ACCESS_TOKEN_NAME", source, StringComparison.Ordinal);
        }

        private static string FindRepositoryFile(string relativePath)
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);
            while (directory != null)
            {
                var candidate = Path.Combine(directory.FullName, relativePath);
                if (File.Exists(candidate))
                {
                    return candidate;
                }

                directory = directory.Parent;
            }

            throw new FileNotFoundException(relativePath);
        }
    }
}
