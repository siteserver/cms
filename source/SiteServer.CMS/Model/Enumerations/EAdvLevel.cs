using System;
using System.Web.UI.WebControls;

namespace SiteServer.CMS.Model.Enumerations
{
    public enum EAdvLevel
    {
        Level1,       //1级
        Level2,       //2级
        Level3,       //3级
        Level4,       //4级
        Level5,       //5级
        Level6,       //6级
        Level7,       //7级
        Level8,       //8级
        Level9,       //9级
        Level10       //10级
    }

    public class EAdvLevelUtils
    {
        public static string GetValue(EAdvLevel type)
        {
            if (type == EAdvLevel.Level1)
            {
                return "1";
            }
            else if (type == EAdvLevel.Level2)
            {
                return "2";
            }
            else if (type == EAdvLevel.Level3)
            {
                return "3";
            }
            else if (type == EAdvLevel.Level4)
            {
                return "4";
            }
            else if (type == EAdvLevel.Level5)
            {
                return "5";
            }
            else if (type == EAdvLevel.Level6)
            {
                return "6";
            }
            else if (type == EAdvLevel.Level7)
            {
                return "7";
            }
            else if (type == EAdvLevel.Level8)
            {
                return "8";
            }
            else if (type == EAdvLevel.Level9)
            {
                return "9";
            }
            else if (type == EAdvLevel.Level10)
            {
                return "10";
            }
           else
            {
                throw new Exception();
            }
        }

        public static string GetText(EAdvLevel type)
        {
            if (type == EAdvLevel.Level1)
            {
                return "1级";
            } 
            else if (type == EAdvLevel.Level2)
            {
                return "2级";
            }
            else if (type == EAdvLevel.Level3)
            {
                return "3级";
            }
            else if (type == EAdvLevel.Level4)
            {
                return "4级";
            }
            else if (type == EAdvLevel.Level5)
            {
                return "5级";
            }
            else if (type == EAdvLevel.Level6)
            {
                return "6级";
            }
            else if (type == EAdvLevel.Level7)
            {
                return "7级";
            }
            else if (type == EAdvLevel.Level8)
            {
                return "8级";
            }
            else if (type == EAdvLevel.Level9)
            {
                return "9级";
            }
            else if (type == EAdvLevel.Level10)
            {
                return "10级";
            }
            else
            {
                throw new Exception();
            }
        }

        public static EAdvLevel GetEnumType(string typeStr)
        {
            var retval = EAdvLevel.Level1;

            if (Equals(EAdvLevel.Level1, typeStr))
            {
                retval = EAdvLevel.Level1;
            }
            else if (Equals(EAdvLevel.Level2, typeStr))
            {
                retval = EAdvLevel.Level2;
            }
            else if (Equals(EAdvLevel.Level3, typeStr))
            {
                retval = EAdvLevel.Level3;
            }
            else if (Equals(EAdvLevel.Level4, typeStr))
            {
                retval = EAdvLevel.Level4;
            }
            else if (Equals(EAdvLevel.Level5, typeStr))
            {
                retval = EAdvLevel.Level5;
            }
            else if (Equals(EAdvLevel.Level6, typeStr))
            {
                retval = EAdvLevel.Level6;
            }
            else if (Equals(EAdvLevel.Level7, typeStr))
            {
                retval = EAdvLevel.Level7;
            }
            else if (Equals(EAdvLevel.Level8, typeStr))
            {
                retval = EAdvLevel.Level8;
            }
            else if (Equals(EAdvLevel.Level9, typeStr))
            {
                retval = EAdvLevel.Level9;
            }
            else if (Equals(EAdvLevel.Level10, typeStr))
            {
                retval = EAdvLevel.Level10;
            }
            return retval;
        }

        public static bool Equals(EAdvLevel type, string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return false;
            if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool Equals(string typeStr, EAdvLevel type)
        {
            return Equals(type, typeStr);
        }

        public static ListItem GetListItem(EAdvLevel type, bool selected)
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
                listControl.Items.Add(GetListItem(EAdvLevel.Level1, false));
                listControl.Items.Add(GetListItem(EAdvLevel.Level2, false));
                listControl.Items.Add(GetListItem(EAdvLevel.Level3, false));
                listControl.Items.Add(GetListItem(EAdvLevel.Level4, false));
                listControl.Items.Add(GetListItem(EAdvLevel.Level5, false));
                listControl.Items.Add(GetListItem(EAdvLevel.Level6, false));
                listControl.Items.Add(GetListItem(EAdvLevel.Level7, false));
                listControl.Items.Add(GetListItem(EAdvLevel.Level8, false));
                listControl.Items.Add(GetListItem(EAdvLevel.Level9, false));
                listControl.Items.Add(GetListItem(EAdvLevel.Level10, false));
            }
        }
    }
}
