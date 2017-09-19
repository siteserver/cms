namespace SiteServer.Plugin.Apis
{
    public interface IPaymentApi
    {
        /// <summary>
        /// 支付宝PC网页支付是否可用
        /// </summary>
        /// <returns></returns>
        bool IsAlipayPc();

        /// <summary>
        /// 发起支付宝PC网页支付请求
        /// </summary>
        /// <param name="amount">订单总金额, 人民币单位：元</param>
        /// <param name="orderNo">商户订单号，适配每个渠道对此参数的要求，必须在商户系统内唯一</param>
        /// <param name="successUrl">支付成功后跳转Url</param>
        /// <returns>Charge 对象</returns>
        object ChargeByAlipayPc(decimal amount, string orderNo, string successUrl);
    }
}
