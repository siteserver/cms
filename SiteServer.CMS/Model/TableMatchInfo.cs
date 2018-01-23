using System;
using System.Collections.Specialized;

namespace SiteServer.CMS.Model
{
    [Serializable]
	public class TableMatchInfo
	{
		private int _id;
		private string _connectionString;
		private string _tableName;
		private string _connectionStringToMatch;
		private string _tableNameToMatch;
		private NameValueCollection _columnsMap;

		public TableMatchInfo()
		{
			_id = 0;
			_connectionString = string.Empty;
			_tableName = string.Empty;
			_connectionStringToMatch = string.Empty;
			_tableNameToMatch = string.Empty;
			_columnsMap = new NameValueCollection();
		}

		public TableMatchInfo(int id, string connectionString, string tableName, string connectionStringToMatch, string tableNameToMatch, NameValueCollection columnsMap) 
		{
            _id = id;
			_connectionString = connectionString;
			_tableName = tableName;
			_connectionStringToMatch = connectionStringToMatch;
			_tableNameToMatch = tableNameToMatch;
			_columnsMap = columnsMap;
		}

		public int Id
		{
			get{ return _id; }
			set{ _id = value; }
		}

		public string ConnectionString
		{
			get{ return _connectionString; }
			set{ _connectionString = value; }
		}

		public string TableName
		{
			get{ return _tableName; }
			set{ _tableName = value; }
		}

		public string ConnectionStringToMatch
		{
			get{ return _connectionStringToMatch; }
			set{ _connectionStringToMatch = value; }
		}

		public string TableNameToMatch
		{
			get{ return _tableNameToMatch; }
			set{ _tableNameToMatch = value; }
		}

		public NameValueCollection ColumnsMap
		{
			get{ return _columnsMap; }
			set{ _columnsMap = value; }
		}
	}
}
