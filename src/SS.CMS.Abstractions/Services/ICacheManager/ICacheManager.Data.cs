namespace SS.CMS.Services.ICacheManager
{
    public partial interface ICacheManager
    {
        void RemoveByClassName(string className);

        void RemoveByPrefix(string prefix);
    }
}
