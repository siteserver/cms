namespace SS.CMS.Abstractions.Services
{
    public partial interface IPluginManager
    {
        bool IsContentTable(IService service);

        string GetContentTableName(string pluginId);

        void SyncContentTable(IService service);

        bool IsContentTableUsed(string tableName);
    }
}
