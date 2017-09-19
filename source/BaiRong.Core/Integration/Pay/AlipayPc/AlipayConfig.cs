using BaiRong.Core.Model;

namespace BaiRong.Core.Integration.Pay.AlipayPc
{
    /// <summary>
    /// 类名：Config
    /// 功能：基础配置类
    /// 详细：设置帐户有关信息及返回路径
    /// 版本：3.3
    /// 日期：2012-07-05
    /// 说明：
    /// 以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
    /// 该代码仅供学习和研究支付宝接口使用，只是提供一个参考。
    /// 
    /// 如何获取安全校验码和合作身份者ID
    /// 1.用您的签约支付宝账号登录支付宝网站(www.alipay.com)
    /// 2.点击“商家服务”(https://b.alipay.com/order/myOrder.htm)
    /// 3.点击“查询合作者身份(PID)”、“查询安全校验码(Key)”
    /// </summary>
    public class Config
    {
        public int OrderID { get; set; }
        public string Partner { get; set; }
        public string Key { get; set; }
        public string Input_charset { get; set; }
        public string Sign_type { get; set; }
        public string Service { get; set; }
        public string Payment_type { get; set; }
        public string Seller_email { get; set; }

        #region DualFun
        public string Quantity { get; set; }
        public string Logistics_fee { get; set; }
        public string Logistics_type { get; set; }
        public string Logistics_payment { get; set; }
        #endregion

        public Config(IntegrationPayConfig config)
        {
            this.OrderID = orderID;
            //合作身份者ID，以2088开头由16位纯数字组成的字符串
            Partner = config.AlipayPcPid;
            //交易安全检验码，由数字和字母组成的32位字符串
            Key = config.AlipayPcMd5;
            //字符编码格式 目前支持 gbk 或 utf-8
            Input_charset = "utf-8";
            //签名方式，选择项：RSA、DSA、MD5
            Sign_type = "MD5";

            //支付类型
            Payment_type = "1";
            //卖家支付宝帐户
            Seller_email = config.AlipayPcAccount;

            Service = "create_direct_pay_by_user";
        }
    }
}