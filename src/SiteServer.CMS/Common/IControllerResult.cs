using System;
using System.Collections.Generic;
using System.Text;

namespace SiteServer.CMS.Common
{
    public interface IControllerResult
    {
        int StatusCode { get; }
        object Result { get; }
    }
}
