using System.Collections.Generic;
using Datory;

namespace SSCMS.Repositories
{
    public partial interface IContentRepository
    {
        List<TableColumn> GetTableColumns(string tableName);
    }
}