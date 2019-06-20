namespace SS.CMS.Services
{
    public partial interface IPluginManager
    {
        void SyncContentTable(IService service);

        bool IsContentTableUsed(string tableName);
    }
}
