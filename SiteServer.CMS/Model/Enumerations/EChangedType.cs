using System;

namespace SiteServer.CMS.Model.Enumerations
{
	public enum EChangedType
	{
		Add,
		Edit,
        Delete,
        None
	}

	public class EChangedTypeUtils
	{
		public static string GetValue(EChangedType type)
		{
		    if (type == EChangedType.Add)
			{
                return "Add";
			}
		    if (type == EChangedType.Edit)
		    {
		        return "Edit";
		    }
		    if (type == EChangedType.Delete)
		    {
		        return "Delete";
		    }
		    if (type == EChangedType.None)
		    {
		        return "None";
		    }
		    throw new Exception();
		}

		public static string GetText(EChangedType type)
		{
		    if (type == EChangedType.Add)
			{
				return "新增";
			}
		    if (type == EChangedType.Edit)
		    {
		        return "修改";
		    }
		    if (type == EChangedType.Delete)
		    {
		        return "删除";
		    }
		    if (type == EChangedType.None)
		    {
		        return "";
		    }
		    throw new Exception();
		}

		public static EChangedType GetEnumType(string typeStr)
		{
            var retval = EChangedType.Edit;

            if (Equals(EChangedType.Add, typeStr))
			{
                retval = EChangedType.Add;
			}
            else if (Equals(EChangedType.Edit, typeStr))
			{
                retval = EChangedType.Edit;
            }
            else if (Equals(EChangedType.Delete, typeStr))
            {
                retval = EChangedType.Delete;
            }
            else if (Equals(EChangedType.None, typeStr))
            {
                retval = EChangedType.None;
            }
            return retval;
		}

		public static bool Equals(EChangedType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, EChangedType type)
        {
            return Equals(type, typeStr);
        }
	}
}
