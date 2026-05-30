using System;
using System.IO;
using System.IO.Compression;
using SSCMS.Core.Utils;
using SSCMS.Core.Services;
using Xunit;

namespace SSCMS.Web.Tests.Security
{
    public class ZipExtractionSafetyTests
    {
        [Fact]
        public void ExtractZipAllowsEntriesInsideDestination()
        {
            var basePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            var zipPath = Path.Combine(basePath, "safe.zip");
            var extractPath = Path.Combine(basePath, "extract");
            var extractedPath = Path.Combine(extractPath, "assets", "style.css");
            Directory.CreateDirectory(basePath);
            Directory.CreateDirectory(extractPath);
            try
            {
                using (var archive = ZipFile.Open(zipPath, ZipArchiveMode.Create))
                {
                    var entry = archive.CreateEntry("assets/style.css");
                    using var writer = new StreamWriter(entry.Open());
                    writer.Write("body{}");
                }

                var pathManager = new PathManager(null, null, null, null, null, null, null, null, null, null, null);

                pathManager.ExtractZip(zipPath, extractPath);

                Assert.Equal("body{}", File.ReadAllText(extractedPath));
            }
            finally
            {
                if (Directory.Exists(basePath))
                {
                    Directory.Delete(basePath, true);
                }
            }
        }

        [Fact]
        public void ExtractZipRejectsArchivesOverExpandedSizeLimit()
        {
            var basePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            var zipPath = Path.Combine(basePath, "large.zip");
            var extractPath = Path.Combine(basePath, "extract");
            var extractedPath = Path.Combine(extractPath, "large.txt");
            Directory.CreateDirectory(basePath);
            Directory.CreateDirectory(extractPath);
            try
            {
                using (var archive = ZipFile.Open(zipPath, ZipArchiveMode.Create))
                {
                    var entry = archive.CreateEntry("large.txt");
                    using var writer = new StreamWriter(entry.Open());
                    writer.Write(new string('A', 32));
                }

                Assert.Throws<InvalidOperationException>(() =>
                    SafeZipUtils.ExtractZip(zipPath, extractPath, maxExtractedBytes: 16));
                Assert.False(File.Exists(extractedPath));
            }
            finally
            {
                if (Directory.Exists(basePath))
                {
                    Directory.Delete(basePath, true);
                }
            }
        }

        [Fact]
        public void ExtractZipRejectsEntriesOutsideDestination()
        {
            var basePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            var zipPath = Path.Combine(basePath, "malicious.zip");
            var extractPath = Path.Combine(basePath, "extract");
            var outsidePath = Path.Combine(basePath, "outside.txt");
            Directory.CreateDirectory(basePath);
            Directory.CreateDirectory(extractPath);
            try
            {
                using (var archive = ZipFile.Open(zipPath, ZipArchiveMode.Create))
                {
                    var entry = archive.CreateEntry("../outside.txt");
                    using var writer = new StreamWriter(entry.Open());
                    writer.Write("escaped");
                }

                var pathManager = new PathManager(null, null, null, null, null, null, null, null, null, null, null);

                Assert.Throws<InvalidOperationException>(() => pathManager.ExtractZip(zipPath, extractPath));
                Assert.False(File.Exists(outsidePath));
            }
            finally
            {
                if (Directory.Exists(basePath))
                {
                    Directory.Delete(basePath, true);
                }
            }
        }
    }
}
