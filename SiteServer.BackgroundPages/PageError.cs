using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaiRong.Core;

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
                    var pair = BaiRongDataProvider.ErrorLogDao.GetMessageAndStacktrace(logId);
                    message = pair.Key;
                    stackTrace = pair.Value;
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
                LtlStackTrace.Text = $@"<!-- 
{stackTrace}
-->";
            }
        }
    }
}
