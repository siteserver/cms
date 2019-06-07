using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using SS.CMS.Data;
using SS.CMS.Utils;

namespace SS.CMS.Core.Tests
{
    public class EnvironmentFixture : IDisposable
    {
        public EnvironmentFixture()
        {
            var rootDirectoryPath = DirectoryUtils.GetParentPath(Directory.GetCurrentDirectory(), 5);
            var apiDirectoryPath = PathUtils.Combine(rootDirectoryPath, "src", "SS.CMS.Api");

            var config = new ConfigurationBuilder()
                .SetBasePath(apiDirectoryPath)
                .AddJsonFile("appSettings.json")
                .Build();

            AppSettings.Load(apiDirectoryPath, PathUtils.Combine(apiDirectoryPath, DirectoryUtils.WwwRoot.DirectoryName), config);
        }

        public void Dispose()
        {
            // ... clean up test data from the database ...
        }
    }
}
