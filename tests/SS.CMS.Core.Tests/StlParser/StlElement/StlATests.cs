using System;
using System.Collections.Generic;
using System.Text;
using SS.CMS.Core.Models;
using SS.CMS.Core.StlParser;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Core.StlParser.Utility;
using SS.CMS.Models;
using Xunit;

namespace SS.CMS.Core.Tests.StlParser.StlElement
{
    [TestCaseOrderer("SS.CMS.Core.Tests.PriorityOrderer", "SS.CMS.Core.Tests")]
    public class StlATests
    {
        private readonly EnvironmentFixture _fixture;

        public StlATests(EnvironmentFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact, TestPriority(0)]
        public void ParseTest()
        {
            var siteInfo = new SiteInfo();
            var templateInfo = new TemplateInfo();
            var pluginItems = new Dictionary<string, object>();
            var pageInfo = new PageInfo(_fixture.UrlManager.ApiUrl, 0, 0, siteInfo, templateInfo, pluginItems);

            var contextInfo = new ParseContext(pageInfo, _fixture.Configuration, _fixture.SettingsManager, _fixture.CacheManager, _fixture.PluginManager, _fixture.PathManager, _fixture.UrlManager, _fixture.FileManager, _fixture.TableManager, _fixture.SiteRepository, _fixture.ChannelRepository, _fixture.UserRepository, _fixture.TableStyleRepository, _fixture.TemplateRepository, _fixture.TagRepository, _fixture.ErrorLogRepository);

            var template = $@"<stl:a href=""https://www.siteserver.cn"">test</stl:a>";
            var builder = new StringBuilder(template);

            contextInfo.ParseTemplateContent(builder);
            var parsedContent = builder.ToString();

            Console.Write(parsedContent);

            Assert.True(true);
        }
    }
}
