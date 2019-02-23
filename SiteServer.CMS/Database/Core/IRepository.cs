using System.Collections.Generic;
using System.Data;
using SiteServer.Plugin;

namespace SiteServer.CMS.Database.Core
{
    public interface IRepository
    {
        string TableName { get; }

        List<TableColumn> TableColumns { get; }
    }
}
