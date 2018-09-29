using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Security.Principal;
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
    public class MyTableInfo
    {
        public List<TableColumn> Columns { get; set; }
        public int TotalCount { get; set; }
        public List<string> RowFiles { get; set; }
    }

    public class PageTest : Page
    {
        public Literal LtlContent;

        public void Page_Load(object sender, EventArgs e)
        {
            var json = @"{
    ""columns"": [
        {
            ""attributeName"": ""IsAbolition"",
            ""dataType"": ""VarChar"",
            ""dataLength"": 10,
            ""isPrimaryKey"": false,
            ""isIdentity"": false,
            ""inputStyle"": {
                ""inputType"": ""Radio"",
                ""displayName"": ""是否废止"",
                ""helpText"": null,
                ""listItems"": [
                    {
                        ""text"": ""是"",
                        ""value"": ""True"",
                        ""selected"": false
                    },
                    {
                        ""text"": ""否"",
                        ""value"": ""False"",
                        ""selected"": true
                    }
                ],
                ""defaultValue"": null,
                ""isRequired"": true,
                ""validateType"": null,
                ""minNum"": 0,
                ""maxNum"": 0,
                ""regExp"": null,
                ""width"": null,
                ""height"": null
            }
        }
    ],
    ""totalCount"": 796,
    ""rowFiles"": [
        ""1.json"",
        ""2.json""
    ]
}";

            var tableInfo = TranslateUtils.JsonDeserialize<MyTableInfo>(json);

            LtlContent.Text = "";
        }

        // MODEL Reference
        //        public void Page_Load(object sender, EventArgs e)
        //        {
        //            var builder = new StringBuilder();
        //            foreach (var provider in DataProvider.AllProviders)
        //            {
        //                if (string.IsNullOrEmpty(provider.TableName) || provider.TableColumns == null ||
        //                    provider.TableColumns.Count == 0) continue;

        //                builder.Append($@"{provider.TableName}<br /><br />
        //字段  | 数据类型  | 数据大小  | 说明3<br />
        //------  | ------  | ------  | ------<br />
        //");
        //                foreach (var column in provider.TableColumns)
        //                {
        //                    builder.Append($"{column.AttributeName} | {column.DataType} | {(column.DataLength == 0 ? string.Empty : column.DataLength.ToString())} | {(column.IsIdentity ? "自增长" : string.Empty) + (column.IsPrimaryKey ? "主键" : string.Empty)}<br />");
        //                }

        //                builder.Append("<br /><br />");
        //            }

        //            builder.Append($@"model_Content<br /><br />
        //字段  | 数据类型  | 数据大小  | 说明<br />
        //------  | ------  | ------  | ------<br />
        //");
        //            foreach (var column in DataProvider.ContentDao.TableColumns)
        //            {
        //                builder.Append($"{column.AttributeName} | {column.DataType} | {(column.DataLength == 0 ? string.Empty : column.DataLength.ToString())} | {(column.IsIdentity ? "自增长" : string.Empty) + (column.IsPrimaryKey ? "主键" : string.Empty)}<br />");
        //            }

        //            LtlContent.Text = builder.ToString();
        //        }

        // STL Reference
        //public void Page_Load(object sender, EventArgs e)
        //{
        //    var _elementName = Request.QueryString["name"];

        //    Type elementType;
        //    if (StlAll.Elements.TryGetValue(_elementName, out elementType))
        //    {
        //        var elementBuilder = new StringBuilder();
        //        var tableBuilder = new StringBuilder();

        //        var elementName = _elementName.Replace("stl:", string.Empty);

        //        var fields = elementType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        //        foreach (var field in fields)
        //        {
        //            var stlAttribute =
        //                (StlAttributeAttribute)Attribute.GetCustomAttribute(field, typeof(StlAttributeAttribute));

        //            if (stlAttribute != null)
        //            {
        //                var attrName = field.Name.ToCamelCase();

        //                elementBuilder.Append($@"&nbsp;{attrName}=""{stlAttribute.Title}""<br />");

        //                tableBuilder.Append($@"|[{attrName}]({elementName}/attributes?id={attrName}) | {stlAttribute.Title} | <br />");
        //            }
        //        }

        //        if (elementName == "channels" || elementName == "contents" || elementName == "each" || elementName == "sites" || elementName == "sqlContents")
        //        {
        //            fields = typeof(StlListBase).GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        //            foreach (var field in fields)
        //            {
        //                var stlAttribute =
        //                    (StlAttributeAttribute)Attribute.GetCustomAttribute(field, typeof(StlAttributeAttribute));

        //                if (stlAttribute != null)
        //                {
        //                    var attrName = field.Name.ToCamelCase();

        //                    elementBuilder.Append($@"&nbsp;{attrName}=""{stlAttribute.Title}""<br />");

        //                    tableBuilder.Append($@"|[{attrName}]({elementName}/attributes?id={attrName}) | {stlAttribute.Title} | <br />");
        //                }
        //            }
        //        }

        //        if (elementBuilder.Length > 0) elementBuilder.Length -= 6;

        //        LtlContent.Text = $"&lt;stl:{elementName}<br />{elementBuilder}&gt;<br />&lt;/stl:{elementName}&gt;" + "<br /><hr /><br />" + tableBuilder;

        //    }
        //}
    }
}
