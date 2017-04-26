using System;
using System.Data;
using System.Data.OleDb;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

namespace BaiRong.Core.Model.Enumerations
{
	public enum EDataType
	{
        Bit,            //boolean
        Char,           //string
        DateTime,       //system.datetime
        Decimal,        //system.decimal
        Float,          //system.double
        Integer,        //int32
        NChar,          //string
        NText,          //string
        NVarChar,       //string
        Text,           //string
        VarChar,        //string
		Unknown
	}

	public class EDataTypeUtils
	{
		public static string GetValue(EDataType type)
		{
		    if (type == EDataType.Bit)
			{
				return "Bit";
			}
		    if (type == EDataType.Char)
		    {
		        return "Char";
		    }
		    if (type == EDataType.DateTime)
		    {
		        return "DateTime";
		    }
		    if (type == EDataType.Decimal)
		    {
		        return "Decimal";
		    }
		    if (type == EDataType.Float)
		    {
		        return "Float";
		    }
		    if (type == EDataType.Integer)
		    {
		        return "Integer";
		    }
		    if (type == EDataType.NChar)
		    {
		        return "NChar";
		    }
		    if (type == EDataType.NText)
		    {
		        return "NText";
		    }
		    if (type == EDataType.NVarChar)
		    {
		        return "NVarChar";
		    }
		    if (type == EDataType.Text)
		    {
		        return "Text";
		    }
		    if (type == EDataType.VarChar)
		    {
		        return "VarChar";
		    }
		    if (type == EDataType.Unknown)
		    {
		        return "Unknown";
		    }
		    throw new Exception();
		}

		public static EDataType GetEnumType(string typeStr)
		{
			var retval = EDataType.Unknown;

			if (Equals(EDataType.Bit, typeStr))
			{
				retval = EDataType.Bit;
			}
			else if (Equals(EDataType.Char, typeStr))
			{
				retval = EDataType.Char;
			}
			else if (Equals(EDataType.DateTime, typeStr))
			{
				retval = EDataType.DateTime;
			}
			else if (Equals(EDataType.Decimal, typeStr))
			{
				retval = EDataType.Decimal;
			}
			else if (Equals(EDataType.Float, typeStr))
			{
				retval = EDataType.Float;
			}
			else if (Equals(EDataType.Integer, typeStr))
			{
				retval = EDataType.Integer;
			}
			else if (Equals(EDataType.NChar, typeStr))
			{
				retval = EDataType.NChar;
			}
			else if (Equals(EDataType.NText, typeStr))
			{
				retval = EDataType.NText;
			}
			else if (Equals(EDataType.NVarChar, typeStr))
			{
				retval = EDataType.NVarChar;
			}
			else if (Equals(EDataType.Text, typeStr))
			{
				retval = EDataType.Text;
			}
			else if (Equals(EDataType.VarChar, typeStr))
			{
				retval = EDataType.VarChar;
			}
			else if (Equals(EDataType.Unknown, typeStr))
			{
				retval = EDataType.Unknown;
			}

			return retval;
		}

		public static bool Equals(EDataType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, EDataType type)
        {
            return Equals(type, typeStr);
        }

		public static EDataType FromMySql(string typeStr)
		{
			var dataType = EDataType.Unknown;
			if (!string.IsNullOrEmpty(typeStr))
			{
				typeStr = typeStr.ToLower().Trim();
				switch (typeStr)
				{
					case "bit":
						dataType = EDataType.Bit;
						break;
					case "char":
						dataType = EDataType.Char;
						break;
					case "datetime":
						dataType = EDataType.DateTime;
						break;
					case "decimal":
						dataType = EDataType.Decimal;
						break;
					case "float":
						dataType = EDataType.Float;
						break;
					case "int":
						dataType = EDataType.Integer;
						break;
					case "nchar":
						dataType = EDataType.NChar;
						break;
					case "longtext":
						dataType = EDataType.NText;
						break;
					case "nvarchar":
						dataType = EDataType.NVarChar;
						break;
					case "text":
						dataType = EDataType.Text;
						break;
					case "varchar":
						dataType = EDataType.VarChar;
						break;
				}
			}
			return dataType;
		}

        public static EDataType FromSqlServer(string typeStr)
        {
            var dataType = EDataType.Unknown;
            if (!string.IsNullOrEmpty(typeStr))
            {
                typeStr = typeStr.ToLower().Trim();
                switch (typeStr)
                {
                    case "bit":
                        dataType = EDataType.Bit;
                        break;
                    case "char":
                        dataType = EDataType.Char;
                        break;
                    case "datetime":
                        dataType = EDataType.DateTime;
                        break;
                    case "decimal":
                        dataType = EDataType.Decimal;
                        break;
                    case "float":
                        dataType = EDataType.Float;
                        break;
                    case "int":
                        dataType = EDataType.Integer;
                        break;
                    case "nchar":
                        dataType = EDataType.NChar;
                        break;
                    case "ntext":
                        dataType = EDataType.NText;
                        break;
                    case "nvarchar":
                        dataType = EDataType.NVarChar;
                        break;
                    case "text":
                        dataType = EDataType.Text;
                        break;
                    case "varchar":
                        dataType = EDataType.VarChar;
                        break;
                }
            }
            return dataType;
        }

        public static EDataType FromAccess(OleDbType oleDbType)
		{
			var dataType = EDataType.Unknown;
			
			switch (oleDbType)
			{
				case OleDbType.Boolean:
					dataType = EDataType.Bit;
					break;
				case OleDbType.BSTR:
					dataType = EDataType.VarChar;
					break;
				case OleDbType.Char:
					dataType = EDataType.Char;
					break;
				case OleDbType.Date:
					dataType = EDataType.DateTime;
					break;
				case OleDbType.DBDate:
					dataType = EDataType.DateTime;
					break;
				case OleDbType.Decimal:
					dataType = EDataType.Decimal;
					break;
				case OleDbType.Filetime:
					dataType = EDataType.DateTime;
					break;
				case OleDbType.Integer:
					dataType = EDataType.Integer;
					break;
				case OleDbType.LongVarChar:
					dataType = EDataType.Text;
					break;
				case OleDbType.LongVarWChar:
					dataType = EDataType.NVarChar;
					break;
				case OleDbType.Single:
					dataType = EDataType.Float;
					break;
				case OleDbType.UnsignedInt:
					dataType = EDataType.Integer;
					break;
				case OleDbType.VarChar:
					dataType = EDataType.VarChar;
					break;
				case OleDbType.VarWChar:
					dataType = EDataType.NVarChar;
					break;
				case OleDbType.WChar:
					dataType = EDataType.NChar;
					break;
			}

			return dataType;
		}

        public static EDataType FromOracle(string typeStr)
        {
            var dataType = EDataType.Unknown;

            if (!string.IsNullOrEmpty(typeStr))
            {
                typeStr = typeStr.ToUpper().Trim();
                switch (typeStr)
                {
                    case "CHAR":
                        dataType = EDataType.Char;
                        break;
                    case "TIMESTAMP(6)":
                        dataType = EDataType.DateTime;
                        break;
                    case "TIMESTAMP(8)":
                        dataType = EDataType.DateTime;
                        break;
                    case "NUMBER":
                        dataType = EDataType.Integer;
                        break;
                    case "NCHAR":
                        dataType = EDataType.NChar;
                        break;
                    case "NCLOB":
                        dataType = EDataType.NText;
                        break;
                    case "NVARCHAR2":
                        dataType = EDataType.NVarChar;
                        break;
                    case "CLOB":
                        dataType = EDataType.Text;
                        break;
                    case "VARCHAR2":
                        dataType = EDataType.VarChar;
                        break;
                }
            }

            return dataType;
        }

		public static SqlDbType ToSqlDbType(EDataType type)
		{
		    if (type == EDataType.Bit)
			{
				return SqlDbType.Bit;
			}
		    if (type == EDataType.Char)
		    {
		        return SqlDbType.Char;
		    }
		    if (type == EDataType.DateTime)
		    {
		        return SqlDbType.DateTime;
		    }
		    if (type == EDataType.Decimal)
		    {
		        return SqlDbType.Decimal;
		    }
		    if (type == EDataType.Float)
		    {
		        return SqlDbType.Float;
		    }
		    if (type == EDataType.Integer)
		    {
		        return SqlDbType.Int;
		    }
		    if (type == EDataType.NChar)
		    {
		        return SqlDbType.NChar;
		    }
		    if (type == EDataType.NText)
		    {
		        return SqlDbType.NText;
		    }
		    if (type == EDataType.NVarChar)
		    {
		        return SqlDbType.NVarChar;
		    }
		    if (type == EDataType.Text)
		    {
		        return SqlDbType.Text;
		    }
            return SqlDbType.VarChar;
        }

        public static MySqlDbType ToMySqlDbType(EDataType type)
        {
            if (type == EDataType.Bit)
            {
                return MySqlDbType.Bit;
            }
            if (type == EDataType.Char)
            {
                return MySqlDbType.String;
            }
            if (type == EDataType.DateTime)
            {
                return MySqlDbType.DateTime;
            }
            if (type == EDataType.Decimal)
            {
                return MySqlDbType.Decimal;
            }
            if (type == EDataType.Float)
            {
                return MySqlDbType.Float;
            }
            if (type == EDataType.Integer)
            {
                return MySqlDbType.Int32;
            }
            if (type == EDataType.NChar)
            {
                return MySqlDbType.String;
            }
            if (type == EDataType.NText)
            {
                return MySqlDbType.LongText;
            }
            if (type == EDataType.NVarChar)
            {
                return MySqlDbType.VarString;
            }
            if (type == EDataType.Text)
            {
                return MySqlDbType.Text;
            }
            return MySqlDbType.VarString;
        }

        public static ListItem GetListItem(EDataType type, string text)
		{
			var item = new ListItem(text, GetValue(type));
			return item;
		}

		public static void AddListItemsToAuxiliaryTable(ListControl listControl)
		{
            if (listControl != null)
            {
                listControl.Items.Add(GetListItem(EDataType.NVarChar, "文本"));
                listControl.Items.Add(GetListItem(EDataType.NText, "备注"));
                listControl.Items.Add(GetListItem(EDataType.Integer, "数字"));
                listControl.Items.Add(GetListItem(EDataType.DateTime, "日期/时间"));
            }
		}

        public static string GetTextByAuxiliaryTable(EDataType dataType, int dataLength)
        {
            var retval = string.Empty;
            if (dataType == EDataType.NVarChar)
            {
                retval = $"文本({dataLength})";
            }
            else if (dataType == EDataType.VarChar)
            {
                retval = $"文本({dataLength})";
            }
            else if (dataType == EDataType.NChar)
            {
                retval = $"文本({dataLength})";
            }
            else if (dataType == EDataType.NText)
            {
                retval = "备注";
            }
            else if (dataType == EDataType.Text)
            {
                retval = "备注";
            }
            else if (dataType == EDataType.Integer)
            {
                retval = "数字";
            }
            else if (dataType == EDataType.DateTime)
            {
                retval = "日期/时间";
            }
            else if (dataType == EDataType.Decimal)
            {
                retval = "小数";
            }
            return retval;
        }

        public static string GetDefaultString(string stringValue)
        {
            return $"'{stringValue}'";
        }

		public static string GetDefaultString(EDataType dataType)
		{
			var retval = string.Empty;

            if (dataType == EDataType.Char || dataType == EDataType.NChar || dataType == EDataType.NText || dataType == EDataType.NVarChar || dataType == EDataType.Text || dataType == EDataType.VarChar)
            {
                return "''";
            }
		    if (dataType == EDataType.Decimal || dataType == EDataType.Decimal || dataType == EDataType.Float || dataType == EDataType.Integer)
		    {
		        return "0";
		    }
		    if (dataType == EDataType.DateTime)
		    {
		        return WebConfigUtils.IsMySql ? "now()" : "getdate()";
		    }

		    return retval;
		}
	}
}
