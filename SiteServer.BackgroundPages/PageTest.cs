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
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages
{
    public class PageTest : Page
    {
        public Literal LtlContent;

        public void Page_Load(object sender, EventArgs e)
        {
            //FieldInfo[] fields = typeof(ChannelAttribute).GetFields(BindingFlags.Static | BindingFlags.Public);


            //LtlContent.Text = DisplayPropertyInfo(fields);

            //LtlContent.Text += "Original: eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJBZG1pbmlzdHJhdG9yTmFtZSI6InRlc3QxIiwiQWRkRGF0ZSI6IlwvRGF0ZSgyMTU3NjA5OTg1NTA3KVwvIn0.5yDhtZUa7iT0axC1hIP6ohVpSGt_3jIhtf_FQNPbZUU" + "<hr />";

            //LtlContent.Text += $"Encrypt: {TranslateUtils.EncryptStringBySecretKey("eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJBZG1pbmlzdHJhdG9yTmFtZSI6InRlc3QxIiwiQWRkRGF0ZSI6IlwvRGF0ZSgyMTU3NjA5OTg1NTA3KVwvIn0.5yDhtZUa7iT0axC1hIP6ohVpSGt_3jIhtf_FQNPbZUU")}" + "<hr />";

            LtlContent.Text += $"Decrypt: {TranslateUtils.DecryptStringBySecretKey("M3ENIa3NKJJ39JCRHnY4PgfJqMC7lFjggL0e9S06Bs9ubZE90add0xM2aesaL0add0Cxo8Xe5VZrSanerzFU8oZaMXCC9DZXJNsl9usrbd9oVcgoA34PNCM50tzhAIxAZUKcuBpZ4zSwm5OFBaY33YmUCvqQM441S84eHTVj3Mu0B4I0slash0UeXTHilH0slash0Gwi7Bo01Dzystb0slash00slash0hdJaSBi8Wtk8OLQDt2z8S5XZHOnrS1")}";
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
