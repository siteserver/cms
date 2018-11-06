using System;
using System.Web.UI.WebControls;

namespace SiteServer.Utils.Enumerations
{
    public enum EUserPasswordRestriction
    {
        None,
        LetterAndDigit,
        LetterAndDigitAndSymbol
    }

    public static class EUserPasswordRestrictionUtils
    {
        public static string GetValue(EUserPasswordRestriction type)
        {
            if (type == EUserPasswordRestriction.None)
            {
                return "None";
            }
            if (type == EUserPasswordRestriction.LetterAndDigit)
            {
                return "LetterAndDigit";
            }
            if (type == EUserPasswordRestriction.LetterAndDigitAndSymbol)
            {
                return "LetterAndDigitAndSymbol";
            }
            throw new Exception();
        }

        public static string GetText(EUserPasswordRestriction type)
        {
            if (type == EUserPasswordRestriction.None)
            {
                return "不限制";
            }
            if (type == EUserPasswordRestriction.LetterAndDigit)
            {
                return "字母和数字组合";
            }
            if (type == EUserPasswordRestriction.LetterAndDigitAndSymbol)
            {
                return "字母、数字以及符号组合";
            }
            throw new Exception();
        }

        public static EUserPasswordRestriction GetEnumType(string typeStr)
        {
            var retval = EUserPasswordRestriction.None;

            if (Equals(EUserPasswordRestriction.None, typeStr))
            {
                retval = EUserPasswordRestriction.None;
            }
            else if (Equals(EUserPasswordRestriction.LetterAndDigit, typeStr))
            {
                retval = EUserPasswordRestriction.LetterAndDigit;
            }
            else if (Equals(EUserPasswordRestriction.LetterAndDigitAndSymbol, typeStr))
            {
                retval = EUserPasswordRestriction.LetterAndDigitAndSymbol;
            }

            return retval;
        }

        public static bool Equals(EUserPasswordRestriction type, string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return false;
            if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool Equals(string typeStr, EUserPasswordRestriction type)
        {
            return Equals(type, typeStr);
        }

        public static ListItem GetListItem(EUserPasswordRestriction type, bool selected)
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
                listControl.Items.Add(GetListItem(EUserPasswordRestriction.None, false));
                listControl.Items.Add(GetListItem(EUserPasswordRestriction.LetterAndDigit, false));
                listControl.Items.Add(GetListItem(EUserPasswordRestriction.LetterAndDigitAndSymbol, false));
            }
        }

        public static bool IsValid(string password, string restrictionStr)
        {
            var restriction = GetEnumType(restrictionStr);
            var isValid = false;
            if (!string.IsNullOrEmpty(password))
            {
                if (restriction == EUserPasswordRestriction.None)
                {
                    isValid = true;
                }
                else if (restriction == EUserPasswordRestriction.LetterAndDigit)
                {
                    var isLetter = false;
                    var isDigit = false;
                    foreach (var c in password)
                    {
                        if (char.IsLetter(c))
                        {
                            isLetter = true;
                        }
                        else if (char.IsDigit(c))
                        {
                            isDigit = true;
                        }
                    }
                    if (isLetter && isDigit)
                    {
                        isValid = true;
                    }
                }
                else if (restriction == EUserPasswordRestriction.LetterAndDigitAndSymbol)
                {
                    var isLetter = false;
                    var isDigit = false;
                    var isSymbol = false;
                    foreach (var c in password)
                    {
                        if (char.IsLetter(c))
                        {
                            isLetter = true;
                        }
                        else if (char.IsDigit(c))
                        {
                            isDigit = true;
                        }
                        else if (char.IsSymbol(c))
                        {
                            isSymbol = true;
                        }
                    }
                    if (isLetter && isDigit && isSymbol)
                    {
                        isValid = true;
                    }
                }
            }
            return isValid;
        }

    }
}
