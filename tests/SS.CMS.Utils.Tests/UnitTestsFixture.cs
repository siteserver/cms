using System.IO;
using Microsoft.Extensions.Configuration;
using SS.CMS.Core.Services;
using SS.CMS.Services;

namespace SS.CMS.Utils.Tests
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
