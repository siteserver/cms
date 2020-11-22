using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mono.Options;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Core;
using SSCMS.Cli.Models;
using SSCMS.Core.Utils;
using SSCMS.Core.Utils.Serialization;
using SSCMS.Plugins;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Cli.Jobs
{
    public class ThemePackageJob : IJobService
    {
        public string CommandName => "theme package";

        private string _directory;
        private bool _isHelp;
        private readonly OptionSet _options;

        private readonly IPathManager _pathManager;
        private readonly ICacheManager _cacheManager;
        private readonly IDatabaseManager _databaseManager;

        public ThemePackageJob(IPathManager pathManager, ICacheManager cacheManager, IDatabaseManager databaseManager)
        {
            _options = new OptionSet
            {
                { "d|directory=", "site directory path",
                    v => _directory = v },
                {
                    "h|help", "Display help",
                    v => _isHelp = v != null
                }
            };

            _pathManager = pathManager;
            _cacheManager = cacheManager;
            _databaseManager = databaseManager;
        }

        public void PrintUsage()
        {
            Console.WriteLine($"Usage: sscms {CommandName}");
            Console.WriteLine("Summary: package theme to zip file");
            Console.WriteLine("Options:");
            _options.WriteOptionDescriptions(Console.Out);
            Console.WriteLine();
        }

        public async Task ExecuteAsync(IPluginJobContext context)
        {
            if (!CliUtils.ParseArgs(_options, context.Args)) return;

            if (_isHelp)
            {
                PrintUsage();
                return;
            }

            var (success, _, filePath) =
                await PackageAsync(_pathManager, _cacheManager, _databaseManager, _directory, true);
            if (success)
            {
                var fileSize = FileUtils.GetFileSizeByFilePath(filePath);
                await WriteUtils.PrintSuccessAsync($"Theme packaged: {filePath} ({fileSize})");
            }
        }

        public static async Task<(bool Success, string name, string filePath)> PackageAsync(IPathManager pathManager, ICacheManager cacheManager, IDatabaseManager databaseManager, string directory, bool isOverride)
        {
            var site = await databaseManager.SiteRepository.GetSiteByDirectoryAsync(directory);
            var sitePath = await pathManager.GetSitePathAsync(site);

            if (site == null || !DirectoryUtils.IsDirectoryExists(sitePath))
            {
                await WriteUtils.PrintErrorAsync($@"Invalid site directory path: ""{directory}""");
                return (false, null, null);
            }

            var readme = string.Empty;
            Theme theme = null;

            var readmePath = PathUtils.Combine(sitePath, "README.md");
            if (FileUtils.IsFileExists(readmePath))
            {
                readme = FileUtils.ReadText(readmePath);
                var yaml = MarkdownUtils.GetYamlFrontMatter(readme);
                if (!string.IsNullOrEmpty(yaml))
                {
                    readme = MarkdownUtils.RemoveYamlFrontMatter(readme);
                    theme = YamlUtils.Deserialize<Theme>(yaml);
                }
            }

            var writeReadme = false;
            if (theme == null || string.IsNullOrEmpty(theme.Name) || string.IsNullOrEmpty(theme.CoverUrl))
            {
                writeReadme = true;
                theme = new Theme
                {
                    Name = ReadUtils.GetString("name:"),
                    CoverUrl = ReadUtils.GetString("cover image url:"),
                    Summary = ReadUtils.GetString("repository url:"),
                    Tags = ReadUtils.GetStringList("tags:"),
                    ThumbUrls = ReadUtils.GetStringList("thumb image urls:"),
                    Compatibilities = ReadUtils.GetStringList("compatibilities:"),
                    Price = ReadUtils.GetYesNo("is free?") ? 0 : ReadUtils.GetDecimal("price:"),
                };
            }

            if (writeReadme)
            {
                readme = @$"---
{YamlUtils.Serialize(theme)}
---

" + readme;
                FileUtils.WriteText(readmePath, readme);
            }

            var packageName = "T_" + theme.Name.Replace(" ", "_");
            var packagePath = pathManager.GetSiteTemplatesPath(packageName);
            var fileName = packageName + ".zip";
            var filePath = pathManager.GetSiteTemplatesPath(fileName);

            if (!isOverride && FileUtils.IsFileExists(filePath))
            {
                return (true, theme.Name, filePath);
            }

            FileUtils.DeleteFileIfExists(filePath);
            DirectoryUtils.DeleteDirectoryIfExists(packagePath);

            await Console.Out.WriteLineAsync($"Theme name: {theme.Name}");
            await Console.Out.WriteLineAsync($"Theme folder: {packagePath}");
            await Console.Out.WriteLineAsync("Theme packaging...");

            var caching = new CacheUtils(cacheManager);
            var manager = new SiteTemplateManager(pathManager, databaseManager, caching);

            if (manager.IsSiteTemplateDirectoryExists(packageName))
            {
                manager.DeleteSiteTemplate(packageName);
            }

            var directoryNames = DirectoryUtils.GetDirectoryNames(sitePath);

            var directories = new List<string>();
            var siteDirList = await databaseManager.SiteRepository.GetSiteDirsAsync(0);
            foreach (var directoryName in directoryNames)
            {
                var isSiteDirectory = false;
                if (site.Root)
                {
                    foreach (var siteDir in siteDirList)
                    {
                        if (StringUtils.EqualsIgnoreCase(siteDir, directoryName))
                        {
                            isSiteDirectory = true;
                        }
                    }
                }
                if (!isSiteDirectory && !pathManager.IsSystemDirectory(directoryName))
                {
                    directories.Add(directoryName);
                }
            }

            var files = DirectoryUtils.GetFileNames(sitePath);

            var exportObject = new ExportObject(pathManager, databaseManager, caching, site);
            await exportObject.ExportFilesToSiteAsync(packagePath, true, directories, files, true);

            var siteContentDirectoryPath = pathManager.GetSiteTemplateMetadataPath(packagePath, DirectoryUtils.SiteFiles.SiteTemplates.SiteContent);

            await exportObject.ExportSiteContentAsync(siteContentDirectoryPath, true, true, new List<int>());

            await SiteTemplateManager.ExportSiteToSiteTemplateAsync(pathManager, databaseManager, caching, site, packageName);

            var siteTemplateInfo = new SiteTemplateInfo
            {
                SiteTemplateName = theme.Name,
                PicFileName = string.Empty,
                WebSiteUrl = string.Empty,
                Description = string.Empty
            };
            var xmlPath = pathManager.GetSiteTemplateMetadataPath(packagePath,
                DirectoryUtils.SiteFiles.SiteTemplates.FileMetadata);
            XmlUtils.SaveAsXml(siteTemplateInfo, xmlPath);

            pathManager.CreateZip(filePath, packagePath);

            return (true, theme.Name, filePath);
        }
    }
}
