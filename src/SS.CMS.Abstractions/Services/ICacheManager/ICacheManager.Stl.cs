namespace SS.CMS.Services
{
    public partial interface ICacheManager
    {
        int GetInt(string cacheKey);

        void Set(string cacheKey, object value);

        void Clear(string nameofClass);
    }
}
