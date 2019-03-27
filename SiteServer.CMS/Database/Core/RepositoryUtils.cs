using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SiteServer.CMS.Database.Wrapper;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Database.Core
{
    public static class RepositoryUtils
    {
        //public static List<TableColumn> GetTableColumns(Type type)
        //{
        //    var tableColumns = new List<TableColumn>();

        //    foreach (var propertyInfo in ReflectionUtils.GetTypeProperties(type))
        //    {
        //        var attributes = propertyInfo.GetCustomAttributes(true);

        //        if (attributes.Any(a => a is ComputedAttribute)) continue;

        //        var dataType = DataType.VarChar;
        //        var dataLength = 0;
        //        if (propertyInfo.PropertyType == typeof(string))
        //        {
        //            var isText = attributes.Any(a => a is TextAttribute);
        //            if (isText)
        //            {
        //                dataType = DataType.Text;
        //            }
        //            else
        //            {
        //                dataType = DataType.VarChar;
        //                var varCharAttribute = (VarCharAttribute)attributes.FirstOrDefault(a => a is VarCharAttribute);
        //                dataLength = varCharAttribute?.Length ?? 2000;
        //            }
        //        }
        //        else if (propertyInfo.PropertyType == typeof(int))
        //        {
        //            dataType = DataType.Integer;
        //        }
        //        else if (propertyInfo.PropertyType == typeof(bool))
        //        {
        //            dataType = DataType.Boolean;
        //        }
        //        else if (propertyInfo.PropertyType == typeof(DateTimeOffset) || propertyInfo.PropertyType == typeof(DateTime))
        //        {
        //            dataType = DataType.DateTime;
        //        }
        //        else if (propertyInfo.PropertyType == typeof(double) || propertyInfo.PropertyType == typeof(decimal))
        //        {
        //            dataType = DataType.Decimal;
        //        }

        //        var tableColumn = new TableColumn
        //        {
        //            AttributeName = propertyInfo.Name,
        //            DataType = dataType,
        //            DataLength = dataLength
        //        };

        //        tableColumns.Add(tableColumn);
        //    }

        //    return tableColumns;
        //}

        //private static IDbConnection GetConnection()
        //{
        //    return SqlDifferences.GetIDbConnection(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString);
        //}

        //public static IDictionary<string, object> ObjToDict(IEntity dataInfo, IList<string> columns, params string[] excludes)
        //{
        //    var excludesList = excludes != null ? excludes.ToList() : new List<string>();
        //    var values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        //    foreach (var column in columns)
        //    {
        //        if (StringUtils.ContainsIgnoreCase(excludesList, column)) continue;
        //        if (ReflectionUtils.GetValue(dataInfo, column, out var val))
        //        {
        //            values[column] = val;
        //        }
        //    }

        //    return values;
        //}
    }
}
