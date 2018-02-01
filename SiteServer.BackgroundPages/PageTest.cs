using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using SiteServer.CMS.Packaging;

namespace SiteServer.BackgroundPages
{
    public class PageTest : Page
    {
        public Literal LtlContent;

        public void Page_Load(object sender, EventArgs e)
        {
            //var list = PackageUtils.GetDependencyGroups("SS.Payment.1.1.5-beta");
            //foreach (var str in list)
            //{
            //    LtlContent.Text += $"{str}<hr /></br />";
            //}



            //string title;
            //string version;
            //DateTimeOffset? published;
            //string releaseNotes;
            //if (PackageUtils.FindLastPackage(PackageUtils.PackageIdSsCms, out title, out version, out published, out releaseNotes))
            //{
            //    LtlContent.Text += $"version: {version}<br />published: {published}<br />releaseNotes: {releaseNotes}<hr /></br />";
            //}
            //if (PackageUtils.FindLastPackage(PackageUtils.PackageIdSsCms, out title, out version, out published, out releaseNotes))
            //{
            //    LtlContent.Text += $"version: {version}<br />published: {published}<br />releaseNotes: {releaseNotes}<hr /></br />";
            //}
        }
    }
}
