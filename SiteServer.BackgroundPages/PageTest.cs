using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using SiteServer.CMS.Plugin.Core;

namespace SiteServer.BackgroundPages
{
    public class PageTest : Page
    {
        public Literal LtlContent;

        public async void Page_Load(object sender, EventArgs e)
        {
            LtlContent.Text = NuGetManager.TestGetLastPackage(false);

            LtlContent.Text += "<br /><hr /></br />";

            LtlContent.Text += NuGetManager.TestGetReleaseVersionList();

            //DataProvider.DatabaseDao.Test();
        }
    }
}
