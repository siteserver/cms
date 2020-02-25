namespace SS.CMS.Abstractions
{
    public partial interface IPathManager
    {
        string GetSiteFilesPath(params string[] paths);

        string GetSiteFilesUrl(params string[] paths);

        string GetAdministratorUploadPath(int userId, params string[] paths);

        string GetAdministratorUploadUrl(int userId, params string[] paths);

        string GetUserUploadPath(int userId, params string[] paths);

        string GetUserUploadUrl(int userId, params string[] paths);

        string GetHomeUploadPath(params string[] paths);

        string GetHomeUploadUrl(params string[] paths);

        string GetTemporaryFilesPath(params string[] paths);

        string GetTemporaryFilesUrl(params string[] paths);
    }
}
