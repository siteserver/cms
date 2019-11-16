using System;
using System.Data;
using Datory;
using SiteServer.CMS.Core;
using SiteServer.Utils;

namespace SiteServer.CMS.Data
{
    public class DataProviderBase
    {

        protected IDbConnection GetConnection()
        {
            return SqlUtils.GetIDbConnection(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString);
        }

        protected IDbConnection GetConnection(string connectionString)
        {
            return SqlUtils.GetIDbConnection(WebConfigUtils.DatabaseType, connectionString);
        }

        protected IDbDataParameter GetParameter(string parameterName, DataType dataType, int value)
        {
            return SqlUtils.GetIDbDataParameter(parameterName, dataType, 0, value);
        }

        protected IDbDataParameter GetParameter(string parameterName, DataType dataType, bool value)
        {
            return SqlUtils.GetIDbDataParameter(parameterName, dataType, 0, value);
        }

        protected IDbDataParameter GetParameter(string parameterName, DataType dataType, string value)
        {
            return SqlUtils.GetIDbDataParameter(parameterName, dataType, 0, value);
        }

        protected IDbDataParameter GetParameter(string parameterName, DataType dataType, int size, string value)
        {
            return SqlUtils.GetIDbDataParameter(parameterName, dataType, size, value);
        }

        protected IDataReader ExecuteReader(string commandText, params IDataParameter[] commandParameters)
        {
            return DataProvider.DatabaseApi.ExecuteReader(WebConfigUtils.ConnectionString, commandText, commandParameters);
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

        protected int ExecuteNonQuery(IDbTransaction trans, string commandText)
        {
            return DataProvider.DatabaseApi.ExecuteNonQuery(trans, commandText);
        }

        protected int ExecuteNonQuery(string commandText, params IDataParameter[] commandParameters)
        {
            return DataProvider.DatabaseApi.ExecuteNonQuery(WebConfigUtils.ConnectionString, commandText, commandParameters);
        }

        protected int ExecuteNonQuery(string commandText)
        {
            return DataProvider.DatabaseApi.ExecuteNonQuery(WebConfigUtils.ConnectionString, commandText);
        }

        protected object ExecuteScalar(string commandText)
        {
            return DataProvider.DatabaseApi.ExecuteScalar(WebConfigUtils.ConnectionString, commandText);
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
