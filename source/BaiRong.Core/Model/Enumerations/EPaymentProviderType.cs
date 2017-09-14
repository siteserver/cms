using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace BaiRong.Core.Model.Enumerations
{
    public enum EPaymentProviderType
    {
        Pingxx,
        None
    }

    public class EPaymentProviderTypeUtils
    {
        public static string GetValue(EPaymentProviderType type)
        {
            if (type == EPaymentProviderType.Pingxx)
            {
                return "Pingxx";
            }
            return "None";
        }

        public static string GetText(EPaymentProviderType type)
        {
            if (type == EPaymentProviderType.Pingxx)
            {
                return "Ping++";
            }
            return "无";
        }

        public static string GetUrl(EPaymentProviderType type)
        {
            if (type == EPaymentProviderType.Pingxx)
            {
                return "http://www.pingxx.com/";
            }
            return string.Empty;
        }

        public static EPaymentProviderType GetEnumType(string typeStr)
        {
            var retval = EPaymentProviderType.None;
            if (Equals(typeStr, EPaymentProviderType.Pingxx))
            {
                retval = EPaymentProviderType.Pingxx;
            }
            return retval;
        }

        public static bool Equals(EPaymentProviderType type, string typeStr)
        {
            return !string.IsNullOrEmpty(typeStr) && string.Equals(GetValue(type).ToLower(), typeStr.ToLower());
        }

        public static bool Equals(string typeStr, EPaymentProviderType type)
        {
            return Equals(type, typeStr);
        }

        public static List<EPaymentProviderType> GetList()
        {
            return new List<EPaymentProviderType>
            {
                EPaymentProviderType.Pingxx
            };
        }

        public static ListItem GetListItem(EPaymentProviderType type, bool selected)
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
                listControl.Items.Add(GetListItem(EPaymentProviderType.None, false));
                listControl.Items.Add(GetListItem(EPaymentProviderType.Pingxx, false));
            }
        }
    }
}
