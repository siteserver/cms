using System.Collections.Generic;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface ITableStyleRepository
    {
        List<KeyValuePair<string, TableStyleInfo>> GetAllTableStyles();
    }
}