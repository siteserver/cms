using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.kfc.keyword.search
    /// </summary>
    public class KfcKeywordSearchRequest : BaseTopRequest<Top.Api.Response.KfcKeywordSearchResponse>
    {
        /// <summary>
        /// 应用点，分为一级应用点、二级应用点。其中一级应用点通常是指某一个系统或产品，比如淘宝的商品应用（taobao_auction）；二级应用点，是指一级应用点下的具体的分类，比如商品标题(title)、商品描述(content)。不同的二级应用可以设置不同关键词。    这里的apply参数是由一级应用点与二级应用点合起来的字符（一级应用点+"."+二级应用点），如taobao_auction.title。      通常apply参数是不需要传递的。如有特殊需求（比如特殊的过滤需求，需要自己维护一套自己词库），需传递此参数。
        /// </summary>
        public string Apply { get; set; }

        /// <summary>
        /// 需要过滤的文本信息
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 发布信息的淘宝会员名，可以不传
        /// </summary>
        public string Nick { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.kfc.keyword.search";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("apply", this.Apply);
            parameters.Add("content", this.Content);
            parameters.Add("nick", this.Nick);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("content", this.Content);
        }

        #endregion
    }
}
