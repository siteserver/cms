namespace SSCMS.Services
{
    public partial interface ICacheManager
    {
        T Get<T>(string key) where T : class;

        string GetByFilePath(string filePath);

        bool Exists(string key);
    }
}
