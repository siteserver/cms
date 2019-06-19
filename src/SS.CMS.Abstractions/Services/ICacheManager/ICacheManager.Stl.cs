namespace SS.CMS.Services.ICacheManager
{
    public partial interface ICacheManager
    {
        int GetInt(string cacheKey);

        void Set(string cacheKey, object value);

        void Clear(string nameofClass);
    }
}
