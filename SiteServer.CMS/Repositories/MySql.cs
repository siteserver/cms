using System;
using System.Data;
using System.IO;
using System.Xml;
using MySql.Data.MySqlClient;

namespace SiteServer.CMS.Repositories
{
    public class MySql : DatabaseApi
    {
        public override IDbConnection GetConnection(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }

        public override IDbDataAdapter GetDataAdapter()
        {
            return new MySqlDataAdapter();
        }
    }
}
