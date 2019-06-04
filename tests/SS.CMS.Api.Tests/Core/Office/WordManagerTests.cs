using System;
using System.IO;
using System.Reflection;
using SS.CMS.Core.Common.Office;
using SS.CMS.Utils;
using Xunit;

namespace SS.CMS.Api.Tests.Core.Office
{
    [TestCaseOrderer("SS.CMS.Api.Tests.PriorityOrderer", "SS.CMS.Api.Tests")]
    public class WordManagerTests : IClassFixture<EnvironmentFixture>
    {
        public EnvironmentFixture Fixture { get; }

        public WordManagerTests(EnvironmentFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact, TestPriority(0)]
        public void WordParseTest()
        {
            var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
            var testsDirectoryPath = DirectoryUtils.GetParentPath(DirectoryUtils.GetParentPath(DirectoryUtils.GetParentPath(Path.GetDirectoryName(codeBasePath))));

            var htmlDirectoryPath = PathUtils.Combine(testsDirectoryPath, "output");
            var imageDirectoryPath = PathUtils.Combine(htmlDirectoryPath, "images");
            const string imageDirectoryUrl = "images";
            DirectoryUtils.DeleteDirectoryIfExists(htmlDirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(htmlDirectoryPath);

            var wordsDirectory = PathUtils.Combine(testsDirectoryPath, "assets/words");

            foreach (var docxFilePath in Directory.GetFiles(wordsDirectory, "*.docx"))
            {
                var settings = new WordManager.ConverterSettings
                {
                    IsSaveHtml = true,
                    HtmlDirectoryPath = htmlDirectoryPath,
                    ImageDirectoryPath = imageDirectoryPath,
                    ImageDirectoryUrl = imageDirectoryUrl
                };

                WordManager.ConvertToHtml(docxFilePath, settings);
            }
            foreach (var file in Directory.GetFiles(htmlDirectoryPath, "*.html"))
            {
                WordManager.ConvertToDocx(file, htmlDirectoryPath);
            }

            Assert.True(true);
        }
    }
}
