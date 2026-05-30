using System;
using System.IO;
using Xunit;

namespace SSCMS.Web.Tests.Security
{
    public class SecurityHeadersTests
    {
        [Fact]
        public void StartupAddsBaselineSecurityHeaders()
        {
            var startupPath = FindRepositoryFile("src/SSCMS.Web/Startup.cs");
            var source = File.ReadAllText(startupPath);

            Assert.Contains("X-Content-Type-Options", source, StringComparison.Ordinal);
            Assert.Contains("nosniff", source, StringComparison.Ordinal);
            Assert.Contains("X-Frame-Options", source, StringComparison.Ordinal);
            Assert.Contains("SAMEORIGIN", source, StringComparison.Ordinal);
            Assert.Contains("Content-Security-Policy", source, StringComparison.Ordinal);
            Assert.Contains("frame-ancestors 'self'", source, StringComparison.Ordinal);
            Assert.Contains("Referrer-Policy", source, StringComparison.Ordinal);
            Assert.Contains("Permissions-Policy", source, StringComparison.Ordinal);
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
