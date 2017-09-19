using System;
using System.Collections.Generic;
using Aop.Api.Response;
using Aop.Api.Util;

namespace Aop.Api.Request
{
    /// <summary>
    /// AOP API: alipay.open.public.life.msg.send
    /// </summary>
    public class AlipayOpenPublicLifeMsgSendRequest : IAopUploadRequest<AlipayOpenPublicLifeMsgSendResponse>
    {
        /// <summary>
        /// 消息分类，请传入对应分类编码值
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 消息正文，html原文或纯文本
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 消息背景图片（目前支持格式jpg, jpeg, bmp），需上传图片原始二进制流，图片最大1MB
        /// </summary>
        public FileItem Cover { get; set; }

        /// <summary>
        /// 消息概述
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 媒体资讯类生活号消息类型
        /// </summary>
        public string MsgType { get; set; }

        /// <summary>
        /// 消息来源方附属信息，供搜索、推荐使用  publish_time（int）：消息发布时间，单位秒  keyword_list（String）: 文章的标签列表，英文逗号分隔  comment（int）：消息来源处评论次数  reward（int）：消息来源处打赏次数  is_recommended（boolean）：消息在来源处是否被推荐  is_news（boolean）：消息是否实时性内容  read（int）：消息在来源处被阅读次数  like（int）：消息在来源处被点赞次数  is_hot（boolean）：消息在来源平台是否是热门内容  share（int）：文章在来源平台的分享次数  deadline（int）：文章的失效时间，单位秒
        /// </summary>
        public string SourceExtInfo { get; set; }

        /// <summary>
        /// 消息标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 来源方消息唯一标识；若不为空，根据此id和生活号id对消息去重；若为空，不去重
        /// </summary>
        public string UniqueMsgId { get; set; }

        /// <summary>
        /// 生活号消息视频时长，单位：秒（视频类消息必填）
        /// </summary>
        public string VideoLength { get; set; }

        /// <summary>
        /// 视频类型消息中视频抽样关键帧截图，视频类消息选填
        /// </summary>
        public List<string> VideoSamples { get; set; }

        /// <summary>
        /// 视频大小，单位：KB（视频类消息必填）
        /// </summary>
        public string VideoSize { get; set; }

        /// <summary>
        /// 视频资源来源id（视频类消息必填），取值限定youku, miaopai, taobao, sina中的一个
        /// </summary>
        public string VideoSource { get; set; }

        /// <summary>
        /// 视频的临时链接（优酷来源的视频消息，该字段不能为空）
        /// </summary>
        public string VideoTemporaryUrl { get; set; }

        /// <summary>
        /// 生活号视频类消息视频id或url（视频类消息必填，根据来源区分）
        /// </summary>
        public string VideoUrl { get; set; }

        #region IAopRequest Members
		private bool needEncrypt=false;
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

		public void SetApiVersion(string apiVersion){
            this.apiVersion=apiVersion;
        }

        public string GetApiVersion(){
            return this.apiVersion;
        }

        public string GetApiName()
        {
            return "alipay.open.public.life.msg.send";
        }

        public IDictionary<string, string> GetParameters()
        {
            AopDictionary parameters = new AopDictionary();
            parameters.Add("category", this.Category);
            parameters.Add("content", this.Content);
            parameters.Add("desc", this.Desc);
            parameters.Add("msg_type", this.MsgType);
            parameters.Add("source_ext_info", this.SourceExtInfo);
            parameters.Add("title", this.Title);
            parameters.Add("unique_msg_id", this.UniqueMsgId);
            parameters.Add("video_length", this.VideoLength);
            parameters.Add("video_samples", this.VideoSamples);
            parameters.Add("video_size", this.VideoSize);
            parameters.Add("video_source", this.VideoSource);
            parameters.Add("video_temporary_url", this.VideoTemporaryUrl);
            parameters.Add("video_url", this.VideoUrl);
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

        #region IAopUploadRequest Members

        public IDictionary<string, FileItem> GetFileParameters()
        {
            IDictionary<string, FileItem> parameters = new Dictionary<string, FileItem>();
            parameters.Add("cover", this.Cover);
            return parameters;
        }

        #endregion
    }
}
