using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalTipMessage : BasePageCms
    {
        public Literal ltlTips;

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
            return PageUtils.GetOpenWindowString(title, PageUtils.GetCmsUrl(nameof(ModalTipMessage), new NameValueCollection
            {
                {"content", TranslateUtils.EncryptStringBySecretKey(content)}
            }), 500, 500, true);
        }

        public void Cancel_OnClick(object sender, EventArgs e)
        {
            PageUtils.CloseModalPageWithoutRefresh(Page);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (!IsPostBack)
            {
                ltlTips.Text = TranslateUtils.DecryptStringBySecretKey(Request.QueryString["content"]);
            }
        }
    }
}
