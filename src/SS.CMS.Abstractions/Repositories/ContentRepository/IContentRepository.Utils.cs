using System.Collections.Generic;
using Datory;

namespace SS.CMS.Abstractions
{
    public partial interface IContentRepository
    {
        List<TableColumn> GetTableColumns(string tableName);
    }
}