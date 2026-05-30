using System;
using System.IO;
using Xunit;

namespace SSCMS.Web.Tests.Security
{
    public class RateLimitConcurrencySourceTests
    {
        [Theory]
        [InlineData("src/SSCMS.Web/Controllers/Stl/ActionsSearchController.cs")]
        [InlineData("src/SSCMS.Web/Controllers/V1/AdministratorsController.cs")]
        [InlineData("src/SSCMS.Web/Controllers/V1/UsersController.cs")]
        public void RateLimitersSynchronizeCacheReadModifyWrite(string relativePath)
        {
            var source = File.ReadAllText(FindRepositoryFile(relativePath));

            Assert.Contains("lock (RateLimitLockManager.GetLock(cacheKey))", source, StringComparison.Ordinal);
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
