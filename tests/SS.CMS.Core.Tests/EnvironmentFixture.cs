using System;
using System.IO;
using System.Reflection;
using SS.CMS.Utils;

namespace SS.CMS.Core.Tests
{
    public class EnvironmentFixture : IDisposable
    {
        public string ContentRootPath { get; }

        public EnvironmentFixture()
        {
            var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
            var testsDirectoryPath = DirectoryUtils.GetParentPath(DirectoryUtils.GetParentPath(DirectoryUtils.GetParentPath(Path.GetDirectoryName(codeBasePath))));

            ContentRootPath = PathUtils.Combine(DirectoryUtils.GetParentPath(testsDirectoryPath), "SS.CMS.Api");
            var webRootPath = PathUtils.Combine(ContentRootPath, DirectoryUtils.WwwRoot.DirectoryName);
            var appSettingsPath = PathUtils.Combine(ContentRootPath, AppSettings.AppSettingsFileName);

            AppSettings.LoadJson(ContentRootPath, webRootPath, appSettingsPath);
        }

        public void Dispose()
        {
            // ... clean up test data from the database ...
        }
    }
}
