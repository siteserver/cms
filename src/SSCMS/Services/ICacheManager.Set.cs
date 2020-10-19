namespace SSCMS.Services
{
    public partial interface ICacheManager<TCacheValue>
    {
        void AddOrUpdateSliding(string key, TCacheValue value, int minutes);

        void AddOrUpdateAbsolute(string key, TCacheValue value, int minutes);

        void AddOrUpdate(string key, TCacheValue value);
    }
}
