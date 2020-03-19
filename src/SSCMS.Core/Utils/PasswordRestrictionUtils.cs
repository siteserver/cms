using SSCMS;
using SSCMS.Utils;

namespace SSCMS.Core.Utils
{
    public static class PasswordRestrictionUtils
    {
        public static bool IsValid(string password, string restrictionStr)
        {
            var restriction = TranslateUtils.ToEnum(restrictionStr, PasswordRestriction.None);
            return IsValid(password, restriction);
        }

        public static bool IsValid(string password, PasswordRestriction restriction)
        {
            var isValid = false;
            if (!string.IsNullOrEmpty(password))
            {
                if (restriction == PasswordRestriction.None)
                {
                    isValid = true;
                }
                else if (restriction == PasswordRestriction.LetterAndDigit)
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
                else if (restriction == PasswordRestriction.LetterAndDigitAndSymbol)
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
                        else
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
