using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.Plugin.Apis;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages
{
    public class PageTest : Page
    {
        public Literal LtlContent;

        public void Page_Load(object sender, EventArgs e)
        {
            FieldInfo[] fields = typeof(ChannelAttribute).GetFields(BindingFlags.Static | BindingFlags.Public);


            LtlContent.Text = DisplayPropertyInfo(fields);
        }

        public static string DisplayPropertyInfo(FieldInfo[] propInfos)
        {
            var builder = new StringBuilder();
            // Display information for all properties.
            foreach (var propInfo in propInfos)
            {

                builder.AppendFormat("   Property name: {0}", propInfo.Name);
                builder.AppendFormat("   Property value: {0}", propInfo.GetValue(null));
            }

            return builder.ToString();
        }
    }
}
