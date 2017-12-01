using System;
using System.Collections.Generic;
using Aop.Api.Response;

namespace Aop.Api.Request
{
    /// <summary>
    /// AOP API: alipay.security.risk.detect
    /// </summary>
    public class AlipaySecurityRiskDetectRequest : IAopRequest<AlipaySecurityRiskDetectResponse>
    {
        /// <summary>
        /// 买家账户编号
        /// </summary>
        public string BuyerAccountNo { get; set; }

        /// <summary>
        /// 买家绑定银行卡号
        /// </summary>
        public string BuyerBindBankcard { get; set; }

        /// <summary>
        /// 买家绑定银行卡的卡类型
        /// </summary>
        public string BuyerBindBankcardType { get; set; }

        /// <summary>
        /// 买家绑定手机号
        /// </summary>
        public string BuyerBindMobile { get; set; }

        /// <summary>
        /// 买家账户在商家的等级，范围：VIP（高级买家）, NORMAL(普通买家）。为空默认NORMAL
        /// </summary>
        public string BuyerGrade { get; set; }

        /// <summary>
        /// 买家证件号码
        /// </summary>
        public string BuyerIdentityNo { get; set; }

        /// <summary>
        /// 买家证件类型
        /// </summary>
        public string BuyerIdentityType { get; set; }

        /// <summary>
        /// 买家真实姓名
        /// </summary>
        public string BuyerRealName { get; set; }

        /// <summary>
        /// 买家注册时间
        /// </summary>
        public string BuyerRegDate { get; set; }

        /// <summary>
        /// 买家注册时留的Email
        /// </summary>
        public string BuyerRegEmail { get; set; }

        /// <summary>
        /// 买家注册手机号
        /// </summary>
        public string BuyerRegMobile { get; set; }

        /// <summary>
        /// 买家业务处理时使用的银行卡号
        /// </summary>
        public string BuyerSceneBankcard { get; set; }

        /// <summary>
        /// 买家业务处理时使用的银行卡类型
        /// </summary>
        public string BuyerSceneBankcardType { get; set; }

        /// <summary>
        /// 买家业务处理时使用的邮箱
        /// </summary>
        public string BuyerSceneEmail { get; set; }

        /// <summary>
        /// 买家业务处理时使用的手机号
        /// </summary>
        public string BuyerSceneMobile { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 客户端的基带版本
        /// </summary>
        public string EnvClientBaseBand { get; set; }

        /// <summary>
        /// 客户端连接的基站信息,格式为：CELLID^LAC
        /// </summary>
        public string EnvClientBaseStation { get; set; }

        /// <summary>
        /// 客户端的经纬度坐标,格式为：精度^维度
        /// </summary>
        public string EnvClientCoordinates { get; set; }

        /// <summary>
        /// 操作的客户端的imei
        /// </summary>
        public string EnvClientImei { get; set; }

        /// <summary>
        /// 操作的客户端IMSI识别码
        /// </summary>
        public string EnvClientImsi { get; set; }

        /// <summary>
        /// IOS设备的UDID
        /// </summary>
        public string EnvClientIosUdid { get; set; }

        /// <summary>
        /// 操作的客户端ip
        /// </summary>
        public string EnvClientIp { get; set; }

        /// <summary>
        /// 操作的客户端mac
        /// </summary>
        public string EnvClientMac { get; set; }

        /// <summary>
        /// 操作的客户端分辨率，格式为：水平像素^垂直像素；如：800^600
        /// </summary>
        public string EnvClientScreen { get; set; }

        /// <summary>
        /// 客户端设备的统一识别码UUID
        /// </summary>
        public string EnvClientUuid { get; set; }

        /// <summary>
        /// 订单产品数量，购买产品的数量（不可为小数）
        /// </summary>
        public string ItemQuantity { get; set; }

        /// <summary>
        /// 订单产品单价，取值范围为[0.01,100000000.00]，精确到小数点后两位。 curren...
        /// </summary>
        public string ItemUnitPrice { get; set; }

        /// <summary>
        /// JS SDK生成的 tokenID
        /// </summary>
        public string JsTokenId { get; set; }

        /// <summary>
        /// 订单总金额，取值范围为[0.01,100000000.00]，精确到小数点后两位。
        /// </summary>
        public string OrderAmount { get; set; }

        /// <summary>
        /// 订单商品所在类目
        /// </summary>
        public string OrderCategory { get; set; }

        /// <summary>
        /// 订单下单时间
        /// </summary>
        public string OrderCredateTime { get; set; }

        /// <summary>
        /// 订单商品所在城市
        /// </summary>
        public string OrderItemCity { get; set; }

        /// <summary>
        /// 订单产品名称
        /// </summary>
        public string OrderItemName { get; set; }

        /// <summary>
        /// 商户订单唯一标识号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 签约的支付宝账号对应的支付宝唯一用户号
        /// </summary>
        public string PartnerId { get; set; }

        /// <summary>
        /// 订单收货人地址
        /// </summary>
        public string ReceiverAddress { get; set; }

        /// <summary>
        /// 订单收货人地址城市
        /// </summary>
        public string ReceiverCity { get; set; }

        /// <summary>
        /// 订单收货人地址所在区
        /// </summary>
        public string ReceiverDistrict { get; set; }

        /// <summary>
        /// 订单收货人邮箱
        /// </summary>
        public string ReceiverEmail { get; set; }

        /// <summary>
        /// 订单收货人手机
        /// </summary>
        public string ReceiverMobile { get; set; }

        /// <summary>
        /// 订单收货人姓名
        /// </summary>
        public string ReceiverName { get; set; }

        /// <summary>
        /// 订单收货人地址省份
        /// </summary>
        public string ReceiverState { get; set; }

        /// <summary>
        /// 订单收货人地址邮编
        /// </summary>
        public string ReceiverZip { get; set; }

        /// <summary>
        /// 场景编码
        /// </summary>
        public string SceneCode { get; set; }

        /// <summary>
        /// 卖家账户编号
        /// </summary>
        public string SellerAccountNo { get; set; }

        /// <summary>
        /// 卖家绑定银行卡号
        /// </summary>
        public string SellerBindBankcard { get; set; }

        /// <summary>
        /// 卖家绑定的银行卡的卡类型
        /// </summary>
        public string SellerBindBankcardType { get; set; }

        /// <summary>
        /// 卖家绑定手机号
        /// </summary>
        public string SellerBindMobile { get; set; }

        /// <summary>
        /// 卖家证件号码
        /// </summary>
        public string SellerIdentityNo { get; set; }

        /// <summary>
        /// 卖家证件类型
        /// </summary>
        public string SellerIdentityType { get; set; }

        /// <summary>
        /// 卖家真实姓名
        /// </summary>
        public string SellerRealName { get; set; }

        /// <summary>
        /// 卖家注册时间,格式为：yyyy-MM-dd。
        /// </summary>
        public string SellerRegDate { get; set; }

        /// <summary>
        /// 卖家注册Email
        /// </summary>
        public string SellerRegEmail { get; set; }

        /// <summary>
        /// 卖家注册手机号
        /// </summary>
        public string SellerRegMoile { get; set; }

        /// <summary>
        /// 订单物流方式
        /// </summary>
        public string TransportType { get; set; }

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
            return "alipay.security.risk.detect";
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
            parameters.Add("buyer_account_no", this.BuyerAccountNo);
            parameters.Add("buyer_bind_bankcard", this.BuyerBindBankcard);
            parameters.Add("buyer_bind_bankcard_type", this.BuyerBindBankcardType);
            parameters.Add("buyer_bind_mobile", this.BuyerBindMobile);
            parameters.Add("buyer_grade", this.BuyerGrade);
            parameters.Add("buyer_identity_no", this.BuyerIdentityNo);
            parameters.Add("buyer_identity_type", this.BuyerIdentityType);
            parameters.Add("buyer_real_name", this.BuyerRealName);
            parameters.Add("buyer_reg_date", this.BuyerRegDate);
            parameters.Add("buyer_reg_email", this.BuyerRegEmail);
            parameters.Add("buyer_reg_mobile", this.BuyerRegMobile);
            parameters.Add("buyer_scene_bankcard", this.BuyerSceneBankcard);
            parameters.Add("buyer_scene_bankcard_type", this.BuyerSceneBankcardType);
            parameters.Add("buyer_scene_email", this.BuyerSceneEmail);
            parameters.Add("buyer_scene_mobile", this.BuyerSceneMobile);
            parameters.Add("currency", this.Currency);
            parameters.Add("env_client_base_band", this.EnvClientBaseBand);
            parameters.Add("env_client_base_station", this.EnvClientBaseStation);
            parameters.Add("env_client_coordinates", this.EnvClientCoordinates);
            parameters.Add("env_client_imei", this.EnvClientImei);
            parameters.Add("env_client_imsi", this.EnvClientImsi);
            parameters.Add("env_client_ios_udid", this.EnvClientIosUdid);
            parameters.Add("env_client_ip", this.EnvClientIp);
            parameters.Add("env_client_mac", this.EnvClientMac);
            parameters.Add("env_client_screen", this.EnvClientScreen);
            parameters.Add("env_client_uuid", this.EnvClientUuid);
            parameters.Add("item_quantity", this.ItemQuantity);
            parameters.Add("item_unit_price", this.ItemUnitPrice);
            parameters.Add("js_token_id", this.JsTokenId);
            parameters.Add("order_amount", this.OrderAmount);
            parameters.Add("order_category", this.OrderCategory);
            parameters.Add("order_credate_time", this.OrderCredateTime);
            parameters.Add("order_item_city", this.OrderItemCity);
            parameters.Add("order_item_name", this.OrderItemName);
            parameters.Add("order_no", this.OrderNo);
            parameters.Add("partner_id", this.PartnerId);
            parameters.Add("receiver_address", this.ReceiverAddress);
            parameters.Add("receiver_city", this.ReceiverCity);
            parameters.Add("receiver_district", this.ReceiverDistrict);
            parameters.Add("receiver_email", this.ReceiverEmail);
            parameters.Add("receiver_mobile", this.ReceiverMobile);
            parameters.Add("receiver_name", this.ReceiverName);
            parameters.Add("receiver_state", this.ReceiverState);
            parameters.Add("receiver_zip", this.ReceiverZip);
            parameters.Add("scene_code", this.SceneCode);
            parameters.Add("seller_account_no", this.SellerAccountNo);
            parameters.Add("seller_bind_bankcard", this.SellerBindBankcard);
            parameters.Add("seller_bind_bankcard_type", this.SellerBindBankcardType);
            parameters.Add("seller_bind_mobile", this.SellerBindMobile);
            parameters.Add("seller_identity_no", this.SellerIdentityNo);
            parameters.Add("seller_identity_type", this.SellerIdentityType);
            parameters.Add("seller_real_name", this.SellerRealName);
            parameters.Add("seller_reg_date", this.SellerRegDate);
            parameters.Add("seller_reg_email", this.SellerRegEmail);
            parameters.Add("seller_reg_moile", this.SellerRegMoile);
            parameters.Add("transport_type", this.TransportType);
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
