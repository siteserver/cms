using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalTipMessage : BasePageCms
    {
        public Literal LtlTips;

        protected override bool IsSinglePage => true;

        public static string GetRedirectUrlString(int siteId, string content)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(ModalTipMessage), new NameValueCollection
            {
                {"content", TranslateUtils.EncryptStringBySecretKey(content)}
            });
        }

        public static string GetOpenWindowString(int siteId, string title, string content)
        {
            return LayerUtils.GetOpenScript(title,
                PageUtils.GetCmsUrl(siteId, nameof(ModalTipMessage), new NameValueCollection
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
