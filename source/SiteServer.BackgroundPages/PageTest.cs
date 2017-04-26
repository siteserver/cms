using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaiRong.Core;

namespace SiteServer.BackgroundPages
{
    public class PageTest : Page
    {
        public Literal LtlContent;

        public void Page_Load(object sender, EventArgs e)
        {
            LtlContent.Text = string.Empty;
            foreach (var c in PathUtils.InvalidPathChars)
            {
                LtlContent.Text += c + ",";
            }
        }
    }
}
