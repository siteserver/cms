using System;

namespace SiteServer.CMS.WeiXin.WeiXinMP.Exceptions
{
    /// <summary>
    /// 微信自定义异常基类
    /// </summary>
    public class WeixinException : ApplicationException
    {
        public WeixinException(string message)
            : base(message, null)
        {
        }

        public WeixinException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
