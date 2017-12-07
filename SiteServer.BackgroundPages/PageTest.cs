using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SiteServer.BackgroundPages
{
    public class PageTest : Page
    {
        public Literal LtlContent;

        public void Page_Load(object sender, EventArgs e)
        {
            LtlContent.Text = string.Empty;

            //BaiRongDataProvider.DatabaseDao.Test();
        }
    }
}
