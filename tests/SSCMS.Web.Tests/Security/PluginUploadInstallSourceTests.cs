using System;
using System.IO;
using Xunit;

namespace SSCMS.Web.Tests.Security
{
    public class PluginUploadInstallSourceTests
    {
        [Fact]
        public void PluginUploadUsesServerGeneratedStagingPath()
        {
            var uploadSource = File.ReadAllText(FindRepositoryFile("src/SSCMS.Web/Controllers/Admin/Plugins/AddLayerUploadController.Upload.cs"));

            Assert.Contains("Guid.NewGuid().ToString(\"N\")", uploadSource, StringComparison.Ordinal);
            Assert.DoesNotContain("GetTemporaryFilesPath(fileName)", uploadSource, StringComparison.Ordinal);
        }

        [Fact]
        public void PluginInstallUsesValidatedStagingDirectory()
        {
            var uploadSource = File.ReadAllText(FindRepositoryFile("src/SSCMS.Web/Controllers/Admin/Plugins/AddLayerUploadController.Upload.cs"));
            var overrideSource = File.ReadAllText(FindRepositoryFile("src/SSCMS.Web/Controllers/Admin/Plugins/AddLayerUploadController.Override.cs"));

            Assert.DoesNotContain("_pathManager.ExtractZip(filePath, pluginPath)", uploadSource, StringComparison.Ordinal);
            Assert.DoesNotContain("_pathManager.ExtractZip(filePath, pluginPath)", overrideSource, StringComparison.Ordinal);
            Assert.Contains("DirectoryUtils.MoveDirectory(tempPluginPath, pluginPath, true)", uploadSource, StringComparison.Ordinal);
            Assert.Contains("DirectoryUtils.MoveDirectory(tempPluginPath, pluginPath, true)", overrideSource, StringComparison.Ordinal);
            Assert.Contains("PluginUtils.ValidateManifestAsync(tempPluginPath)", overrideSource, StringComparison.Ordinal);
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
