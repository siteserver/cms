using System;
using System.Collections.Generic;
using Aop.Api.Response;

namespace Aop.Api.Request
{
    /// <summary>
    /// AOP API: alipay.micropay.order.direct.pay
    /// </summary>
    public class AlipayMicropayOrderDirectPayRequest : IAopRequest<AlipayMicropayOrderDirectPayResponse>
    {
        /// <summary>
        /// 支付宝订单号，冻结流水号.这个是创建冻结订单支付宝返回的
        /// </summary>
        public string AlipayOrderNo { get; set; }

        /// <summary>
        /// 支付金额,区间必须在[0.01,30]，只能保留小数点后两位
        /// </summary>
        public string Amount { get; set; }

        /// <summary>
        /// 支付备注
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 收款方的支付宝ID
        /// </summary>
        public string ReceiveUserId { get; set; }

        /// <summary>
        /// 本次转账的外部单据号（只能由字母和数字组成,maxlength=32
        /// </summary>
        public string TransferOutOrderNo { get; set; }

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
            return "alipay.micropay.order.direct.pay";
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
            parameters.Add("alipay_order_no", this.AlipayOrderNo);
            parameters.Add("amount", this.Amount);
            parameters.Add("memo", this.Memo);
            parameters.Add("receive_user_id", this.ReceiveUserId);
            parameters.Add("transfer_out_order_no", this.TransferOutOrderNo);
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
