namespace SS.CMS.Services.IPathManager
{
    public partial interface IPathManager
    {
        string GetAdminPath(params string[] paths);

        string GetHomePath(params string[] paths);

        string GetBackupFilesPath(params string[] paths);

        string GetTemporaryFilesPath(params string[] paths);

        string GetSiteTemplatesPath(params string[] paths);

        bool IsSystemDirectory(string directoryName);

        bool IsWebSiteDirectory(string directoryName);

        string AddVirtualToPath(string path);

        string MapPath(string directoryPath, string virtualPath);

        string GetSiteTemplateMetadataPath(string siteTemplatePath, string relatedPath);

        bool IsSystemFile(string fileName);

        bool IsSystemFileForChangeSiteType(string fileName);

        bool IsWebSiteFile(string fileName);

        string MapContentRootPath(string virtualPath);

        string MapWebRootPath(string virtualPath);

        string GetSiteFilesPath(params string[] paths);

        string PluginsPath { get; }

        string GetPluginPath(string pluginId, params string[] paths);

        string GetPluginNuspecPath(string pluginId);

        string GetPluginDllDirectoryPath(string pluginId);

        string GetPackagesPath(params string[] paths);

        string GetHomeUploadPath(params string[] paths);

        string GetUserUploadPath(int userId, string relatedPath);

        string GetUserUploadFileName(string filePath);
    }
}
