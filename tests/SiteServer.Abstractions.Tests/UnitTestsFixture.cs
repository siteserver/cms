using System.IO;
using Microsoft.Extensions.Configuration;

namespace SiteServer.Abstractions.Tests
{
    public class UnitTestsFixture
    {
        public ISettingsManager SettingsManager { get; }

        public UnitTestsFixture()
        {
            var contentRootPath = Directory.GetCurrentDirectory();

            var config = new ConfigurationBuilder()
                .SetBasePath(contentRootPath)
                .AddJsonFile("ss.json")
                .Build();

            SettingsManager = new SettingsManager(config, contentRootPath, PathUtils.Combine(contentRootPath, "wwwroot"));
        }
    }
}
