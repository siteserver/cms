using System.Web;

namespace SiteServer.Plugin.Apis
{
    public interface IPaymentApi
    {
        bool IsAlipayPc { get; }

        string ChargeByAlipayPc(string productName, decimal amount, string orderNo, string returnUrl, string notifyUrl);

        bool IsAlipayMobi { get; }

        string ChargeByAlipayMobi(string productName, decimal amount, string orderNo, string returnUrl, string notifyUrl);

        bool IsWeixin { get; }

        string ChargeByWeixin(string productName, decimal amount, string orderNo, string notifyUrl);

        void NotifyByWeixin(HttpRequest request, out bool isPaied, out string responseXml);

        bool IsJdpay { get; }

        string ChargeByJdpay(string productName, decimal amount, string orderNo, string returnUrl, string notifyUrl);
    }
}
