using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiteServer.Plugin.Models;

namespace SiteServer.Plugin.Apis
{
    public interface IDataApi
    {
        string DatabaseType { get; }

        string ConnectionString { get; }

        IDbHelper DbHelper { get; }
    }
}
