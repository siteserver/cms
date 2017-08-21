using SiteServer.Plugin;
using SiteServer.Plugin.Models;

namespace BaiRong.Core.Model
{
	public class TableColumnInfo
	{
	    public TableColumnInfo()
		{
			DatabaseName = string.Empty;
			TableId = string.Empty;
			ColumnName = string.Empty;
			DataType = DataType.NVarChar;
			Length = 0;
			Precision = 0;
			Scale = 0;
			IsPrimaryKey = false;
			IsNullable = false;
			IsIdentity = false;
		}

		public TableColumnInfo(string databaseName, string tableId, string columnName, DataType dataType, int length, int precision, int scale, bool isPrimaryKey, bool isNullable, bool isIdentity) 
		{
			DatabaseName = databaseName;
			TableId = tableId;
			ColumnName = columnName;
			DataType = dataType;
			Length = length;
			Precision = precision;
			Scale = scale;
			IsPrimaryKey = isPrimaryKey;
			IsNullable = isNullable;
			IsIdentity = isIdentity;
		}

		public string DatabaseName { get; set; }

	    public string TableId { get; set; }

	    public string ColumnName { get; set; }

	    public DataType DataType { get; set; }

	    public int Length { get; set; }

	    public int Precision { get; set; }

	    public int Scale { get; set; }

	    public bool IsPrimaryKey { get; set; }

	    public bool IsNullable { get; set; }

	    public bool IsIdentity { get; set; }
	}
}
