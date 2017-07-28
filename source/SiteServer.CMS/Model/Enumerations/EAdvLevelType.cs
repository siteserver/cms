using System;
using System.Web.UI.WebControls;

namespace SiteServer.CMS.Model.Enumerations
{
    public enum EAdvLevelType
    {
        Hold,           //独占
        Standard       //标准
    }

    public class EAdvLevelTypeUtils
    {
        public static string GetValue(EAdvLevelType type)
        {
            if (type == EAdvLevelType.Hold)
            {
                return "Hold";
            }
            else if (type == EAdvLevelType.Standard)
            {
                return "Standard";
            }
           else
            {
                throw new Exception();
            }
        }

        public static string GetText(EAdvLevelType type)
        {
            if (type == EAdvLevelType.Hold )
            {
                return "独占";
            }
            else if (type == EAdvLevelType.Standard)
            {
                return "标准";
            }
           else
            {
                throw new Exception();
            }
        }

        public static EAdvLevelType GetEnumType(string typeStr)
        {
            var retval = EAdvLevelType.Hold;

            if (Equals(EAdvLevelType.Hold, typeStr))
            {
                retval = EAdvLevelType.Hold;
            }
            else if (Equals(EAdvLevelType.Standard, typeStr))
            {
                retval = EAdvLevelType.Standard;
            }
          
            return retval;
        }

        public static bool Equals(EAdvLevelType type, string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return false;
            if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool Equals(string typeStr, EAdvType type)
        {
            return Equals(type, typeStr);
        }

        public static ListItem GetListItem(EAdvLevelType type, bool selected)
        {
            var item = new ListItem(GetText(type), GetValue(type));
            if (selected)
            {
                item.Selected = true;
            }
            return item;
        }

        public static void AddListItems(ListControl listControl)
        {
            if (listControl != null)
            {
                listControl.Items.Add(GetListItem(EAdvLevelType.Hold, false));
                listControl.Items.Add(GetListItem(EAdvLevelType.Standard, false));
                
            }
        }
    }
}
