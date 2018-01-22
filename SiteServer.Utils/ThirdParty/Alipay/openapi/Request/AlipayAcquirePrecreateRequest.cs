using System;
using System.Collections.Generic;
using Aop.Api.Response;

namespace Aop.Api.Request
{
    /// <summary>
    /// AOP API: alipay.acquire.precreate
    /// </summary>
    public class AlipayAcquirePrecreateRequest : IAopRequest<AlipayAcquirePrecreateResponse>
    {
        /// <summary>
        /// 对一笔交易的具体描述信息。如果是多种商品，请将商品描述字符串累加传给body
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// 描述多渠道收单的渠道明细信息，json格式
        /// </summary>
        public string ChannelParameters { get; set; }

        /// <summary>
        /// 订单金额币种。目前只支持传入156（人民币）。  如果为空，则默认设置为156
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 公用业务扩展信息。用于商户的特定业务信息的传递，只有商户与支付宝约定了传递此参数且约定了参数含义，此参数才有效。  比如可传递二维码支付场景下的门店ID等信息，以json格式传输。
        /// </summary>
        public string ExtendParams { get; set; }

        /// <summary>
        /// 描述商品明细信息，json格式。
        /// </summary>
        public string GoodsDetail { get; set; }

        /// <summary>
        /// 订单支付超时时间。设置未付款交易的超时时间，一旦超时，该笔交易就会自动被关闭。  取值范围：1m～15d。  m-分钟，h-小时，d-天，1c-当天（无论交易何时创建，都在0点关闭）。  该参数数值不接受小数点，如1.5h，可转换为90m。  该功能需要联系支付宝配置关闭时间。
        /// </summary>
        public string ItBPay { get; set; }

        /// <summary>
        /// 操作员的类型：  0：支付宝操作员  1：商户的操作员  如果传入其它值或者为空，则默认设置为1
        /// </summary>
        public string OperatorCode { get; set; }

        /// <summary>
        /// 卖家的操作员ID
        /// </summary>
        public string OperatorId { get; set; }

        /// <summary>
        /// 支付宝合作商户网站唯一订单号
        /// </summary>
        public string OutTradeNo { get; set; }

        /// <summary>
        /// 订单中商品的单价。  如果请求时传入本参数，则必须满足total_fee=price×quantity的条件
        /// </summary>
        public string Price { get; set; }

        /// <summary>
        /// 订单中商品的数量。  如果请求时传入本参数，则必须满足total_fee=price×quantity的条件
        /// </summary>
        public string Quantity { get; set; }

        /// <summary>
        /// 分账信息。  描述分账明细信息，json格式
        /// </summary>
        public string RoyaltyParameters { get; set; }

        /// <summary>
        /// 分账类型。卖家的分账类型，目前只支持传入ROYALTY（普通分账类型）
        /// </summary>
        public string RoyaltyType { get; set; }

        /// <summary>
        /// 卖家支付宝账号，可以为email或者手机号。如果seller_id不为空，则以seller_id的值作为卖家账号，忽略本参数
        /// </summary>
        public string SellerEmail { get; set; }

        /// <summary>
        /// 卖家支付宝账号对应的支付宝唯一用户号，以2088开头的纯16位数字。如果和seller_email同时为空，则本参数默认填充partner的值
        /// </summary>
        public string SellerId { get; set; }

        /// <summary>
        /// 收银台页面上，商品展示的超链接
        /// </summary>
        public string ShowUrl { get; set; }

        /// <summary>
        /// 商品购买
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 订单金额。该笔订单的资金总额，取值范围[0.01,100000000]，精确到小数点后2位。
        /// </summary>
        public string TotalFee { get; set; }

        #region IAopRequest Members
		private bool  needEncrypt=false;
        private string apiVersion = "1.0";
		private string terminalType;
		private string terminalInfo;
        private string prodCode;
		private string notifyUrl;
        private string returnUrl;
		private AopObject bizModel;

		public void SetNeedEncrypt(bool needEncrypt){
             this.needEncrypt=needEncrypt;
        }

        public bool GetNeedEncrypt(){

            return this.needEncrypt;
        }

		public void SetNotifyUrl(string notifyUrl){
            this.notifyUrl = notifyUrl;
        }

        public string GetNotifyUrl(){
            return this.notifyUrl;
        }

        public void SetReturnUrl(string returnUrl){
            this.returnUrl = returnUrl;
        }

        public string GetReturnUrl(){
            return this.returnUrl;
        }

        public void SetTerminalType(String terminalType){
			this.terminalType=terminalType;
		}

    	public string GetTerminalType(){
    		return this.terminalType;
    	}

    	public void SetTerminalInfo(String terminalInfo){
    		this.terminalInfo=terminalInfo;
    	}

    	public string GetTerminalInfo(){
    		return this.terminalInfo;
    	}

        public void SetProdCode(String prodCode){
            this.prodCode=prodCode;
        }

        public string GetProdCode(){
            return this.prodCode;
        }

        public string GetApiName()
        {
            return "alipay.acquire.precreate";
        }

        public void SetApiVersion(string apiVersion){
            this.apiVersion=apiVersion;
        }

        public string GetApiVersion(){
            return this.apiVersion;
        }

        public IDictionary<string, string> GetParameters()
        {
            AopDictionary parameters = new AopDictionary();
            parameters.Add("body", this.Body);
            parameters.Add("channel_parameters", this.ChannelParameters);
            parameters.Add("currency", this.Currency);
            parameters.Add("extend_params", this.ExtendParams);
            parameters.Add("goods_detail", this.GoodsDetail);
            parameters.Add("it_b_pay", this.ItBPay);
            parameters.Add("operator_code", this.OperatorCode);
            parameters.Add("operator_id", this.OperatorId);
            parameters.Add("out_trade_no", this.OutTradeNo);
            parameters.Add("price", this.Price);
            parameters.Add("quantity", this.Quantity);
            parameters.Add("royalty_parameters", this.RoyaltyParameters);
            parameters.Add("royalty_type", this.RoyaltyType);
            parameters.Add("seller_email", this.SellerEmail);
            parameters.Add("seller_id", this.SellerId);
            parameters.Add("show_url", this.ShowUrl);
            parameters.Add("subject", this.Subject);
            parameters.Add("total_fee", this.TotalFee);
            return parameters;
        }

		public AopObject GetBizModel()
        {
            return this.bizModel;
        }

        public void SetBizModel(AopObject bizModel)
        {
            this.bizModel = bizModel;
        }

        #endregion
    }
}
