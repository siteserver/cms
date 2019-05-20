using System;
using System.IO;
using System.Reflection;
using SiteServer.Utils;

namespace SiteServer.API.Tests
{
    public class EnvironmentFixture : IDisposable
    {
        public string ApplicationPhysicalPath { get; }

        public EnvironmentFixture()
        {
            var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
            var testsDirectoryPath = DirectoryUtils.GetParentPath(DirectoryUtils.GetParentPath(DirectoryUtils.GetParentPath(Path.GetDirectoryName(codeBasePath))));

            ApplicationPhysicalPath = PathUtils.Combine(DirectoryUtils.GetParentPath(testsDirectoryPath), "SiteServer.Web");

            AppSettings.Load("/", ApplicationPhysicalPath, PathUtils.Combine(ApplicationPhysicalPath, AppSettings.WebConfigFileName));
        }

        public void Dispose()
        {
            // ... clean up test data from the database ...
        }
    }
}
