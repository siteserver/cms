using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using SiteServer.CMS.Core;
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

        // MODEL Reference
        public void Page_Load(object sender, EventArgs e)
        {
            var builder = new StringBuilder();
            foreach (var provider in DataProvider.AllProviders)
            {
                if (string.IsNullOrEmpty(provider.TableName) || provider.TableColumns == null ||
                    provider.TableColumns.Count == 0) continue;

                builder.Append($@"{provider.TableName}<br /><br />
字段  | 数据类型  | 数据大小  | 说明3<br />
------  | ------  | ------  | ------<br />
");
                foreach (var column in provider.TableColumns)
                {
                    builder.Append($"{column.AttributeName} | {column.DataType} | {(column.DataLength == 0 ? string.Empty : column.DataLength.ToString())} | {(column.IsIdentity ? "自增长" : string.Empty) + (column.IsPrimaryKey ? "主键" : string.Empty)}<br />");
                }

                builder.Append("<br /><br />");
            }

            builder.Append($@"model_Content<br /><br />
字段  | 数据类型  | 数据大小  | 说明<br />
------  | ------  | ------  | ------<br />
");
            foreach (var column in DataProvider.ContentDao.TableColumns)
            {
                builder.Append($"{column.AttributeName} | {column.DataType} | {(column.DataLength == 0 ? string.Empty : column.DataLength.ToString())} | {(column.IsIdentity ? "自增长" : string.Empty) + (column.IsPrimaryKey ? "主键" : string.Empty)}<br />");
            }

            LtlContent.Text = builder.ToString();
        }

        // STL Reference
        //public void Page_Load(object sender, EventArgs e)
        //{
        //    FieldInfo[] fields = typeof(ChannelAttribute).GetFields(BindingFlags.Static | BindingFlags.Public);

        //    LtlContent.Text = DisplayPropertyInfo(fields);
        //}

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
