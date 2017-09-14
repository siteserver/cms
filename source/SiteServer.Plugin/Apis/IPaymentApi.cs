namespace SiteServer.Plugin.Apis
{
    public interface IPaymentApi
    {
        /// <summary>
        /// 是否集成支付
        /// </summary>
        /// <returns></returns>
        bool IsReady();

        /// <summary>
        /// 支付宝PC网页支付是否可用
        /// </summary>
        /// <returns></returns>
        bool IsPcAlipay();

        /// <summary>
        /// 发起支付宝PC网页支付请求
        /// </summary>
        /// <param name="amount">订单总金额, 人民币单位：分（如订单总金额为 1 元，此处请填 100）</param>
        /// <param name="orderNo">商户订单号，适配每个渠道对此参数的要求，必须在商户系统内唯一</param>
        /// <param name="successUrl">支付成功后跳转Url</param>
        /// <returns>Charge 对象</returns>
        object ChargeByPcAlipay(int amount, string orderNo, string successUrl);

        /// <summary>
        /// 支付宝手机网页支付是否可用
        /// </summary>
        /// <returns></returns>
        bool IsWapAlipay();

        /// <summary>
        /// 发起支付宝手机网页支付请求
        /// </summary>
        /// <param name="amount">订单总金额, 人民币单位：分（如订单总金额为 1 元，此处请填 100）</param>
        /// <param name="orderNo">商户订单号，适配每个渠道对此参数的要求，必须在商户系统内唯一</param>
        /// <param name="successUrl">支付成功后跳转Url</param>
        /// <param name="cancelUrl">支付取消后跳转Url</param>
        /// <returns>Charge 对象</returns>
        object ChargeByWapAlipay(int amount, string orderNo, string successUrl, string cancelUrl);

        bool IsPcWeixin();

        object ChargeByPcWeixin(int amount, string orderNo, string openId);

        bool IsWapWeiXin();

        object ChargeByWapWeiXin(int amount, string orderNo, string openId);

        /// <summary>
        /// 是否支持银联支付
        /// </summary>
        /// <returns></returns>
        bool IsPcUnionPay();

        object ChargeByPcUnionPay(int amount, string orderNo, string successUrl);

        /// <summary>
        /// 是否支持银联支付
        /// </summary>
        /// <returns></returns>
        bool IsWapUnionPay();

        object ChargeByWapUnionPay(int amount, string orderNo, string successUrl);
    }
}
