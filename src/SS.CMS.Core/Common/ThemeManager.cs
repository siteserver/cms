using System.Collections.Generic;
using System.Linq;
using SS.CMS.Models;
using SS.CMS.Services;
using SS.CMS.Utils;
using SS.CMS.Utils.Enumerations;

namespace SS.CMS.Core.Common
{
    public class ThemeManager
    {
        private readonly IPathManager _pathManager;
        private readonly string _rootPath;

        public ThemeManager(IPathManager pathManager)
        {
            _rootPath = pathManager.GetThemesPath();
            DirectoryUtils.CreateDirectoryIfNotExists(_rootPath);
        }

        public List<PackageInfo> GetThemeInfoList()
        {
            var list = new List<PackageInfo>();
            var directoryPaths = DirectoryUtils.GetDirectoryPaths(_rootPath);
            foreach (var themePath in directoryPaths)
            {
                var packageJsonFilePath = PathUtils.Combine(themePath, "package.json");
                if (FileUtils.IsFileExists(packageJsonFilePath))
                {
                    var content = FileUtils.ReadText(packageJsonFilePath);
                    var packageInfo = TranslateUtils.JsonDeserialize<PackageInfo>(content);
                    if (packageInfo != null)
                    {
                        packageInfo.Name = PathUtils.GetDirectoryName(themePath, false);
                        list.Add(packageInfo);
                    }
                }
            }

            return list.OrderBy(x => x.Name).ToList();
        }

        public List<string> GetZipThemeList()
        {
            var list = new List<string>();
            foreach (var fileName in DirectoryUtils.GetFileNames(_rootPath))
            {
                if (EFileSystemTypeUtils.IsZip(PathUtils.GetExtension(fileName)))
                {
                    list.Add(fileName);
                }
            }
            return list;
        }
    }
}