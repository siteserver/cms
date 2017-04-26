using System;
using System.Data;
using BaiRong.Core.Model;
using BaiRong.Core.Data.Helper;
using System.Data.SqlClient;
using System.Collections.Specialized;
using System.Collections;

namespace BaiRong.Core.Data
{
	//public abstract class SqlBase
	//{
	//	#region 私有数据

	//	private AdoHelper _helper;

	//	private AdoHelper Helper
	//	{
	//		get
	//		{
	//			if (_helper == null)
	//			{
 //                   _helper = new SqlServer();
	//			}
	//			return _helper;
				
	//		}
	//	}

	//	#endregion

 //       protected abstract string ConnectionString { get; }

	//	protected IDbConnection GetConnection()
	//	{
	//		return SqlUtils.GetIDbConnection(ConnectionString);
	//	}

 //       protected SqlParameter GetParameter(string parameterName, SqlDbType dataType, int value)
 //       {
 //           return GetParameterInner(parameterName, dataType, 0, value);
 //       }

 //       protected SqlParameter GetParameter(string parameterName, SqlDbType dataType, DateTime value)
 //       {
 //           return GetParameterInner(parameterName, dataType, 0, value);
 //       }

 //       protected SqlParameter GetParameter(string parameterName, SqlDbType dataType, string value)
 //       {
 //           return GetParameterInner(parameterName, dataType, 0, value);
 //       }

 //       protected SqlParameter GetParameter(string parameterName, SqlDbType dataType, int size, string value)
 //       {
 //           return GetParameterInner(parameterName, dataType, size, value);
 //       }

 //       private SqlParameter GetParameterInner(string parameterName, SqlDbType dataType, int size, object value)
 //       {
 //           if (size == 0)
 //           {
 //               var parameter = new SqlParameter(parameterName, dataType);
 //               parameter.Value = value;
 //               return parameter;
 //           }
 //           else
 //           {
 //               var parameter = new SqlParameter(parameterName, dataType, size);
 //               parameter.Value = value;
 //               return parameter;
 //           }

 //       }

	//	protected IDataReader ExecuteReader(string commandText, params IDataParameter[] commandParameters)
	//	{
 //           if (!string.IsNullOrEmpty(commandText))
 //           {
 //               return Helper.ExecuteReader(ConnectionString, CommandType.Text, commandText, commandParameters);
 //           }
 //           return null;
	//	}

 //       protected IDataReader ExecuteReader(string commandText, CommandType commandType, params IDataParameter[] commandParameters)
 //       {
 //           if (!string.IsNullOrEmpty(commandText))
 //           {
 //               return Helper.ExecuteReader(ConnectionString, commandType, commandText, commandParameters);
 //           }
 //           return null;
 //       }


	//	protected IDataReader ExecuteReader(string commandText)
	//	{
 //           if (!string.IsNullOrEmpty(commandText))
 //           {
 //               return Helper.ExecuteReader(ConnectionString, CommandType.Text, commandText);
 //           }
 //           return null;
	//	}


	//	protected IDataReader ExecuteReader(IDbConnection conn, string commandText, params IDataParameter[] commandParameters)
	//	{
 //           if (!string.IsNullOrEmpty(commandText))
 //           {
 //               return Helper.ExecuteReader(conn, CommandType.Text, commandText, commandParameters);
 //           }
 //           return null;
	//	}

	//	protected IDataReader ExecuteReader(IDbConnection conn, string commandText)
	//	{
 //           if (!string.IsNullOrEmpty(commandText))
 //           {
 //               return Helper.ExecuteReader(conn, CommandType.Text, commandText);
 //           }
 //           return null;
	//	}


	//	protected IDataReader ExecuteReader(IDbTransaction trans, string commandText, params IDataParameter[] commandParameters)
	//	{
 //           if (!string.IsNullOrEmpty(commandText))
 //           {
 //               return Helper.ExecuteReader(trans, CommandType.Text, commandText, commandParameters);
 //           }
 //           return null;
	//	}


	//	protected IDataReader ExecuteReader(IDbTransaction trans, string commandText)
	//	{
 //           if (!string.IsNullOrEmpty(commandText))
 //           {
 //               return Helper.ExecuteReader(trans, CommandType.Text, commandText);
 //           }
 //           return null;
	//	}


	//	protected IDataReader ExecuteReader(string connectionString, string commandText)
	//	{
 //           if (!string.IsNullOrEmpty(commandText))
 //           {
 //               return Helper.ExecuteReader(connectionString, CommandType.Text, commandText);
 //           }
 //           return null;
	//	}


	//	protected DataSet ExecuteDataset(string commandText, params IDataParameter[] commandParameters)
	//	{
 //           if (!string.IsNullOrEmpty(commandText))
 //           {
 //               return Helper.ExecuteDataset(ConnectionString, CommandType.Text, commandText, commandParameters);
 //           }
 //           return null;
	//	}


	//	protected DataSet ExecuteDataset(string commandText)
	//	{
 //           if (!string.IsNullOrEmpty(commandText))
 //           {
 //               return Helper.ExecuteDataset(ConnectionString, CommandType.Text, commandText);
 //           }
 //           return null;
	//	}

 //       protected DataSet ExecuteDataset(string connectionString, string commandText)
 //       {
 //           if (!string.IsNullOrEmpty(commandText))
 //           {
 //               return Helper.ExecuteDataset(connectionString, CommandType.Text, commandText);
 //           }
 //           return null;
 //       }

	//	protected int ExecuteNonQuery(IDbConnection conn, CommandType commandType, string commandText, params IDataParameter[] commandParameters)
	//	{
 //           if (!string.IsNullOrEmpty(commandText))
 //           {
 //               return Helper.ExecuteNonQuery(conn, commandType, commandText, commandParameters);
 //           }
 //           return 0;
	//	}


	//	protected int ExecuteNonQuery(IDbConnection conn, string commandText, params IDataParameter[] commandParameters)
	//	{
 //           if (!string.IsNullOrEmpty(commandText))
 //           {
 //               return Helper.ExecuteNonQuery(conn, CommandType.Text, commandText, commandParameters);
 //           }
 //           return 0;
	//	}


	//	protected int ExecuteNonQuery(IDbConnection conn, string commandText)
	//	{
 //           if (!string.IsNullOrEmpty(commandText))
 //           {
 //               return Helper.ExecuteNonQuery(conn, CommandType.Text, commandText);
 //           }
 //           return 0;
	//	}


	//	protected int ExecuteNonQuery(IDbTransaction trans, string commandText, params IDataParameter[] commandParameters)
	//	{
 //           if (!string.IsNullOrEmpty(commandText))
 //           {
 //               return Helper.ExecuteNonQuery(trans, CommandType.Text, commandText, commandParameters);
 //           }
 //           return 0;
	//	}


	//	protected int ExecuteNonQuery(IDbTransaction trans, string commandText)
	//	{
 //           if (!string.IsNullOrEmpty(commandText))
 //           {
 //               return Helper.ExecuteNonQuery(trans, CommandType.Text, commandText);
 //           }
 //           return 0;
	//	}


	//	protected int ExecuteNonQuery(string commandText, params IDataParameter[] commandParameters)
	//	{
 //           if (!string.IsNullOrEmpty(commandText))
 //           {
 //               return Helper.ExecuteNonQuery(ConnectionString, CommandType.Text, commandText, commandParameters);
 //           }
 //           return 0;
	//	}

	//	protected int ExecuteNonQuery(string commandText)
	//	{
 //           if (!string.IsNullOrEmpty(commandText))
 //           {
 //               return Helper.ExecuteNonQuery(ConnectionString, CommandType.Text, commandText);
 //           }
 //           return 0;
	//	}

	//	protected object ExecuteScalar(IDbConnection conn, string commandText, params IDataParameter[] commandParameters)
	//	{
 //           if (!string.IsNullOrEmpty(commandText))
 //           {
 //               return Helper.ExecuteScalar(conn, CommandType.Text, commandText, commandParameters);
 //           }
 //           return null;
	//	}

	//	protected object ExecuteScalar(IDbConnection conn, string commandText)
	//	{
 //           if (!string.IsNullOrEmpty(commandText))
 //           {
 //               return Helper.ExecuteScalar(conn, CommandType.Text, commandText);
 //           }
 //           return null;
	//	}

	//	protected object ExecuteScalar(IDbTransaction trans, string commandText, params IDataParameter[] commandParameters)
	//	{
 //           if (!string.IsNullOrEmpty(commandText))
 //           {
 //               return Helper.ExecuteScalar(trans, CommandType.Text, commandText, commandParameters);
 //           }
 //           return null;
	//	}

	//	protected object ExecuteScalar(IDbTransaction trans, string commandText)
	//	{
 //           if (!string.IsNullOrEmpty(commandText))
 //           {
 //               return Helper.ExecuteScalar(trans, CommandType.Text, commandText);
 //           }
 //           return null;
	//	}

	//	protected object ExecuteScalar(string commandText, params IDataParameter[] commandParameters)
	//	{
 //           if (!string.IsNullOrEmpty(commandText))
 //           {
 //               return Helper.ExecuteScalar(ConnectionString, CommandType.Text, commandText, commandParameters);
 //           }
 //           return null;
	//	}

	//	protected object ExecuteScalar(string commandText)
	//	{
 //           if (!string.IsNullOrEmpty(commandText))
 //           {
 //               return Helper.ExecuteScalar(ConnectionString, CommandType.Text, commandText);
 //           }
 //           return null;
	//	}

 //       protected string GetInsertSqlString(NameValueCollection attributes, string tableName, out IDataParameter[] parms)
 //       {
 //           return BaiRongDataProvider.TableStructureDao.GetInsertSqlString(attributes, ConnectionString, tableName, out parms);
 //       }

 //       protected string GetUpdateSqlString(NameValueCollection attributes, string tableName, out IDataParameter[] parms)
 //       {
 //           return BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(attributes, ConnectionString, tableName, out parms);
 //       }

 //       protected int GetIntResult(string sqlString)
 //       {
 //           return BaiRongDataProvider.DatabaseDao.GetIntResult(ConnectionString, sqlString);
 //       }

 //       protected string GetString(string sqlString)
 //       {
 //           return BaiRongDataProvider.DatabaseDao.GetString(ConnectionString, sqlString);
 //       }

 //       protected ArrayList GetIntArrayList(string sqlString)
 //       {
 //           return BaiRongDataProvider.DatabaseDao.GetIntArrayList(sqlString);
 //       }

 //       protected ArrayList GetIntArrayList(string connectionString, string sqlString)
 //       {
 //           return BaiRongDataProvider.DatabaseDao.GetIntArrayList(connectionString, sqlString);
 //       }

 //       protected ArrayList GetStringArrayList(string sqlString)
 //       {
 //           return BaiRongDataProvider.DatabaseDao.GetStringArrayList(sqlString);
 //       }

 //       protected ArrayList GetStringArrayList(string connectionString, string sqlString)
 //       {
 //           return BaiRongDataProvider.DatabaseDao.GetStringArrayList(connectionString, sqlString);
 //       }

 //       protected string GetSelectSqlString(string tableName, string columns, string where)
 //       {
 //           return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, tableName, 0, columns, where, null);
 //       }

 //       protected string GetSelectSqlString(string connectionString, string tableName, int totalNum, string columns, string whereString, string orderByString)
 //       {
 //           return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(connectionString, tableName, totalNum, columns, whereString, orderByString);
 //       }

 //       protected string GetSelectSqlString(string connectionString, string tableName, int startNum, int totalNum, string columns, string whereString, string orderByString)
 //       {
 //           return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(connectionString, tableName, startNum, totalNum, columns, whereString, orderByString);
 //       }

 //       protected void ReadResultsToNameValueCollection(IDataReader rdr, NameValueCollection attributes)
 //       {
 //           BaiRongDataProvider.DatabaseDao.ReadResultsToNameValueCollection(rdr, attributes);
 //       }

 //       protected void ReadResultsToExtendedAttributes(IDataReader rdr, ExtendedAttributes attributes)
 //       {
 //           BaiRongDataProvider.DatabaseDao.ReadResultsToExtendedAttributes(rdr, attributes);
 //       }
	//}
}
