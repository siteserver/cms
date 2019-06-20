using System;
using System.Collections.Generic;
using SS.CMS.Data;

namespace SS.CMS.Services
{
    public partial interface ITableManager
    {
        List<TableColumn> GetTableColumnInfoList(string tableName);

        List<TableColumn> GetTableColumnInfoList(string tableName, List<string> excludeAttributeNameList);

        List<TableColumn> GetTableColumnInfoList(string tableName, DataType excludeDataType);

        TableColumn GetTableColumnInfo(string tableName, string attributeName);

        bool IsAttributeNameExists(string tableName, string attributeName);

        List<string> GetTableColumnNameList(string tableName);

        List<string> GetTableColumnNameList(string tableName, List<string> excludeAttributeNameList);

        List<string> GetTableColumnNameList(string tableName, DataType excludeDataType);

        bool CreateTable(string tableName, List<TableColumn> tableColumns, string pluginId, bool isContentTable, out Exception ex);

        void AlterTable(string tableName, List<TableColumn> tableColumns, string pluginId, List<string> dropColumnNames = null);

        void CreateContentTable(string tableName, List<TableColumn> tableColumns);

        void AlterSystemTable(string tableName, List<TableColumn> tableColumns, List<string> dropColumnNames = null);

        void InstallDatabase(string userName, string adminPassword);

        void SyncSystemTables();

        void SyncContentTables();

        void UpdateConfigVersion();

        void SyncDatabase();
    }

}
