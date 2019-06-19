namespace SS.CMS.Services.IPluginManager
{
    public partial interface IPluginManager
    {
        void SyncContentTable(IService service);

        bool IsContentTableUsed(string tableName);
    }
}
