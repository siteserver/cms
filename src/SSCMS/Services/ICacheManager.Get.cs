namespace SSCMS.Services
{
    public partial interface ICacheManager
    {
        T Get<T>(string key);

        string GetByFilePath(string filePath);

        bool Exists(string key);
    }
}
