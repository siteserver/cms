using System.Collections.Generic;
using Datory;

namespace SSCMS.Abstractions
{
    public partial interface IContentRepository
    {
        string GetRandomTableName();
        List<TableColumn> GetTableColumns(string tableName);
    }
}