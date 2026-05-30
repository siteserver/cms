using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;
using System.Text.RegularExpressions;
using SSCMS.Configuration;

namespace SSCMS.Core.Utils
{
    public static class SafeZipUtils
    {
        public static void ExtractZip(string zipFilePath, string directoryPath, string fileFilter = null, long maxExtractedBytes = Constants.MaxUploadRequestSize)
        {
            var destinationRoot = Path.GetFullPath(directoryPath);
            Directory.CreateDirectory(destinationRoot);
            long extractedBytes = 0;

            using var fileStream = File.OpenRead(zipFilePath);
            using var zipFile = new ZipFile(fileStream);
            foreach (ZipEntry entry in zipFile)
            {
                if (!ShouldExtract(entry.Name, fileFilter))
                {
                    continue;
                }

                var destinationPath = GetDestinationPath(destinationRoot, entry.Name);
                if (entry.IsDirectory)
                {
                    Directory.CreateDirectory(destinationPath);
                    continue;
                }

                if (!entry.IsFile)
                {
                    continue;
                }

                if (entry.Size < 0)
                {
                    throw new InvalidOperationException($"Zip entry '{entry.Name}' has an unknown extracted size.");
                }

                extractedBytes += entry.Size;
                if (extractedBytes > maxExtractedBytes)
                {
                    throw new InvalidOperationException("Zip archive exceeds the maximum extracted size.");
                }

                var destinationDirectory = Path.GetDirectoryName(destinationPath);
                if (!string.IsNullOrEmpty(destinationDirectory))
                {
                    Directory.CreateDirectory(destinationDirectory);
                }

                using var inputStream = zipFile.GetInputStream(entry);
                using var outputStream = File.Create(destinationPath);
                inputStream.CopyTo(outputStream);
            }
        }

        private static bool ShouldExtract(string entryName, string fileFilter)
        {
            return string.IsNullOrEmpty(fileFilter) || Regex.IsMatch(entryName, fileFilter);
        }

        private static string GetDestinationPath(string destinationRoot, string entryName)
        {
            if (string.IsNullOrWhiteSpace(entryName))
            {
                throw new InvalidOperationException("Zip entry name cannot be empty.");
            }

            var normalizedEntryName = entryName.Replace('\\', '/');
            if (normalizedEntryName.StartsWith("/", StringComparison.Ordinal) ||
                Regex.IsMatch(normalizedEntryName, "^[A-Za-z]:/"))
            {
                throw new InvalidOperationException($"Zip entry '{entryName}' is outside the destination directory.");
            }

            var destinationPath = Path.GetFullPath(Path.Combine(
                destinationRoot,
                normalizedEntryName.Replace('/', Path.DirectorySeparatorChar)));

            if (!IsInsideDirectory(destinationRoot, destinationPath))
            {
                throw new InvalidOperationException($"Zip entry '{entryName}' is outside the destination directory.");
            }

            return destinationPath;
        }

        private static bool IsInsideDirectory(string directoryPath, string filePath)
        {
            return string.Equals(directoryPath, filePath, StringComparison.OrdinalIgnoreCase) ||
                   filePath.StartsWith(EnsureTrailingSeparator(directoryPath), StringComparison.OrdinalIgnoreCase);
        }

        private static string EnsureTrailingSeparator(string directoryPath)
        {
            return directoryPath.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal)
                ? directoryPath
                : directoryPath + Path.DirectorySeparatorChar;
        }
    }
}
