using System;
using System.Web.UI.WebControls;
using BaiRong.Core;

namespace SiteServer.BackgroundPages
{
    public class PageError : BasePage
    {
        public Literal ltlErrorMessage;

        protected override bool IsAccessable => true;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (!Page.IsPostBack)
            {
                if (Body.IsQueryExists("ErrorMessage"))
                {
                    var errorMessage = PageUtils.FilterXss(StringUtils.ValueFromUrl(Body.GetQueryString("ErrorMessage")));
                    ltlErrorMessage.Text = errorMessage;
                }
            }
        }
    }
}
