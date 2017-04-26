using System;
using BaiRong.Core.Model.Enumerations;

namespace BaiRong.Core.Model
{
	[Serializable]
	public class TableMetadataInfo
	{
		private int _tableMetadataId;
		private string _auxiliaryTableEnName;
		private string _attributeName;
		private EDataType _dataType;
		private int _dataLength;
		private int _taxis;
        private bool _isSystem;

		public TableMetadataInfo()
		{
			_tableMetadataId = 0;
			_auxiliaryTableEnName = string.Empty;
			_attributeName = string.Empty;
			_dataType = EDataType.VarChar;
			_dataLength = 50;
			_taxis = 0;
			_isSystem = false;
		}

        public TableMetadataInfo(int tableMetadataId, string auxiliaryTableEnName, string attributeName, EDataType dataType, int dataLength, int taxis, bool isSystem) 
		{
			_tableMetadataId = tableMetadataId;
			_auxiliaryTableEnName = auxiliaryTableEnName;
			_attributeName = attributeName;
			_dataType = dataType;
			_dataLength = dataLength;
			_taxis = taxis;
			_isSystem = isSystem;
		}

		public int TableMetadataId
		{
			get{ return _tableMetadataId; }
			set{ _tableMetadataId = value; }
		}

		public string AuxiliaryTableEnName
		{
			get{ return _auxiliaryTableEnName; }
			set{ _auxiliaryTableEnName = value; }
		}

		public string AttributeName
		{
			get{ return _attributeName; }
			set{ _attributeName = value; }
		}

		public EDataType DataType
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
