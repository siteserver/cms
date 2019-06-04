using System;
using System.Collections.Generic;
using System.Text;
using SS.CMS.Core.Models;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Core.StlParser.Utility;
using Xunit;

namespace SS.CMS.Api.Tests.StlParser.StlElement
{
    [TestCaseOrderer("SS.CMS.Api.Tests.PriorityOrderer", "SS.CMS.Api.Tests")]
    public class StlATests
    {
        [Fact, TestPriority(0)]
        public void ParseTest()
        {
            var siteInfo = new SiteInfo();
            var templateInfo = new TemplateInfo();
            var pluginItems = new Dictionary<string, object>();
            var pageInfo = new PageInfo(0, 0, siteInfo, templateInfo, pluginItems);
            var contextInfo = new ContextInfo(pageInfo);

            var template = $@"<stl:a href=""https://www.siteserver.cn"">test</stl:a>";
            var builder = new StringBuilder(template);
            StlParserManager.ParseTemplateContent(builder, pageInfo, contextInfo);
            var parsedContent = builder.ToString();

            Console.Write(parsedContent);

            Assert.True(true);
        }
    }
}
