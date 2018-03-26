using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using SiteServer.CMS.Core;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages
{
    public class PageError : Page
    {
        public Literal LtlMessage;
        public Literal LtlStackTrace;

        public void Page_Load(object sender, EventArgs e)
        {
            var message = string.Empty;
            var stackTrace = string.Empty;
            try
            {
                var logId = TranslateUtils.ToInt(Request.QueryString["logId"]);
                if (logId > 0)
                {
                    var pair = DataProvider.ErrorLogDao.GetMessageAndStacktrace(logId);
                    message = pair.Key;
                    stackTrace = pair.Value;

                    var rollbarTitle = message;
                    if (rollbarTitle == "未将对象引用设置到对象的实例。")
                    {
                        rollbarTitle = StringUtils.Guid();
                    }

                    LtlStackTrace.Text = $@"<script>Rollbar.error(""{StringUtils.ToJsString(rollbarTitle)}"", {{version: ""{StringUtils.ToJsString(SystemManager.Version) + (WebConfigUtils.IsNightlyUpdate ? "-nightly" : string.Empty)}"", stackTrace: ""{StringUtils.ToJsString(stackTrace)}""}});</script>";
                }
                if (string.IsNullOrEmpty(message))
                {
                    message = TranslateUtils.DecryptStringBySecretKey(Request.QueryString["message"]);
                    stackTrace = TranslateUtils.DecryptStringBySecretKey(Request.QueryString["stackTrace"]);
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                stackTrace = ex.StackTrace;
            }

            LtlMessage.Text = message;
            if (!string.IsNullOrEmpty(stackTrace))
            {
                LtlStackTrace.Text += $@"<!-- 
{stackTrace}
-->";
            }
        }
    }
}
