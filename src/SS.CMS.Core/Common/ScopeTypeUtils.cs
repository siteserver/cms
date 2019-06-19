using System;
using System.Collections.Generic;
using System.Text;
using SS.CMS.Enums;

namespace SS.CMS.Core.Common
{
    public class ScopeTypeUtils
    {
        public static string GetValue(ScopeType type)
        {
            if (type == ScopeType.Self)
            {
                return "Self";
            }
            if (type == ScopeType.Children)
            {
                return "Children";
            }
            if (type == ScopeType.SelfAndChildren)
            {
                return "SelfAndChildren";
            }
            if (type == ScopeType.Descendant)
            {
                return "Descendant";
            }
            if (type == ScopeType.All)
            {
                return "All";
            }
            throw new Exception();
        }

        public static string GetText(ScopeType type)
        {
            if (type == ScopeType.Children)
            {
                return "子栏目";
            }
            if (type == ScopeType.Descendant)
            {
                return "所有子栏目";
            }
            if (type == ScopeType.Self)
            {
                return "本级栏目";
            }
            if (type == ScopeType.SelfAndChildren)
            {
                return "本级栏目及子栏目";
            }
            if (type == ScopeType.All)
            {
                return "全部";
            }
            throw new Exception();
        }

        public static ScopeType GetEnumType(string typeStr)
        {
            var retval = ScopeType.Self;

            if (Equals(ScopeType.Children, typeStr))
            {
                retval = ScopeType.Children;
            }
            else if (Equals(ScopeType.Descendant, typeStr))
            {
                retval = ScopeType.Descendant;
            }
            else if (Equals(ScopeType.Self, typeStr))
            {
                retval = ScopeType.Self;
            }
            else if (Equals(ScopeType.SelfAndChildren, typeStr))
            {
                retval = ScopeType.SelfAndChildren;
            }
            else if (Equals(ScopeType.All, typeStr))
            {
                retval = ScopeType.All;
            }

            return retval;
        }

        public static bool Equals(ScopeType type, string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return false;
            if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool Equals(string typeStr, ScopeType type)
        {
            return Equals(type, typeStr);
        }

    }
}
