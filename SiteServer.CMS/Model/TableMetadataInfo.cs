using System;
using SiteServer.Plugin;

namespace SiteServer.CMS.Model
{
	[Serializable]
	public class TableMetadataInfo
	{
		private int _id;
		private string _tableName;
		private string _attributeName;
		private DataType _dataType;
		private int _dataLength;
		private int _taxis;
        private bool _isSystem;

		public TableMetadataInfo()
		{
            _id = 0;
            _tableName = string.Empty;
			_attributeName = string.Empty;
			_dataType = DataType.VarChar;
			_dataLength = 50;
			_taxis = 0;
			_isSystem = false;
		}

        public TableMetadataInfo(int id, string tableName, string attributeName, DataType dataType, int dataLength, int taxis, bool isSystem) 
		{
            _id = id;
            _tableName = tableName;
			_attributeName = attributeName;
			_dataType = dataType;
			_dataLength = dataLength;
			_taxis = taxis;
			_isSystem = isSystem;
		}

		public int Id
		{
			get{ return _id; }
			set{ _id = value; }
		}

		public string TableName
		{
			get{ return _tableName; }
			set{ _tableName = value; }
		}

		public string AttributeName
		{
			get{ return _attributeName; }
			set{ _attributeName = value; }
		}

		public DataType DataType
		{
			get{ return _dataType; }
			set{ _dataType = value; }
		}

		public int DataLength
		{
			get{ return _dataLength; }
			set{ _dataLength = value; }
		}

		public int Taxis
		{
			get{ return _taxis; }
			set{ _taxis = value; }
		}

        public bool IsSystem
		{
			get{ return _isSystem; }
			set{ _isSystem = value; }
		}

        private TableStyleInfo _styleInfo;
        public TableStyleInfo StyleInfo
        {
            get { return _styleInfo; }
            set { _styleInfo = value; }
        }


	}
}
