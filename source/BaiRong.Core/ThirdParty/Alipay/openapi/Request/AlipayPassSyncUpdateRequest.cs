using System;
using System.Collections.Generic;
using Aop.Api.Response;

namespace Aop.Api.Request
{
    /// <summary>
    /// AOP API: alipay.pass.sync.update
    /// </summary>
    public class AlipayPassSyncUpdateRequest : IAopRequest<AlipayPassSyncUpdateResponse>
    {
        /// <summary>
        /// 代理商代替商户发放卡券后，再代替商户更新卡券时，此值为商户的pid/appid；商户自己发券时，此值为空或者商户appId
        /// </summary>
        public string ChannelId { get; set; }

        /// <summary>
        /// 用来传递外部交易号等扩展参数信息，格式为json
        /// </summary>
        public string ExtInfo { get; set; }

        /// <summary>
        /// 需要修改的pass信息，可以更新全部pass信息，也可以斤更新某一节点。pass信息中的pass.json中的数据格式，如果不需要更新该属性值，设置为null即可。
        /// </summary>
        public string Pass { get; set; }

        /// <summary>
        /// Alipass唯一标识
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// Alipass状态，目前仅支持CLOSED及USED两种数据。status为USED时，verify_type即为核销时的核销方式。
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 核销码串值【当状态变更为USED时，建议传入】
        /// </summary>
        public string VerifyCode { get; set; }

        /// <summary>
        /// 核销方式，目前支持：wave（声波方式）、qrcode（二维码方式）、barcode（条码方式）、input（文本方式，即手工输入方式）。pass和verify_type不能同时为空
        /// </summary>
        public string VerifyType { get; set; }

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
            return "alipay.pass.sync.update";
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
            parameters.Add("channel_id", this.ChannelId);
            parameters.Add("ext_info", this.ExtInfo);
            parameters.Add("pass", this.Pass);
            parameters.Add("serial_number", this.SerialNumber);
            parameters.Add("status", this.Status);
            parameters.Add("verify_code", this.VerifyCode);
            parameters.Add("verify_type", this.VerifyType);
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
