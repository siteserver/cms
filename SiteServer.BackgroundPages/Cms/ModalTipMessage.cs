using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalTipMessage : BasePageCms
    {
        public Literal LtlTips;

        protected override bool IsSinglePage => true;

        public static string GetRedirectUrlString(string content)
        {
            return PageUtils.GetCmsUrl(nameof(ModalTipMessage), new NameValueCollection
            {
                {"content", TranslateUtils.EncryptStringBySecretKey(content)}
            });
        }

        public static string GetOpenWindowString(string title, string content)
        {
            return LayerUtils.GetOpenScript(title,
                PageUtils.GetCmsUrl(nameof(ModalTipMessage), new NameValueCollection
                {
                    {"content", TranslateUtils.EncryptStringBySecretKey(content)}
                }), 500, 500);
        }

        public void Cancel_OnClick(object sender, EventArgs e)
        {
            LayerUtils.CloseWithoutRefresh(Page);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (!IsPostBack)
            {
                LtlTips.Text = TranslateUtils.DecryptStringBySecretKey(Request.QueryString["content"]);
            }
        }
    }
}
