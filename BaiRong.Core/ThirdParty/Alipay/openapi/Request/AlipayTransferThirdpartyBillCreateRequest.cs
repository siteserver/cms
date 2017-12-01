using System;
using System.Collections.Generic;
using Aop.Api.Response;

namespace Aop.Api.Request
{
    /// <summary>
    /// AOP API: alipay.transfer.thirdparty.bill.create
    /// </summary>
    public class AlipayTransferThirdpartyBillCreateRequest : IAopRequest<AlipayTransferThirdpartyBillCreateResponse>
    {
        /// <summary>
        /// 收款金额，单位：分
        /// </summary>
        public string Amount { get; set; }

        /// <summary>
        /// 收款币种，默认为156（人民币）目前只允许转账人民币
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 扩展参数
        /// </summary>
        public string ExtParam { get; set; }

        /// <summary>
        /// 转账备注
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 合作方的支付宝帐号UID
        /// </summary>
        public string PartnerId { get; set; }

        /// <summary>
        /// 外部系统收款方UID，付款人和收款人不能是同一个帐户
        /// </summary>
        public string PayeeAccount { get; set; }

        /// <summary>
        /// （同payer_type所列举的）  目前限制payer_type和payee_type必须一致
        /// </summary>
        public string PayeeType { get; set; }

        /// <summary>
        /// 外部系统付款方的UID
        /// </summary>
        public string PayerAccount { get; set; }

        /// <summary>
        /// 1-支付宝帐户  2-淘宝帐户  10001-新浪微博帐户  10002-阿里云帐户  （1、2目前对外不可见、不可用）
        /// </summary>
        public string PayerType { get; set; }

        /// <summary>
        /// 发起支付交易来源方定义的交易ID，用于将支付回执通知给来源方。不同来源方给出的ID可以重复，同一个来源方给出的ID唯一性由来源方保证。
        /// </summary>
        public string PaymentId { get; set; }

        /// <summary>
        /// 支付来源  10001-新浪微博  10002-阿里云
        /// </summary>
        public string PaymentSource { get; set; }

        /// <summary>
        /// 支付款项的标题
        /// </summary>
        public string Title { get; set; }

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
            return "alipay.transfer.thirdparty.bill.create";
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
            parameters.Add("amount", this.Amount);
            parameters.Add("currency", this.Currency);
            parameters.Add("ext_param", this.ExtParam);
            parameters.Add("memo", this.Memo);
            parameters.Add("partner_id", this.PartnerId);
            parameters.Add("payee_account", this.PayeeAccount);
            parameters.Add("payee_type", this.PayeeType);
            parameters.Add("payer_account", this.PayerAccount);
            parameters.Add("payer_type", this.PayerType);
            parameters.Add("payment_id", this.PaymentId);
            parameters.Add("payment_source", this.PaymentSource);
            parameters.Add("title", this.Title);
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
