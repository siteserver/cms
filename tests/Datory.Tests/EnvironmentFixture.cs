using System;
using System.IO;
using System.Reflection;

namespace Datory.Tests
{
    public class EnvironmentFixture : IDisposable
    {
        public string ApplicationPhysicalPath { get; }

        public EnvironmentFixture()
        {
            var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
            var dirPath = Path.GetDirectoryName(codeBasePath);
            ApplicationPhysicalPath = Path.Combine(GetParentPath(GetParentPath(GetParentPath(GetParentPath(dirPath)))), "SiteServer.Web");

            EnvUtils.Load(ApplicationPhysicalPath, Path.Combine(ApplicationPhysicalPath, EnvUtils.WebConfigFileName));
        }

        private static string GetParentPath(string path)
        {
            return Directory.GetParent(path).FullName;
        }

        public void Dispose()
        {
            // ... clean up test data from the database ...
        }
    }
}
