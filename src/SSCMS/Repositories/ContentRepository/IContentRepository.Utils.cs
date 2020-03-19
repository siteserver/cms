using System.Collections.Generic;
using Datory;

namespace SSCMS
{
    public partial interface IContentRepository
    {
        List<TableColumn> GetTableColumns(string tableName);
    }
}