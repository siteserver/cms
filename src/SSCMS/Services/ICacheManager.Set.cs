namespace SSCMS.Services
{
    public partial interface ICacheManager
    {
        void AddOrUpdateSliding<T>(string key, T value, int minutes);

        void AddOrUpdateAbsolute<T>(string key, T value, int minutes);

        void AddOrUpdate<T>(string key, T value);
    }
}
