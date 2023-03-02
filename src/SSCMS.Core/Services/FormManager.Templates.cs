using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class FormManager
    {
        public const string MAIN_FILE_NAME = "index.html";

        private string GetTemplatesDirectoryPath()
        {
            return _pathManager.GetSiteFilesPath("assets/forms");
        }

        private async Task<string> GetTemplatesDirectoryPathAsync(Site site)
        {
            return await _pathManager.GetSitePathAsync(site, "forms");
        }

        public async Task<string> GetTemplateDirectoryPathAsync(Site site, bool isSystem, string name)
        {
            if (isSystem)
            {
                var directoryPath = GetTemplatesDirectoryPath();
                return PathUtils.Combine(directoryPath, name);
            }
            else
            {
                var directoryPath = await GetTemplatesDirectoryPathAsync(site);
                return PathUtils.Combine(directoryPath, name);
            }
        }

        public async Task<string> GetTemplateHtmlAsync(Site site, bool isSystem, string name)
        {
            if (isSystem)
            {
                var directoryPath = GetTemplatesDirectoryPath();
                var htmlPath = PathUtils.Combine(directoryPath, name, MAIN_FILE_NAME);
                return _pathManager.GetContentByFilePath(htmlPath);
            }
            else
            {
                var directoryPath = await GetTemplatesDirectoryPathAsync(site);
                var htmlPath = PathUtils.Combine(directoryPath, name, MAIN_FILE_NAME);
                return _pathManager.GetContentByFilePath(htmlPath);
            }
        }

        public async Task SetTemplateHtmlAsync(Site site, string name, string html)
        {
            var directoryPath = await GetTemplatesDirectoryPathAsync(site);
            var htmlPath = PathUtils.Combine(directoryPath, name, MAIN_FILE_NAME);
            FileUtils.WriteText(htmlPath, html);
        }

        public async Task DeleteTemplateAsync(Site site, string name)
        {
            if (string.IsNullOrEmpty(name)) return;

            var directoryPath = await GetTemplatesDirectoryPathAsync(site);
            var templatePath = PathUtils.Combine(directoryPath, name);
            DirectoryUtils.DeleteDirectoryIfExists(templatePath);
        }

        public async Task<List<FormTemplate>> GetFormTemplatesAsync(Site site)
        {
            var templates = new List<FormTemplate>();

            var directoryPath = await GetTemplatesDirectoryPathAsync(site);
            if (DirectoryUtils.IsDirectoryExists(directoryPath))
            {
                var directoryNames = DirectoryUtils.GetDirectoryNames(directoryPath);
                foreach (var directoryName in directoryNames)
                {
                    var template = GetFormTemplate(directoryPath, false, directoryName);
                    if (template == null) continue;
                    templates.Add(template);
                }
            }

            directoryPath = GetTemplatesDirectoryPath();
            if (DirectoryUtils.IsDirectoryExists(directoryPath))
            {
                var directoryNames = DirectoryUtils.GetDirectoryNames(directoryPath);
                foreach (var directoryName in directoryNames)
                {
                    var template = GetFormTemplate(directoryPath, true, directoryName);
                    if (template == null) continue;
                    templates.Add(template);
                }
            }

            return templates;
        }

        public async Task<FormTemplate> GetFormTemplateAsync(Site site, string name)
        {
            var directoryPath = await GetTemplatesDirectoryPathAsync(site);
            var template = GetFormTemplate(directoryPath, false, name);
            if (template != null)
            {
                return template;
            }

            directoryPath = GetTemplatesDirectoryPath();
            return GetFormTemplate(directoryPath, true, name);
        }

        private FormTemplate GetFormTemplate(string templatesDirectoryPath, bool isSystem, string name)
        {
            if (!FileUtils.IsFileExists(PathUtils.Combine(templatesDirectoryPath, name, MAIN_FILE_NAME)))
            {
                return null;
            }

            return new FormTemplate
            {
                IsSystem = isSystem,
                Name = name,
            };
        }

        public async Task CloneAsync(Site site, bool isSystemOriginal, string nameOriginal, string name)
        {
            var directoryPathSite = await GetTemplatesDirectoryPathAsync(site);
            var directoryPathToClone = isSystemOriginal ? GetTemplatesDirectoryPath() : directoryPathSite;

            DirectoryUtils.Copy(PathUtils.Combine(directoryPathToClone, nameOriginal), PathUtils.Combine(directoryPathSite, name), true);
        }

        public async Task CloneAsync(Site site, bool isSystemOriginal, string nameOriginal, string name, string templateHtml)
        {
            await CloneAsync(site, isSystemOriginal, nameOriginal, name);
            await SetTemplateHtmlAsync(site, name, templateHtml);
        }
    }
}