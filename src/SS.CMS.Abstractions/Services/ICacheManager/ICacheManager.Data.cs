namespace SS.CMS.Services
{
    public partial interface ICacheManager
    {
        void RemoveByClassName(string className);

        void RemoveByPrefix(string prefix);
    }
}
