using System;
using System.IO;
using System.Reflection;

namespace SS.CMS.Plugin.Tests
{
    public class EnvironmentFixture : IDisposable
    {
        public string ApplicationPhysicalPath { get; }

        public EnvironmentFixture()
        {
            var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
            var dirPath = Path.GetDirectoryName(codeBasePath);
            ApplicationPhysicalPath = GetParentPath(GetParentPath(GetParentPath(dirPath)));

            EnvUtils.Load(Path.Combine(ApplicationPhysicalPath, "Web.config"));
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
