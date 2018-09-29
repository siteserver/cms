using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using SiteServer.CMS.Core;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Data
{
    public class DataProviderBase
    {
        protected virtual string ConnectionString => WebConfigUtils.ConnectionString;

        public virtual string TableName => string.Empty;

        public virtual List<TableColumn> TableColumns => null;

        protected IDbConnection GetConnection()
        {
            return SqlUtils.GetIDbConnection(WebConfigUtils.DatabaseType, ConnectionString);
        }

        protected IDbConnection GetConnection(string connectionString)
        {
            return SqlUtils.GetIDbConnection(WebConfigUtils.DatabaseType, connectionString);
        }

        protected IDbConnection GetConnection(DatabaseType databaseType, string connectionString)
        {
            return SqlUtils.GetIDbConnection(databaseType, connectionString);
        }

        protected IDbDataParameter GetParameter(string parameterName, DataType dataType, int value)
        {
            return SqlUtils.GetIDbDataParameter(parameterName, dataType, 0, value);
        }

        protected IDbDataParameter GetParameter(string parameterName, DataType dataType, bool value)
        {
            return SqlUtils.GetIDbDataParameter(parameterName, dataType, 0, value);
        }

        protected IDbDataParameter GetParameter(string parameterName, DataType dataType, decimal value)
        {
            return SqlUtils.GetIDbDataParameter(parameterName, dataType, 0, value);
        }

        protected IDbDataParameter GetParameter(string parameterName, DataType dataType, DateTime value)
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

        protected IDbDataParameter GetParameter(string parameterName, DataType dataType, int size, decimal value)
        {
            return SqlUtils.GetIDbDataParameter(parameterName, dataType, size, value);
        }

        protected List<IDataParameter> GetInParameterList(string parameterName, DataType dataType, int dataLength, ICollection valueCollection, out string parameterNameList)
        {
            parameterNameList = string.Empty;
            if (valueCollection == null || valueCollection.Count <= 0) return new List<IDataParameter>();

            var parameterList = new List<IDataParameter>();

            var sbCondition = new StringBuilder();
            var i = 0;
            foreach (var obj in valueCollection)
            {
                i++;

                var value = obj.ToString();
                var parmName = parameterName + "_" + i;

                sbCondition.Append(parmName + ",");

                parameterList.Add(dataType == DataType.Integer
                    ? GetParameter(parmName, dataType, value)
                    : GetParameter(parmName, dataType, dataLength, value));
            }

            parameterNameList = sbCondition.ToString().TrimEnd(',');

            return parameterList;
        }

        protected IDataReader ExecuteReader(string commandText, params IDataParameter[] commandParameters)
        {
            return DataProvider.DatabaseApi.ExecuteReader(ConnectionString, commandText, commandParameters);
        }


        protected IDataReader ExecuteReader(string commandText)
        {
            return DataProvider.DatabaseApi.ExecuteReader(ConnectionString, commandText);
        }


        protected IDataReader ExecuteReader(IDbConnection conn, string commandText, params IDataParameter[] commandParameters)
        {
            return DataProvider.DatabaseApi.ExecuteReader(conn, commandText, commandParameters);
        }


        protected IDataReader ExecuteReader(IDbConnection conn, string commandText)
        {
            return DataProvider.DatabaseApi.ExecuteReader(conn, commandText);
        }


        protected IDataReader ExecuteReader(IDbTransaction trans, string commandText, params IDataParameter[] commandParameters)
        {
            return DataProvider.DatabaseApi.ExecuteReader(trans, commandText, commandParameters);
        }


        protected IDataReader ExecuteReader(IDbTransaction trans, string commandText)
        {
            return DataProvider.DatabaseApi.ExecuteReader(trans, commandText);
        }


        protected IDataReader ExecuteReader(string connectionString, string commandText)
        {
            return DataProvider.DatabaseApi.ExecuteReader(connectionString, commandText);
        }

        protected IDataReader ExecuteReader(string connectionString, string commandText, params IDataParameter[] commandParameters)
        {
            return DataProvider.DatabaseApi.ExecuteReader(connectionString, commandText, commandParameters);
        }


        protected DataSet ExecuteDataset(string commandText, params IDataParameter[] commandParameters)
        {
            return DataProvider.DatabaseApi.ExecuteDataset(ConnectionString, commandText, commandParameters);
        }


        protected DataSet ExecuteDataset(string commandText)
        {
            return DataProvider.DatabaseApi.ExecuteDataset(ConnectionString, commandText);
        }

        protected DataSet ExecuteDataset(string connectionString, string commandText)
        {
            return DataProvider.DatabaseApi.ExecuteDataset(connectionString, commandText);
        }

        protected int ExecuteNonQuery(IDbConnection conn, string commandText, params IDataParameter[] commandParameters)
        {
            return DataProvider.DatabaseApi.ExecuteNonQuery(conn, commandText, commandParameters);
        }


        protected int ExecuteNonQuery(IDbConnection conn, string commandText)
        {
            return DataProvider.DatabaseApi.ExecuteNonQuery(conn, commandText);
        }


        protected int ExecuteNonQuery(IDbTransaction trans, string commandText, params IDataParameter[] commandParameters)
        {
            return DataProvider.DatabaseApi.ExecuteNonQuery(trans, commandText, commandParameters);
        }


        protected int ExecuteNonQuery(IDbTransaction trans, string commandText)
        {
            return DataProvider.DatabaseApi.ExecuteNonQuery(trans, commandText);
        }

        protected int ExecuteNonQuery(string connectionString, string commandText, params IDataParameter[] commandParameters)
        {
            return DataProvider.DatabaseApi.ExecuteNonQuery(connectionString, commandText, commandParameters);
        }

        protected int ExecuteNonQuery(string commandText, params IDataParameter[] commandParameters)
        {
            return DataProvider.DatabaseApi.ExecuteNonQuery(ConnectionString, commandText, commandParameters);
        }

        protected int ExecuteNonQuery(string commandText)
        {
            return DataProvider.DatabaseApi.ExecuteNonQuery(ConnectionString, commandText);
        }

        protected int ExecuteNonQueryAndReturnId(string tableName, string idColumnName, string commandText, params IDataParameter[] commandParameters)
        {
            return DataProvider.DatabaseApi.ExecuteNonQueryAndReturnId(tableName, idColumnName, ConnectionString, commandText, commandParameters);
        }

        protected int ExecuteNonQueryAndReturnId(string tableName, string idColumnName, IDbTransaction trans, string commandText, params IDataParameter[] commandParameters)
        {
            return DataProvider.DatabaseApi.ExecuteNonQueryAndReturnId(tableName, idColumnName, trans, commandText, commandParameters);
        }

        protected int ExecuteCurrentId(IDbTransaction trans, string tableName, string idColumnName)
        {
            return DataProvider.DatabaseApi.ExecuteCurrentId(trans, tableName, idColumnName);
        }

        protected object ExecuteScalar(IDbConnection conn, string commandText, params IDataParameter[] commandParameters)
        {
            return DataProvider.DatabaseApi.ExecuteScalar(conn, commandText, commandParameters);
        }

        protected object ExecuteScalar(IDbConnection conn, string commandText)
        {
            return DataProvider.DatabaseApi.ExecuteScalar(conn, commandText);
        }

        protected object ExecuteScalar(IDbTransaction trans, string commandText, params IDataParameter[] commandParameters)
        {
            return DataProvider.DatabaseApi.ExecuteScalar(trans, commandText, commandParameters);
        }

        protected object ExecuteScalar(IDbTransaction trans, string commandText)
        {
            return DataProvider.DatabaseApi.ExecuteScalar(trans, commandText);
        }

        protected object ExecuteScalar(string commandText, params IDataParameter[] commandParameters)
        {
            return DataProvider.DatabaseApi.ExecuteScalar(ConnectionString, commandText, commandParameters);
        }

        protected object ExecuteScalar(string commandText)
        {
            return DataProvider.DatabaseApi.ExecuteScalar(ConnectionString, commandText);
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

        protected bool GetBool(IDataReader rdr, int i)
        {
            return !rdr.IsDBNull(i) && TranslateUtils.ToBool(rdr.GetValue(i).ToString());
        }

        protected int GetInt(IDataReader rdr, int i)
        {
            return rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
        }

        protected decimal GetDecimal(IDataReader rdr, int i)
        {
            return rdr.IsDBNull(i) ? 0 : rdr.GetDecimal(i);
        }

        protected double GetDouble(IDataReader rdr, int i)
        {
            return rdr.IsDBNull(i) ? 0 : rdr.GetDouble(i);
        }

        protected DateTime GetDateTime(IDataReader rdr, int i)
        {
            return rdr.IsDBNull(i) ? DateTime.Now : rdr.GetDateTime(i);
        }
    }
}
