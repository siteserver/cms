using System;
using System.IO;
using Xunit;

namespace SSCMS.Web.Tests.Security
{
    public class StartupAuthenticationTests
    {
        [Fact]
        public void JwtBearerDoesNotAcceptAccessTokenFromQueryString()
        {
            var startupPath = FindRepositoryFile("src/SSCMS.Web/Startup.cs");
            var source = File.ReadAllText(startupPath);

            Assert.DoesNotContain("access_token", source, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("OnMessageReceived", source, StringComparison.Ordinal);
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
