using System.IO;
using SSCMS.Tests;
using SSCMS.Core.Utils.Office;
using SSCMS.Utils;
using Xunit;

namespace SSCMS.Core.Tests.Common.Office
{
    public class WordManagerTests : IClassFixture<UnitTestsFixture>
    {
        private readonly UnitTestsFixture _fixture;

        public WordManagerTests(UnitTestsFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void WordParseTest()
        {
            var projDirectoryPath = _fixture.SettingsManager.ContentRootPath;

            var htmlDirectoryPath = PathUtils.Combine(projDirectoryPath, "build");
            var imageDirectoryPath = PathUtils.Combine(htmlDirectoryPath, "images");
            const string imageDirectoryUrl = "images";
            DirectoryUtils.DeleteDirectoryIfExists(htmlDirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(htmlDirectoryPath);

            var wordsDirectory = PathUtils.Combine(projDirectoryPath, "assets/words");

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
