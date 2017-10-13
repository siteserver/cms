using System;
using System.Collections.Generic;
using Aop.Api.Response;
using Aop.Api.Util;

namespace Aop.Api.Request
{
    /// <summary>
    /// AOP API: alipay.zdatafront.datatransfered.fileupload
    /// </summary>
    public class AlipayZdatafrontDatatransferedFileuploadRequest : IAopUploadRequest<AlipayZdatafrontDatatransferedFileuploadResponse>
    {
        /// <summary>
        /// 合作伙伴上传文件中的各字段,使用英文半角","分隔，file_type为json_data时必选
        /// </summary>
        public string Columns { get; set; }

        /// <summary>
        /// 二进制字节数组，由文件转出，最大支持50M文件的上传
        /// </summary>
        public FileItem File { get; set; }

        /// <summary>
        /// 文件描述信息，非解析数据类型必选
        /// </summary>
        public string FileDescription { get; set; }

        /// <summary>
        /// 文件摘要，算法SHA
        /// </summary>
        public string FileDigest { get; set; }

        /// <summary>
        /// 描述上传文件的类型
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// 上传数据文件的主键字段，注意该pk若出现重复则后入数据会覆盖前面的，file_type为json_data时必选
        /// </summary>
        public string PrimaryKey { get; set; }

        /// <summary>
        /// 上传数据文件包含的记录数，file_type为json_data时必选
        /// </summary>
        public Nullable<long> Records { get; set; }

        /// <summary>
        /// 外部公司的数据源标识信息，由联接网络分配
        /// </summary>
        public string TypeId { get; set; }

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
            return "alipay.zdatafront.datatransfered.fileupload";
        }

        public IDictionary<string, string> GetParameters()
        {
            AopDictionary parameters = new AopDictionary();
            parameters.Add("columns", this.Columns);
            parameters.Add("file_description", this.FileDescription);
            parameters.Add("file_digest", this.FileDigest);
            parameters.Add("file_type", this.FileType);
            parameters.Add("primary_key", this.PrimaryKey);
            parameters.Add("records", this.Records);
            parameters.Add("type_id", this.TypeId);
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
            parameters.Add("file", this.File);
            return parameters;
        }

        #endregion
    }
}
