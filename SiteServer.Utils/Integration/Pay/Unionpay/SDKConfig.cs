using System.Web.Configuration;
using System.Configuration;
using BaiRong.Core;
using SiteServer.B2C.Model;
using SiteServer.B2C.Core;
using SiteServer.B2C.Core.Union;

namespace SiteServer.B2C.Core.Union
{

    public class SDKConfig
    {
        private static string cardRequestUrl = "https://gateway.95516.com/gateway/api/cardTransReq.do";  //有卡交易地址
        private static string appRequestUrl = "https://gateway.95516.com/gateway/api/appTransReq.do";  //app交易地址 手机控件支付使用该地址
        private static string singleQueryUrl = "https://gateway.95516.com/gateway/api/queryTrans.do"; //交易状态查询地址
        private static string fileTransUrl = "https://filedownload.95516.com/";  //文件传输类交易地址
        private static string frontTransUrl = "https://gateway.95516.com/gateway/api/frontTransReq.do"; //前台交易地址
        private static string backTransUrl = "https://gateway.95516.com/gateway/api/backTransReq.do";//后台交易地址
        private static string batTransUrl = "https://gateway.95516.com/gateway/api/batchTrans.do";//功能：读取配批量交易地址

        public static string CardRequestUrl
        {
            get { return SDKConfig.cardRequestUrl; }
            set { SDKConfig.cardRequestUrl = value; }
        }
        public static string AppRequestUrl
        {
            get { return SDKConfig.appRequestUrl; }
            set { SDKConfig.appRequestUrl = value; }
        }

        public static string FrontTransUrl
        {
            get { return SDKConfig.frontTransUrl; }
            set { SDKConfig.frontTransUrl = value; }
        }


        public static string BackTransUrl
        {
            get { return SDKConfig.backTransUrl; }
            set { SDKConfig.backTransUrl = value; }
        }

        public static string SingleQueryUrl
        {
            get { return SDKConfig.singleQueryUrl; }
            set { SDKConfig.singleQueryUrl = value; }
        }

        public static string FileTransUrl
        {
            get { return SDKConfig.fileTransUrl; }
            set { SDKConfig.fileTransUrl = value; }
        }

        public static string BatTransUrl
        {
            get { return SDKConfig.batTransUrl; }
            set { SDKConfig.batTransUrl = value; }
        }

        public static string GetSDKRootVirvualPath(string relatedPath)
        {
            return PathUtils.GetAppPath(AppManager.B2C.AppID, "certs", relatedPath);
        }



        #region 参数
        /// <summary>
        /// 订单号
        /// </summary>
        public int OrderID { get; set; }

        /// <summary>
        /// 接口版本：5.0.0
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// 编码格式
        /// </summary>
        public string Input_Encoding { get; set; }
        /// <summary>
        /// 证书ID
        /// </summary>
        public string Txn_Type { get; set; }
        /// <summary>
        /// 交易子类型
        /// </summary>
        public string Txn_SubType { get; set; }
        /// <summary>
        /// 业务类型
        /// </summary>
        public string Biz_Type { get; set; }
        /// <summary>
        /// 签名方法
        /// </summary>
        public string Sign_Method { get; set; }
        /// <summary>
        /// 接入类型
        /// </summary>
        public string Access_Type { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string Mer_ID { get; set; }
        /// <summary>
        /// 交易币种
        /// </summary>
        public string Currency_Code { get; set; }

        /// <summary>
        /// 签名证书路径
        /// </summary>
        public string SignCertPath { get; set; }
        /// <summary>
        /// 签名证书密码
        /// </summary>
        public string SignCertPwd { get; set; }
        /// <summary>
        /// 验证证书目录
        /// </summary>
        public string ValidateCertDir { get; set; }
        /// <summary>
        /// 加密证书路径
        /// </summary>
        public string EncryptCert { get; set; }

        public SDKConfig(int publishmentSystemID, int orderID)
        {
            this.OrderID = orderID;
            PaymentInfo paymentInfo = PaymentManager.GetPaymentInfo(publishmentSystemID, EPaymentType.Unionpay);
            if (paymentInfo != null)
            {
                PaymentUnionInfo unionInfo = new PaymentUnionInfo(paymentInfo.SettingsXML);

                if (unionInfo.IsTest)
                {
                    CardRequestUrl = "https://101.231.204.80:5000/gateway/api/cardTransReq.do";  //有卡交易地址
                    AppRequestUrl = "https://101.231.204.80:5000/gateway/api/appTransReq.do";  //app交易地址 手机控件支付使用该地址
                    SingleQueryUrl = "https://101.231.204.80:5000/gateway/api/queryTrans.do"; //交易状态查询地址
                    FileTransUrl = "https://101.231.204.80:9080/";  //文件传输类交易地址
                    FrontTransUrl = "https://101.231.204.80:5000/gateway/api/frontTransReq.do"; //前台交易地址
                    BackTransUrl = "https://101.231.204.80:5000/gateway/api/backTransReq.do";//后台交易地址
                    BatTransUrl = "https://101.231.204.80:5000/gateway/api/batchTrans.do";//功能：读取配批量交易地址
                }

                //表单参数
                this.Version = "5.0.0";
                this.Input_Encoding = "UTF-8";
                this.Txn_Type = "01";//消费
                this.Txn_SubType = "01";//消费
                this.Biz_Type = "000201";//B2C 网关支付 
                this.Sign_Method = "01";//签名类型,RAS
                this.Access_Type = "0";//商户直接接入
                this.Mer_ID = unionInfo.MerID;
                this.Currency_Code = "156";

                //后台参数
                this.SignCertPath = APIPageUtils.ParseUrl(GetSDKRootVirvualPath(unionInfo.SignCertPath));
                this.SignCertPwd = unionInfo.SignCertPwd;
                this.ValidateCertDir = APIPageUtils.ParseUrl(GetSDKRootVirvualPath(string.Empty));
                this.EncryptCert = APIPageUtils.ParseUrl(GetSDKRootVirvualPath(unionInfo.EncryptCert));
            }

            CertUtil.sdkConfig = SDKUtil.sdkConfig = SecurityUtil.sdkConfig = this;
        }
        #endregion

    }
}