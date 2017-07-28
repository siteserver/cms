using System;

namespace SiteServer.CMS.Model.Enumerations
{
	
	public enum ENodeType
	{
        BackgroundPublishNode,	//应用
        BackgroundNormalNode,   //栏目
    }

	public class ENodeTypeUtils
	{
		public static string GetValue(ENodeType type)
		{
			if (type == ENodeType.BackgroundPublishNode)
			{
				return "BackgroundPublishNode";
			}
			else if (type == ENodeType.BackgroundNormalNode)
			{
				return "BackgroundNormalNode";
            }
			else
			{
				throw new Exception();
			}
		}

		public static ENodeType GetEnumType(string typeStr)
		{
			var retval = ENodeType.BackgroundNormalNode;

			if (Equals(ENodeType.BackgroundPublishNode, typeStr))
			{
				retval = ENodeType.BackgroundPublishNode;
			}
			else if (Equals(ENodeType.BackgroundNormalNode, typeStr))
			{
				retval = ENodeType.BackgroundNormalNode;
            }

			return retval;
		}

		public static bool Equals(ENodeType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

		public static bool Equals(string typeStr, ENodeType type)
		{
			return Equals(type, typeStr);
		}
	}
}
