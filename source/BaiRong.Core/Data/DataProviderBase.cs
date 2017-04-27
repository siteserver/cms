using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BaiRong.Core.Model.Enumerations;

namespace BaiRong.Core.Data
{
    public class DataProviderBase
    {
        protected virtual string ConnectionString => WebConfigUtils.ConnectionString;

        protected IDbConnection GetConnection()
        {
            return SqlUtils.GetIDbConnection(WebConfigUtils.IsMySql, ConnectionString);
        }

        protected IDbConnection GetConnection(string connectionString)
        {
            return SqlUtils.GetIDbConnection(WebConfigUtils.IsMySql, connectionString);
        }

        protected IDbConnection GetConnection(bool isMySql, string connectionString)
        {
            return SqlUtils.GetIDbConnection(isMySql, connectionString);
        }

        protected IDbDataParameter GetParameter(string parameterName, EDataType dataType, int value)
        {
            return GetParameterInner(parameterName, dataType, 0, value);
        }

        protected IDbDataParameter GetParameter(string parameterName, EDataType dataType, DateTime value)
        {
            return GetParameterInner(parameterName, dataType, 0, value);
        }

        protected IDbDataParameter GetParameter(string parameterName, EDataType dataType, string value)
        {
            return GetParameterInner(parameterName, dataType, 0, value);
        }

        protected IDbDataParameter GetParameter(string parameterName, EDataType dataType, int size, string value)
        {
            return GetParameterInner(parameterName, dataType, size, value);
        }

        protected IDbDataParameter GetParameter(string parameterName, EDataType dataType, int size, decimal value)
        {
            return GetParameterInner(parameterName, dataType, size, value);
        }

        protected List<IDataParameter> GetInParameterList(string parameterName, EDataType dataType, int dataLength, ICollection valueCollection, out string parameterNameList)
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

                parameterList.Add(dataType == EDataType.Integer
                    ? GetParameter(parmName, dataType, value)
                    : GetParameter(parmName, dataType, dataLength, value));
            }

            parameterNameList = sbCondition.ToString().TrimEnd(',');

            return parameterList;
        }

        private static IDbDataParameter GetParameterInner(string parameterName, EDataType dataType, int size, object value)
        {
            if (size == 0)
            {
                var parameter = SqlUtils.GetIDbDataParameter(parameterName, dataType);
                parameter.Value = value;
                return parameter;
            }
            else
            {
                var parameter = SqlUtils.GetIDbDataParameter(parameterName, dataType, size);
                parameter.Value = value;
                return parameter;
            }
        }


        protected IDataReader ExecuteReader(string commandText, params IDataParameter[] commandParameters)
        {
            return !string.IsNullOrEmpty(commandText) ? WebConfigUtils.Helper.ExecuteReader(ConnectionString, CommandType.Text, commandText, commandParameters) : null;
        }


        protected IDataReader ExecuteReader(string commandText)
        {
            return !string.IsNullOrEmpty(commandText) ? WebConfigUtils.Helper.ExecuteReader(ConnectionString, CommandType.Text, commandText) : null;
        }


        protected IDataReader ExecuteReader(IDbConnection conn, string commandText, params IDataParameter[] commandParameters)
        {
            return !string.IsNullOrEmpty(commandText) ? WebConfigUtils.Helper.ExecuteReader(conn, CommandType.Text, commandText, commandParameters) : null;
        }


        protected IDataReader ExecuteReader(IDbConnection conn, string commandText)
        {
            return !string.IsNullOrEmpty(commandText) ? WebConfigUtils.Helper.ExecuteReader(conn, CommandType.Text, commandText) : null;
        }


        protected IDataReader ExecuteReader(IDbTransaction trans, string commandText, params IDataParameter[] commandParameters)
        {
            return !string.IsNullOrEmpty(commandText) ? WebConfigUtils.Helper.ExecuteReader(trans, CommandType.Text, commandText, commandParameters) : null;
        }


        protected IDataReader ExecuteReader(IDbTransaction trans, string commandText)
        {
            return !string.IsNullOrEmpty(commandText) ? WebConfigUtils.Helper.ExecuteReader(trans, CommandType.Text, commandText) : null;
        }


        protected IDataReader ExecuteReader(string connectionString, string commandText)
        {
            return !string.IsNullOrEmpty(commandText) ? WebConfigUtils.Helper.ExecuteReader(connectionString, CommandType.Text, commandText) : null;
        }

        protected IDataReader ExecuteReader(string connectionString, string commandText, params IDataParameter[] commandParameters)
        {
            return !string.IsNullOrEmpty(commandText) ? WebConfigUtils.Helper.ExecuteReader(connectionString, CommandType.Text, commandText, commandParameters) : null;
        }


        protected DataSet ExecuteDataset(string commandText, params IDataParameter[] commandParameters)
        {
            return !string.IsNullOrEmpty(commandText) ? WebConfigUtils.Helper.ExecuteDataset(ConnectionString, CommandType.Text, commandText, commandParameters) : null;
        }


        protected DataSet ExecuteDataset(string commandText)
        {
            return !string.IsNullOrEmpty(commandText) ? WebConfigUtils.Helper.ExecuteDataset(ConnectionString, CommandType.Text, commandText) : null;
        }

        protected DataSet ExecuteDataset(string connectionString, string commandText)
        {
            return !string.IsNullOrEmpty(commandText) ? WebConfigUtils.Helper.ExecuteDataset(connectionString, CommandType.Text, commandText) : null;
        }

        protected int ExecuteNonQuery(IDbConnection conn, CommandType commandType, string commandText, params IDataParameter[] commandParameters)
        {
            return !string.IsNullOrEmpty(commandText) ? WebConfigUtils.Helper.ExecuteNonQuery(conn, commandType, commandText, commandParameters) : 0;
        }


        protected int ExecuteNonQuery(IDbConnection conn, string commandText, params IDataParameter[] commandParameters)
        {
            return !string.IsNullOrEmpty(commandText) ? WebConfigUtils.Helper.ExecuteNonQuery(conn, CommandType.Text, commandText, commandParameters) : 0;
        }


        protected int ExecuteNonQuery(IDbConnection conn, string commandText)
        {
            return !string.IsNullOrEmpty(commandText) ? WebConfigUtils.Helper.ExecuteNonQuery(conn, CommandType.Text, commandText) : 0;
        }


        protected int ExecuteNonQuery(IDbTransaction trans, string commandText, params IDataParameter[] commandParameters)
        {
            return !string.IsNullOrEmpty(commandText) ? WebConfigUtils.Helper.ExecuteNonQuery(trans, CommandType.Text, commandText, commandParameters) : 0;
        }


        protected int ExecuteNonQuery(IDbTransaction trans, string commandText)
        {
            return !string.IsNullOrEmpty(commandText) ? WebConfigUtils.Helper.ExecuteNonQuery(trans, CommandType.Text, commandText) : 0;
        }

        protected int ExecuteNonQuery(string connectionString, string commandText, params IDataParameter[] commandParameters)
        {
            return !string.IsNullOrEmpty(commandText) ? WebConfigUtils.Helper.ExecuteNonQuery(connectionString, CommandType.Text, commandText, commandParameters) : 0;
        }


        protected int ExecuteNonQuery(string commandText, params IDataParameter[] commandParameters)
        {
            return !string.IsNullOrEmpty(commandText) ? WebConfigUtils.Helper.ExecuteNonQuery(ConnectionString, CommandType.Text, commandText, commandParameters) : 0;
        }


        protected int ExecuteNonQuery(string commandText)
        {
            return !string.IsNullOrEmpty(commandText) ? WebConfigUtils.Helper.ExecuteNonQuery(ConnectionString, CommandType.Text, commandText) : 0;
        }

        protected int ExecuteNonQueryAndReturnId(IDbTransaction trans, string commandText, params IDataParameter[] commandParameters)
        {
            if (string.IsNullOrEmpty(commandText)) return 0;

            var id = 0;
            WebConfigUtils.Helper.ExecuteNonQuery(trans, CommandType.Text, commandText, commandParameters);

            using (var rdr = ExecuteReader(trans, "SELECT @@IDENTITY AS 'ID'"))
            {
                if (rdr.Read())
                {
                    id = TranslateUtils.ToInt(GetString(rdr, 0));
                }
                rdr.Close();
            }

            if (id == 0)
            {
                trans.Rollback();
            }

            return id;
        }


        protected int ExecuteNonQueryAndReturnId(IDbTransaction trans, string commandText)
        {
            return ExecuteNonQueryAndReturnId(trans, commandText, null);
        }


        protected object ExecuteScalar(IDbConnection conn, string commandText, params IDataParameter[] commandParameters)
        {
            return !string.IsNullOrEmpty(commandText) ? WebConfigUtils.Helper.ExecuteScalar(conn, CommandType.Text, commandText, commandParameters) : null;
        }


        protected object ExecuteScalar(IDbConnection conn, string commandText)
        {
            return !string.IsNullOrEmpty(commandText) ? WebConfigUtils.Helper.ExecuteScalar(conn, CommandType.Text, commandText) : null;
        }


        protected object ExecuteScalar(IDbTransaction trans, string commandText, params IDataParameter[] commandParameters)
        {
            return !string.IsNullOrEmpty(commandText) ? WebConfigUtils.Helper.ExecuteScalar(trans, CommandType.Text, commandText, commandParameters) : null;
        }


        protected object ExecuteScalar(IDbTransaction trans, string commandText)
        {
            return !string.IsNullOrEmpty(commandText) ? WebConfigUtils.Helper.ExecuteScalar(trans, CommandType.Text, commandText) : null;
        }


        protected object ExecuteScalar(string commandText, params IDataParameter[] commandParameters)
        {
            return !string.IsNullOrEmpty(commandText) ? WebConfigUtils.Helper.ExecuteScalar(ConnectionString, CommandType.Text, commandText, commandParameters) : null;
        }


        protected object ExecuteScalar(string commandText)
        {
            return !string.IsNullOrEmpty(commandText) ? WebConfigUtils.Helper.ExecuteScalar(ConnectionString, CommandType.Text, commandText) : null;
        }

        protected string GetString(IDataReader rdr, int i)
        {
            var value = rdr.IsDBNull(i) ? string.Empty : rdr.GetValue(i).ToString();
            if (!string.IsNullOrEmpty(value))
            {
                value = PageUtils.UnFilterSql(value);
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
