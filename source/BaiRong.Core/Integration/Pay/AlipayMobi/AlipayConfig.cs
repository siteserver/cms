namespace BaiRong.Core.Integration.Pay.AlipayMobi
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
        #region 字段
        public int orderID = 0;
        public string partner = "";
        public string key = "";
        public string private_key = "";
        public string public_key = "";
        public string input_charset = "";
        public string sign_type = "";
        public string seller_email = "";
        public string notify_url = "";
        public string return_url = "";
        #endregion

        public Config(int publishmentSystemID, int OrderID)
        {
            this.orderID = OrderID;
            PaymentInfo paymentInfo = PaymentManager.GetPaymentInfo(publishmentSystemID, EPaymentType.Alipay);
            if (paymentInfo != null)
            {
                PaymentAlipayInfo alipayInfo = new PaymentAlipayInfo(paymentInfo.SettingsXML);
                //↓↓↓↓↓↓↓↓↓↓请在这里配置您的基本信息↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓            

                //合作身份者ID，以2088开头由16位纯数字组成的字符串
                partner = alipayInfo.Partner;

                //交易安全检验码，由数字和字母组成的32位字符串
                key = alipayInfo.Key;

                //卖家支付宝帐户
                seller_email = alipayInfo.SellerEmail;

                //服务器异步通知页面路径
                notify_url = "";

                //页面跳转同步通知页面路径
                return_url = "";

                //商户的私钥
                //如果签名方式设置为“0001”时，请设置该参数
                private_key = @"";

                //支付宝的公钥
                //如果签名方式设置为“0001”时，请设置该参数
                public_key = @"";

                //↑↑↑↑↑↑↑↑↑↑请在这里配置您的基本信息↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑

                //字符编码格式 目前支持 utf-8
                input_charset = "utf-8";

                //签名方式，选择项：0001(RSA)、MD5
                sign_type = "MD5";
                //无线的产品中，签名方式为rsa时，sign_type需赋值为0001而不是RSA
            }
        }

        #region 属性
 
         public int OrderID
         {
             get { return orderID; }
             set { orderID = value; }
         }

        /// <summary>
        /// 获取或设置合作者身份ID
        /// </summary>
        public string Partner
        {
            get { return partner; }
            set { partner = value; }
        }

        /// <summary>
        /// 获取或设交易安全校验码
        /// </summary>
        public string Key
        {
            get { return key; }
            set { key = value; }
        }

        /// <summary>
        /// 获取或设置商户的私钥
        /// </summary>
        public string Private_key
        {
            get { return private_key; }
            set { private_key = value; }
        }

        /// <summary>
        /// 获取或设置支付宝的公钥
        /// </summary>
        public string Public_key
        {
            get { return public_key; }
            set { public_key = value; }
        }

        /// <summary>
        /// 获取字符编码格式
        /// </summary>
        public string Input_charset
        {
            get { return input_charset; }
        }

        /// <summary>
        /// 获取签名方式
        /// </summary>
        public string Sign_type
        {
            get { return sign_type; }
        }

        public string Seller_email
        {
            get { return seller_email; }
        }
        public string Notify_url
        {
            get { return notify_url; }
        }
        public string Return_url
        {
            get { return return_url; }
        }
        #endregion
    }
}