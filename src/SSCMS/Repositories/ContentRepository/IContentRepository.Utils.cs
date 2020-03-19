using System.Collections.Generic;
using Datory;

namespace SSCMS.Abstractions
{
    public partial interface IContentRepository
    {
        List<TableColumn> GetTableColumns(string tableName);
    }
}