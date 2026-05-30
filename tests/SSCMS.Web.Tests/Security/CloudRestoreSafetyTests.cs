using System;
using System.IO;
using SSCMS.Core.Services;
using Xunit;

namespace SSCMS.Web.Tests.Security
{
    public class CloudRestoreSafetyTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("../backup")]
        [InlineData("backup/../other")]
        [InlineData("/backup")]
        [InlineData("backup\\other")]
        public void BackupPrefixRejectsUnsafeBackupIds(string backupId)
        {
            Assert.Throws<ArgumentException>(() => CloudManager.GetBackupPrefixKey(1, backupId));
        }

        [Theory]
        [InlineData("2026-04-12T10:20:30.000Z")]
        [InlineData("70b38f98-2c13-4560-9a2f-6207be9bf0be")]
        public void BackupPrefixAllowsSingleSegmentBackupIds(string backupId)
        {
            var prefix = CloudManager.GetBackupPrefixKey(1, backupId);

            Assert.Equal($"backups/1/{backupId}/", prefix);
        }

        [Fact]
        public void CloudRestoreValidatesObjectKeyBeforeCombiningWithRootPath()
        {
            var source = File.ReadAllText(FindRepositoryFile("src/SSCMS.Core/Services/CloudManager.Restore.cs"));

            Assert.Contains("if (!IsSafeBackupFileKey(key))", source, StringComparison.Ordinal);
            Assert.Contains("var filePath = PathUtils.Combine(rootPath, key);", source, StringComparison.Ordinal);
        }

        private static string FindRepositoryFile(string relativePath)
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);
            while (directory != null)
            {
                var candidate = Path.Combine(directory.FullName, relativePath);
                if (File.Exists(candidate))
                {
                    return candidate;
                }

                directory = directory.Parent;
            }

            throw new FileNotFoundException(relativePath);
        }
    }
}
