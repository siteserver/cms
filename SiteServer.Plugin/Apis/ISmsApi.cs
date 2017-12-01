using System.Collections.Generic;

namespace SiteServer.Plugin.Apis
{
    public interface ISmsApi
    {
        bool IsReady();

        bool Send(string mobile, string tplId, Dictionary<string, string> parameters, out string errorMessage);

        bool SendCode(string mobile, int code, string tplId, out string errorMessage);
    }
}
