using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using SiteServer.CMS.Plugin.Apis;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages
{
    public class PageTest : Page
    {
        public Literal LtlContent;

        public void Page_Load(object sender, EventArgs e)
        {
            
            //LtlContent.Text = Regex.Replace(@"<!DOCTYPE html><html><head><meta charset=""utf-8""><meta name=""viewport"" content=""width=device-width,initial-scale=1""><title>用户中心</title><meta name=""renderer"" content=""webkit""/><link rel=""icon"" type=""image/x-icon"" href=""assets/favicon.png""><script type=""text/javascript"" src=""assets/js/jquery-1.9.1.min.js""></script><script type=""text/javascript"" src=""assets/js/jquery.imgareaselect.min.js""></script><script type=""text/javascript"">var config={domainAPI:""http://localhost:88/api/plugins/SS.Home""}</script></head><body><div id=""root""></div><script type=""text/javascript"" src=""assets/ueditor-1.4.3.2/ueditor.config.js""></script><script type=""text/javascript"" src=""assets/ueditor-1.4.3.2/ueditor.all.min.js""></script><script type=""text/javascript"" src=""assets/ueditor-1.4.3.2/lang/zh-cn/zh-cn.js""></script><script type=""text/javascript"" src=""main.71852d4a.min.js""></script></body></html>", "domainAPI:\"[^\"]*\"", $@"domainAPI:""{2}""");

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
