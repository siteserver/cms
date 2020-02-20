using System.Collections.Generic;
using Datory;

namespace SiteServer.Abstractions
{
    public partial interface IContentRepository
    {
        List<TableColumn> GetTableColumns(string tableName);

        List<TableColumn> GetDefaultTableColumns(string tableName);
    }
}