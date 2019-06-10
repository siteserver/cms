using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using SS.CMS.Utils;
using AppContext = SS.CMS.Core.Settings.AppContext;

namespace SS.CMS.Api.Tests
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

            AppContext.Load(apiDirectoryPath, PathUtils.Combine(apiDirectoryPath, DirectoryUtils.WwwRoot.DirectoryName), config);
        }

        public void Dispose()
        {
            // ... clean up test data from the database ...
        }
    }
}
