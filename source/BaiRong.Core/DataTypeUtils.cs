using System;
using System.Data;
using System.Data.OleDb;
using System.Web.UI.WebControls;
using BaiRong.Core.Model.Enumerations;
using MySql.Data.MySqlClient;
using SiteServer.Plugin;
using SiteServer.Plugin.Models;

namespace BaiRong.Core
{
	public class DataTypeUtils
	{
		public static string GetValue(DataType type)
		{
		    if (type == DataType.Bit)
			{
				return "Bit";
			}
		    if (type == DataType.Char)
		    {
		        return "Char";
		    }
		    if (type == DataType.DateTime)
		    {
		        return "DateTime";
		    }
		    if (type == DataType.Decimal)
		    {
		        return "Decimal";
		    }
		    if (type == DataType.Float)
		    {
		        return "Float";
		    }
		    if (type == DataType.Integer)
		    {
		        return "Integer";
		    }
		    if (type == DataType.NChar)
		    {
		        return "NChar";
		    }
		    if (type == DataType.NText)
		    {
		        return "NText";
		    }
		    if (type == DataType.NVarChar)
		    {
		        return "NVarChar";
		    }
		    if (type == DataType.Text)
		    {
		        return "Text";
		    }
		    if (type == DataType.VarChar)
		    {
		        return "VarChar";
		    }
		    throw new Exception();
		}

		public static DataType GetEnumType(string typeStr)
		{
			var retval = DataType.NVarChar;

			if (Equals(DataType.Bit, typeStr))
			{
				retval = DataType.Bit;
			}
			else if (Equals(DataType.Char, typeStr))
			{
				retval = DataType.Char;
			}
			else if (Equals(DataType.DateTime, typeStr))
			{
				retval = DataType.DateTime;
			}
			else if (Equals(DataType.Decimal, typeStr))
			{
				retval = DataType.Decimal;
			}
			else if (Equals(DataType.Float, typeStr))
			{
				retval = DataType.Float;
			}
			else if (Equals(DataType.Integer, typeStr))
			{
				retval = DataType.Integer;
			}
			else if (Equals(DataType.NChar, typeStr))
			{
				retval = DataType.NChar;
			}
			else if (Equals(DataType.NText, typeStr))
			{
				retval = DataType.NText;
			}
			else if (Equals(DataType.NVarChar, typeStr))
			{
				retval = DataType.NVarChar;
			}
			else if (Equals(DataType.Text, typeStr))
			{
				retval = DataType.Text;
			}
			else if (Equals(DataType.VarChar, typeStr))
			{
				retval = DataType.VarChar;
			}

			return retval;
		}

		public static bool Equals(DataType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, DataType type)
        {
            return Equals(type, typeStr);
        }

		public static DataType FromMySql(string typeStr)
		{
			var dataType = DataType.NVarChar;
			if (!string.IsNullOrEmpty(typeStr))
			{
				typeStr = typeStr.ToLower().Trim();
				switch (typeStr)
				{
					case "bit":
						dataType = DataType.Bit;
						break;
					case "char":
						dataType = DataType.Char;
						break;
					case "datetime":
						dataType = DataType.DateTime;
						break;
					case "decimal":
						dataType = DataType.Decimal;
						break;
					case "float":
						dataType = DataType.Float;
						break;
					case "int":
						dataType = DataType.Integer;
						break;
					case "nchar":
						dataType = DataType.NChar;
						break;
					case "longtext":
						dataType = DataType.NText;
						break;
					case "nvarchar":
						dataType = DataType.NVarChar;
						break;
					case "text":
						dataType = DataType.Text;
						break;
					case "varchar":
						dataType = DataType.VarChar;
						break;
				}
			}
			return dataType;
		}

        public static DataType FromSqlServer(string typeStr)
        {
            var dataType = DataType.NVarChar;
            if (!string.IsNullOrEmpty(typeStr))
            {
                typeStr = typeStr.ToLower().Trim();
                switch (typeStr)
                {
                    case "bit":
                        dataType = DataType.Bit;
                        break;
                    case "char":
                        dataType = DataType.Char;
                        break;
                    case "datetime":
                        dataType = DataType.DateTime;
                        break;
                    case "decimal":
                        dataType = DataType.Decimal;
                        break;
                    case "float":
                        dataType = DataType.Float;
                        break;
                    case "int":
                        dataType = DataType.Integer;
                        break;
                    case "nchar":
                        dataType = DataType.NChar;
                        break;
                    case "ntext":
                        dataType = DataType.NText;
                        break;
                    case "nvarchar":
                        dataType = DataType.NVarChar;
                        break;
                    case "text":
                        dataType = DataType.Text;
                        break;
                    case "varchar":
                        dataType = DataType.VarChar;
                        break;
                }
            }
            return dataType;
        }

        public static DataType FromAccess(OleDbType oleDbType)
		{
			var dataType = DataType.NVarChar;
			
			switch (oleDbType)
			{
				case OleDbType.Boolean:
					dataType = DataType.Bit;
					break;
				case OleDbType.BSTR:
					dataType = DataType.VarChar;
					break;
				case OleDbType.Char:
					dataType = DataType.Char;
					break;
				case OleDbType.Date:
					dataType = DataType.DateTime;
					break;
				case OleDbType.DBDate:
					dataType = DataType.DateTime;
					break;
				case OleDbType.Decimal:
					dataType = DataType.Decimal;
					break;
				case OleDbType.Filetime:
					dataType = DataType.DateTime;
					break;
				case OleDbType.Integer:
					dataType = DataType.Integer;
					break;
				case OleDbType.LongVarChar:
					dataType = DataType.Text;
					break;
				case OleDbType.LongVarWChar:
					dataType = DataType.NVarChar;
					break;
				case OleDbType.Single:
					dataType = DataType.Float;
					break;
				case OleDbType.UnsignedInt:
					dataType = DataType.Integer;
					break;
				case OleDbType.VarChar:
					dataType = DataType.VarChar;
					break;
				case OleDbType.VarWChar:
					dataType = DataType.NVarChar;
					break;
				case OleDbType.WChar:
					dataType = DataType.NChar;
					break;
			}

			return dataType;
		}

        public static DataType FromOracle(string typeStr)
        {
            var dataType = DataType.NVarChar;

            if (!string.IsNullOrEmpty(typeStr))
            {
                typeStr = typeStr.ToUpper().Trim();
                switch (typeStr)
                {
                    case "CHAR":
                        dataType = DataType.Char;
                        break;
                    case "TIMESTAMP(6)":
                        dataType = DataType.DateTime;
                        break;
                    case "TIMESTAMP(8)":
                        dataType = DataType.DateTime;
                        break;
                    case "NUMBER":
                        dataType = DataType.Integer;
                        break;
                    case "NCHAR":
                        dataType = DataType.NChar;
                        break;
                    case "NCLOB":
                        dataType = DataType.NText;
                        break;
                    case "NVARCHAR2":
                        dataType = DataType.NVarChar;
                        break;
                    case "CLOB":
                        dataType = DataType.Text;
                        break;
                    case "VARCHAR2":
                        dataType = DataType.VarChar;
                        break;
                }
            }

            return dataType;
        }

		public static SqlDbType ToSqlDbType(DataType type)
		{
		    if (type == DataType.Bit)
			{
				return SqlDbType.Bit;
			}
		    if (type == DataType.Char)
		    {
		        return SqlDbType.Char;
		    }
		    if (type == DataType.DateTime)
		    {
		        return SqlDbType.DateTime;
		    }
		    if (type == DataType.Decimal)
		    {
		        return SqlDbType.Decimal;
		    }
		    if (type == DataType.Float)
		    {
		        return SqlDbType.Float;
		    }
		    if (type == DataType.Integer)
		    {
		        return SqlDbType.Int;
		    }
		    if (type == DataType.NChar)
		    {
		        return SqlDbType.NChar;
		    }
		    if (type == DataType.NText)
		    {
		        return SqlDbType.NText;
		    }
		    if (type == DataType.NVarChar)
		    {
		        return SqlDbType.NVarChar;
		    }
		    if (type == DataType.Text)
		    {
		        return SqlDbType.Text;
		    }
            return SqlDbType.VarChar;
        }

        public static MySqlDbType ToMySqlDbType(DataType type)
        {
            if (type == DataType.Bit)
            {
                return MySqlDbType.Bit;
            }
            if (type == DataType.Char)
            {
                return MySqlDbType.String;
            }
            if (type == DataType.DateTime)
            {
                return MySqlDbType.DateTime;
            }
            if (type == DataType.Decimal)
            {
                return MySqlDbType.Decimal;
            }
            if (type == DataType.Float)
            {
                return MySqlDbType.Float;
            }
            if (type == DataType.Integer)
            {
                return MySqlDbType.Int32;
            }
            if (type == DataType.NChar)
            {
                return MySqlDbType.String;
            }
            if (type == DataType.NText)
            {
                return MySqlDbType.LongText;
            }
            if (type == DataType.NVarChar)
            {
                return MySqlDbType.VarString;
            }
            if (type == DataType.Text)
            {
                return MySqlDbType.Text;
            }
            return MySqlDbType.VarString;
        }

        public static ListItem GetListItem(DataType type, string text)
		{
			var item = new ListItem(text, GetValue(type));
			return item;
		}

		public static void AddListItemsToAuxiliaryTable(ListControl listControl)
		{
		    if (listControl == null) return;

		    listControl.Items.Add(GetListItem(DataType.NVarChar, "文本"));
		    listControl.Items.Add(GetListItem(DataType.NText, "备注"));
		    listControl.Items.Add(GetListItem(DataType.Integer, "数字"));
		    listControl.Items.Add(GetListItem(DataType.DateTime, "日期/时间"));
		}

        public static string GetTextByAuxiliaryTable(DataType dataType, int dataLength)
        {
            var retval = string.Empty;
            if (dataType == DataType.NVarChar)
            {
                retval = $"文本({dataLength})";
            }
            else if (dataType == DataType.VarChar)
            {
                retval = $"文本({dataLength})";
            }
            else if (dataType == DataType.NChar)
            {
                retval = $"文本({dataLength})";
            }
            else if (dataType == DataType.NText)
            {
                retval = "备注";
            }
            else if (dataType == DataType.Text)
            {
                retval = "备注";
            }
            else if (dataType == DataType.Integer)
            {
                retval = "数字";
            }
            else if (dataType == DataType.DateTime)
            {
                retval = "日期/时间";
            }
            else if (dataType == DataType.Decimal)
            {
                retval = "小数";
            }
            return retval;
        }

        public static string GetDefaultString(string stringValue)
        {
            return $"'{stringValue}'";
        }

		public static string GetDefaultString(DataType dataType)
		{
			var retval = string.Empty;

            if (dataType == DataType.Char || dataType == DataType.NChar || dataType == DataType.NText || dataType == DataType.NVarChar || dataType == DataType.Text || dataType == DataType.VarChar)
            {
                return "''";
            }
		    if (dataType == DataType.Decimal || dataType == DataType.Decimal || dataType == DataType.Float || dataType == DataType.Integer)
		    {
		        return "0";
		    }
		    if (dataType == DataType.DateTime)
		    {
		        return WebConfigUtils.DatabaseType == EDatabaseType.MySql ? "now()" : "getdate()";
		    }

		    return retval;
		}
	}
}
