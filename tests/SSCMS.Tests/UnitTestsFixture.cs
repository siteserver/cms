using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using SSCMS.Core.Services;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Tests
{
    public class UnitTestsFixture
    {
        public ISettingsManager SettingsManager { get; }

        public UnitTestsFixture()
        {
            var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
            var dirPath = Path.GetDirectoryName(codeBasePath);

            var contentRootPath = DirectoryUtils.GetParentPath(DirectoryUtils.GetParentPath(DirectoryUtils.GetParentPath(dirPath)));

            //var contentRootPath = Directory.GetCurrentDirectory();

            var config = new ConfigurationBuilder()
                .SetBasePath(contentRootPath)
                .AddJsonFile("sscms.json")
                .Build();

            SettingsManager = new SettingsManager(config, contentRootPath, PathUtils.Combine(contentRootPath, "wwwroot"), null);
        }
    }
}
