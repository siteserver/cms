using System.Collections.Generic;
using System.Data;
using SiteServer.CMS.Apis;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Database.Core
{
    public class DataProviderBase
    {
        protected virtual string ConnectionString => WebConfigUtils.ConnectionString;

        public virtual string TableName => string.Empty;

        public virtual List<TableColumn> TableColumns => null;

        protected IDbConnection GetConnection()
        {
            return SqlDifferences.GetIDbConnection(WebConfigUtils.DatabaseType, ConnectionString);
        }

        protected IDbConnection GetConnection(string connectionString)
        {
            return SqlDifferences.GetIDbConnection(WebConfigUtils.DatabaseType, connectionString);
        }

        protected IDbConnection GetConnection(DatabaseType databaseType, string connectionString)
        {
            return SqlDifferences.GetIDbConnection(databaseType, connectionString);
        }

        protected IDataParameter GetParameter(string name, object value)
        {
            return DatabaseApi.GetParameter(name, value);
        }

        //protected int InsertEntity<T>(T entityInfo) where T : class, IDataInfo
        //{
        //    if (entityInfo == null) return 0;

        //    if (string.IsNullOrEmpty(entityInfo.Guid) || !StringUtils.IsGuid(entityInfo.Guid))
        //    {
        //        entityInfo.Guid = StringUtils.GetGuid();
        //    }
        //    entityInfo.LastModifiedDate = DateTime.Now;

        //    using (var connection = GetConnection())
        //    {
        //        entityInfo.Id = Convert.ToInt32(connection.InsertObject(entityInfo));
        //    }

        //    return entityInfo.Id;
        //}

        //protected T GetEntity<T>(int identity) where T : class, IDataInfo
        //{
        //    if (identity <= 0) return null;

        //    T entityInfo;

        //    using (var connection = GetConnection())
        //    {
        //        entityInfo = connection.Get<T>(identity);
        //    }

        //    if (entityInfo != null && (string.IsNullOrEmpty(entityInfo.Guid) || !StringUtils.IsGuid(entityInfo.Guid)))
        //    {
        //        entityInfo.Guid = StringUtils.GetGuid();
        //        entityInfo.LastModifiedDate = DateTime.Now;
        //        UpdateEntity(entityInfo);
        //    }

        //    return entityInfo;
        //}

        //protected bool UpdateEntity<T>(T entityInfo) where T : class, IDataInfo
        //{
        //    if (entityInfo == null) return false;

        //    bool updated;
        //    if (string.IsNullOrEmpty(entityInfo.Guid) || !StringUtils.IsGuid(entityInfo.Guid))
        //    {
        //        entityInfo.Guid = StringUtils.GetGuid();
        //    }
        //    entityInfo.LastModifiedDate = DateTime.Now;

        //    using (var connection = GetConnection())
        //    {
        //        updated = connection.UpdateObject(entityInfo);
        //    }

        //    return updated;
        //}

        //protected bool DeleteEntity<T>(int identity) where T : class, IDataInfo, new()
        //{
        //    if (identity <= 0) return false;

        //    bool deleted;

        //    using (var connection = GetConnection())
        //    {
        //        deleted = connection.DeleteById(new T { Id = identity });
        //    }

        //    return deleted;
        //}

        //private void SaveAsync(string tableName, BaseEntityInfo entityInfo)
        //{
        //    if (string.IsNullOrEmpty(entityInfo.Guid) || !StringUtils.IsGuid(entityInfo.Guid))
        //    {
        //        var sqlString = $@"UPDATE {tableName} SET {nameof(BaseEntityInfo.LastModifiedDate)} = @{nameof(BaseEntityInfo.LastModifiedDate)} WHERE {nameof(BaseEntityInfo.Id)} = @{nameof(BaseEntityInfo.Id)}";

        //        IDataParameter[] parameters =
        //        {
        //            GetParameter(nameof(entityInfo.LastModifiedDate), DateTimeOffset.UtcNow),
        //            GetParameter(nameof(entityInfo.Id), entityInfo.Id)
        //        };

        //        DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
        //    }
        //    else
        //    {
        //        var sqlString = $@"UPDATE {tableName} SET
        //        {nameof(BaseEntityInfo.Guid)} = @{nameof(BaseEntityInfo.Guid)}, 
        //        {nameof(BaseEntityInfo.LastModifiedDate)} = @{nameof(BaseEntityInfo.LastModifiedDate)}
        //    WHERE {nameof(BaseEntityInfo.Id)} = @{nameof(BaseEntityInfo.Id)}";

        //        IDataParameter[] parameters =
        //        {
        //            GetParameter(nameof(entityInfo.Guid), entityInfo.Guid),
        //            GetParameter(nameof(entityInfo.LastModifiedDate), DateTimeOffset.UtcNow),
        //            GetParameter(nameof(entityInfo.Id), entityInfo.Id)
        //        };

        //        DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
        //    }
        //}

        //protected IDbDataParameter GetParameter(string parameterName, DataType dataType, int value)
        //{
        //    return SqlUtils.GetIDbDataParameter(parameterName, dataType, 0, value);
        //}

        //protected IDbDataParameter GetParameter(string parameterName, DataType dataType, bool value)
        //{
        //    return SqlUtils.GetIDbDataParameter(parameterName, dataType, 0, value);
        //}

        //protected IDbDataParameter GetParameter(string parameterName, DataType dataType, decimal value)
        //{
        //    return SqlUtils.GetIDbDataParameter(parameterName, dataType, 0, value);
        //}

        //protected IDbDataParameter GetParameter(string parameterName, DataType dataType, DateTime value)
        //{
        //    return SqlUtils.GetIDbDataParameter(parameterName, dataType, 0, value);
        //}

        //protected IDbDataParameter GetParameter(string parameterName, DataType dataType, string value)
        //{
        //    return SqlUtils.GetIDbDataParameter(parameterName, dataType, 0, value);
        //}

        //protected IDbDataParameter GetParameter(string parameterName, DataType dataType, int size, string value)
        //{
        //    return SqlUtils.GetIDbDataParameter(parameterName, dataType, size, value);
        //}

        //protected IDbDataParameter GetParameter(string parameterName, DataType dataType, int size, decimal value)
        //{
        //    return SqlUtils.GetIDbDataParameter(parameterName, dataType, size, value);
        //}

        protected DatabaseApi DatabaseApi => DatabaseApi.Instance;


        //protected IDataReader ExecuteReader(string commandText, params IDataParameter[] commandParameters)
        //{
        //    return DatabaseApi.Instance.ExecuteReader(ConnectionString, commandText, commandParameters);
        //}


        //protected IDataReader ExecuteReader(string commandText)
        //{
        //    return DatabaseApi.Instance.ExecuteReader(ConnectionString, commandText);
        //}


        //protected IDataReader ExecuteReader(IDbConnection conn, string commandText, params IDataParameter[] commandParameters)
        //{
        //    return DatabaseApi.Instance.ExecuteReader(conn, commandText, commandParameters);
        //}


        //protected IDataReader ExecuteReader(IDbConnection conn, string commandText)
        //{
        //    return DatabaseApi.Instance.ExecuteReader(conn, commandText);
        //}


        //protected IDataReader ExecuteReader(IDbTransaction trans, string commandText, params IDataParameter[] commandParameters)
        //{
        //    return DatabaseApi.Instance.ExecuteReader(trans, commandText, commandParameters);
        //}


        //protected IDataReader ExecuteReader(IDbTransaction trans, string commandText)
        //{
        //    return DatabaseApi.Instance.ExecuteReader(trans, commandText);
        //}


        //protected IDataReader ExecuteReader(string connectionString, string commandText)
        //{
        //    return DatabaseApi.Instance.ExecuteReader(connectionString, commandText);
        //}

        //protected IDataReader ExecuteReader(string connectionString, string commandText, params IDataParameter[] commandParameters)
        //{
        //    return DatabaseApi.Instance.ExecuteReader(connectionString, commandText, commandParameters);
        //}


        //protected DataSet ExecuteDataset(string commandText, params IDataParameter[] commandParameters)
        //{
        //    return DatabaseApi.Instance.ExecuteDataset(ConnectionString, commandText, commandParameters);
        //}


        //protected DataSet ExecuteDataset(string commandText)
        //{
        //    return DatabaseApi.Instance.ExecuteDataset(ConnectionString, commandText);
        //}

        //protected DataSet ExecuteDataset(string connectionString, string commandText)
        //{
        //    return DatabaseApi.Instance.ExecuteDataset(connectionString, commandText);
        //}

        //protected int ExecuteNonQuery(IDbConnection conn, string commandText, params IDataParameter[] commandParameters)
        //{
        //    return DatabaseApi.Instance.ExecuteNonQuery(conn, commandText, commandParameters);
        //}


        //protected int ExecuteNonQuery(IDbConnection conn, string commandText)
        //{
        //    return DatabaseApi.Instance.ExecuteNonQuery(conn, commandText);
        //}

        //protected int ExecuteNonQuery(IDbTransaction trans, string commandText, params IDataParameter[] commandParameters)
        //{
        //    return DatabaseApi.Instance.ExecuteNonQuery(trans, commandText, commandParameters);
        //}

        //protected int ExecuteNonQuery(IDbTransaction trans, string commandText)
        //{
        //    return DatabaseApi.Instance.ExecuteNonQuery(trans, commandText);
        //}

        //protected int ExecuteNonQuery(string connectionString, string commandText, params IDataParameter[] commandParameters)
        //{
        //    return DatabaseApi.Instance.ExecuteNonQuery(connectionString, commandText, commandParameters);
        //}

        //protected int ExecuteNonQuery(string commandText, params IDataParameter[] commandParameters)
        //{
        //    return DatabaseApi.Instance.ExecuteNonQuery(ConnectionString, commandText, commandParameters);
        //}

        //protected int ExecuteNonQuery(string commandText)
        //{
        //    return DatabaseApi.Instance.ExecuteNonQuery(ConnectionString, commandText);
        //}

        //protected int ExecuteNonQueryAndReturnId(string tableName, string idColumnName, string commandText, params IDataParameter[] commandParameters)
        //{
        //    return DatabaseApi.Instance.ExecuteNonQueryAndReturnId(tableName, idColumnName, ConnectionString, commandText, commandParameters);
        //}

        //protected int ExecuteNonQueryAndReturnId(string tableName, string idColumnName, IDbTransaction trans, string commandText, params IDataParameter[] commandParameters)
        //{
        //    return DatabaseApi.Instance.ExecuteNonQueryAndReturnId(tableName, idColumnName, trans, commandText, commandParameters);
        //}

        //protected int ExecuteCurrentId(IDbTransaction trans, string tableName, string idColumnName)
        //{
        //    return DatabaseApi.Instance.ExecuteCurrentId(trans, tableName, idColumnName);
        //}

        //protected object ExecuteScalar(IDbConnection conn, string commandText, params IDataParameter[] commandParameters)
        //{
        //    return DatabaseApi.Instance.ExecuteScalar(conn, commandText, commandParameters);
        //}

        //protected object ExecuteScalar(IDbConnection conn, string commandText)
        //{
        //    return DatabaseApi.Instance.ExecuteScalar(conn, commandText);
        //}

        //protected object ExecuteScalar(IDbTransaction trans, string commandText, params IDataParameter[] commandParameters)
        //{
        //    return DatabaseApi.Instance.ExecuteScalar(trans, commandText, commandParameters);
        //}

        //protected object ExecuteScalar(IDbTransaction trans, string commandText)
        //{
        //    return DatabaseApi.Instance.ExecuteScalar(trans, commandText);
        //}

        //protected object ExecuteScalar(string commandText, params IDataParameter[] commandParameters)
        //{
        //    return DatabaseApi.Instance.ExecuteScalar(ConnectionString, commandText, commandParameters);
        //}

        //protected object ExecuteScalar(string commandText)
        //{
        //    return DatabaseApi.Instance.ExecuteScalar(ConnectionString, commandText);
        //}

        //protected string GetString(IDataReader rdr, int i)
        //{
        //    var value = rdr.IsDBNull(i) ? string.Empty : rdr.GetValueById(i).ToString();
        //    if (!string.IsNullOrEmpty(value))
        //    {
        //        value = AttackUtils.UnFilterSql(value);
        //    }
        //    if (WebConfigUtils.DatabaseType == DatabaseType.Oracle && value == SqlUtils.OracleEmptyValue)
        //    {
        //        value = string.Empty;
        //    }
        //    return value;
        //}

        //protected bool GetBool(IDataReader rdr, int i)
        //{
        //    return !rdr.IsDBNull(i) && TranslateUtils.ToBool(rdr.GetValueById(i).ToString());
        //}

        //protected int GetInt(IDataReader rdr, int i)
        //{
        //    return rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
        //}

        //protected decimal GetDecimal(IDataReader rdr, int i)
        //{
        //    return rdr.IsDBNull(i) ? 0 : rdr.GetDecimal(i);
        //}

        //protected double GetDouble(IDataReader rdr, int i)
        //{
        //    return rdr.IsDBNull(i) ? 0 : rdr.GetDouble(i);
        //}

        //protected DateTime GetDateTime(IDataReader rdr, int i)
        //{
        //    return rdr.IsDBNull(i) ? DateTime.Now : rdr.GetDateTime(i);
        //}
    }
}
