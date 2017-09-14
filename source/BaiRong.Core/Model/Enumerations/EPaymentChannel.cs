using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace BaiRong.Core.Model.Enumerations
{
    public enum EPaymentChannel
    {
        AlipayWap,
        AlipayPcDirect,
        WxPub,
        UpacpWap,
        UpacpPc
    }

    public class EPaymentChannelUtils
    {
        public static string GetValue(EPaymentChannel type)
        {
            if (type == EPaymentChannel.AlipayWap)
            {
                return "alipay_wap";
            }
            if (type == EPaymentChannel.AlipayPcDirect)
            {
                return "alipay_pc_direct";
            }
            if (type == EPaymentChannel.WxPub)
            {
                return "wx_pub";
            }
            if (type == EPaymentChannel.UpacpWap)
            {
                return "upacp_wap";
            }
            if (type == EPaymentChannel.UpacpPc)
            {
                return "upacp_pc";
            }
            return string.Empty;
        }

        public static string GetText(EPaymentChannel type)
        {
            if (type == EPaymentChannel.AlipayWap)
            {
                return "支付宝手机网页支付";
            }
            if (type == EPaymentChannel.AlipayPcDirect)
            {
                return "支付宝电脑网站支付";
            }
            if (type == EPaymentChannel.WxPub)
            {
                return "微信公众号支付（电脑/手机）";
            }
            if (type == EPaymentChannel.UpacpWap)
            {
                return "银联手机网页支付";
            }
            if (type == EPaymentChannel.UpacpPc)
            {
                return "银联电脑网页支付";
            }
            return string.Empty;
        }

        public static EPaymentChannel GetEnumType(string typeStr)
        {
            var retval = EPaymentChannel.AlipayPcDirect;
            if (Equals(typeStr, EPaymentChannel.AlipayWap))
            {
                retval = EPaymentChannel.AlipayWap;
            }
            else if (Equals(typeStr, EPaymentChannel.AlipayPcDirect))
            {
                retval = EPaymentChannel.AlipayPcDirect;
            }
            else if (Equals(typeStr, EPaymentChannel.WxPub))
            {
                retval = EPaymentChannel.WxPub;
            }
            else if (Equals(typeStr, EPaymentChannel.UpacpWap))
            {
                retval = EPaymentChannel.UpacpWap;
            }
            else if (Equals(typeStr, EPaymentChannel.UpacpPc))
            {
                retval = EPaymentChannel.UpacpPc;
            }
            return retval;
        }

        public static bool Equals(EPaymentChannel type, string typeStr)
        {
            return !string.IsNullOrEmpty(typeStr) && string.Equals(GetValue(type).ToLower(), typeStr.ToLower());
        }

        public static bool Equals(string typeStr, EPaymentChannel type)
        {
            return Equals(type, typeStr);
        }

        public static ListItem GetListItem(EPaymentChannel type, bool selected)
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
                listControl.Items.Add(GetListItem(EPaymentChannel.AlipayPcDirect, false));
                listControl.Items.Add(GetListItem(EPaymentChannel.AlipayWap, false));
                listControl.Items.Add(GetListItem(EPaymentChannel.WxPub, false));
                listControl.Items.Add(GetListItem(EPaymentChannel.UpacpPc, false));
                listControl.Items.Add(GetListItem(EPaymentChannel.UpacpWap, false));
            }
        }
    }
}
