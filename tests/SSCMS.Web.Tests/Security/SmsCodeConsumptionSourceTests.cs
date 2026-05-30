using System;
using System.IO;
using Xunit;

namespace SSCMS.Web.Tests.Security
{
    public class SmsCodeConsumptionSourceTests
    {
        [Theory]
        [InlineData("src/SSCMS.Web/Controllers/Admin/LoginController.Submit.cs")]
        [InlineData("src/SSCMS.Web/Controllers/Admin/LostPasswordController.Submit.cs")]
        [InlineData("src/SSCMS.Web/Controllers/Home/LoginController.Submit.cs")]
        [InlineData("src/SSCMS.Web/Controllers/Home/LostPasswordController.Submit.cs")]
        [InlineData("src/SSCMS.Web/Controllers/Home/RegisterController.VerifyMobile.cs")]
        [InlineData("src/SSCMS.Web/Controllers/Home/ProfileController.VerifyMobile.cs")]
        [InlineData("src/SSCMS.Web/Controllers/Home/VerifyMobileController.Submit.cs")]
        [InlineData("src/SSCMS.Web/Controllers/V1/FormsController.Submit.cs")]
        public void SuccessfulSmsCodeVerificationConsumesCode(string relativePath)
        {
            var source = File.ReadAllText(FindRepositoryFile(relativePath));

            Assert.Contains("_cacheManager.Remove(codeCacheKey)", source, StringComparison.Ordinal);
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
