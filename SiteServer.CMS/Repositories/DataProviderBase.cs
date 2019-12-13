using System.Data;
using Datory;
using SiteServer.Abstractions;

namespace SiteServer.CMS.Repositories
{
    public class DataProviderBase
    {
        protected IDbConnection GetConnection(string connectionString)
        {
            return SqlUtils.GetIDbConnection(WebConfigUtils.DatabaseType, connectionString);
        }

        protected IDataReader ExecuteReader(string commandText)
        {
            return DataProvider.DatabaseApi.ExecuteReader(WebConfigUtils.ConnectionString, commandText);
        }

        protected IDataReader ExecuteReader(IDbConnection conn, string commandText)
        {
            return DataProvider.DatabaseApi.ExecuteReader(conn, commandText);
        }

        protected IDataReader ExecuteReader(string connectionString, string commandText)
        {
            return DataProvider.DatabaseApi.ExecuteReader(connectionString, commandText);
        }

        protected DataSet ExecuteDataset(string commandText)
        {
            return DataProvider.DatabaseApi.ExecuteDataset(WebConfigUtils.ConnectionString, commandText);
        }

        protected DataSet ExecuteDataset(string connectionString, string commandText)
        {
            return DataProvider.DatabaseApi.ExecuteDataset(connectionString, commandText);
        }

        protected string GetString(IDataReader rdr, int i)
        {
            var value = rdr.IsDBNull(i) ? string.Empty : rdr.GetValue(i).ToString();
            if (!string.IsNullOrEmpty(value))
            {
                value = AttackUtils.UnFilterSql(value);
            }
            if (WebConfigUtils.DatabaseType == DatabaseType.Oracle && value == SqlUtils.OracleEmptyValue)
            {
                value = string.Empty;
            }
            return value;
        }

        protected int GetInt(IDataReader rdr, int i)
        {
            return rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
        }
    }
}
