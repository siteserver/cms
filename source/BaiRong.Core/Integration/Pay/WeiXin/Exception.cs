using System;
using System.Collections.Generic;
using System.Web;

namespace SiteServer.B2C.Core.Union
{
    public class WxPayException : Exception 
    {
        public WxPayException(string msg) : base(msg) 
        {

        }
     }
}