using System.Collections.Generic;
using SiteServer.CMS.Core;
using SiteServer.Plugin.Apis;

namespace SiteServer.CMS.Plugin.Apis
{
    public class SmsApi
    {
        private SmsApi() { }

        private static SmsApi _instance;
        public static SmsApi Instance => _instance ?? (_instance = new SmsApi());

        public bool IsReady()
        {
            return SmsManager.IsReady();
        }

        public bool Send(string mobile, string tplId, Dictionary<string, string> parameters, out string errorMessage)
        {
            return SmsManager.Send(mobile, tplId, parameters, out errorMessage);
        }

        public bool SendCode(string mobile, int code, string tplId, out string errorMessage)
        {
            return SmsManager.SendCode(mobile, code, tplId, out errorMessage);
        }
    }
}
