using System;
using System.Collections.Generic;
using System.Reflection;
using Datory.Annotations;
using Newtonsoft.Json.Linq;

namespace Datory.Utils
{
    public static class ValueUtils
    {
        public static object GetSqlValue(Entity dataInfo, TableColumn tableColumn)
        {
            var value = dataInfo.Get(tableColumn.AttributeName);

            if (value is Enum enumValue)
            {
                value = enumValue.GetValue();
            }
            else if (value is List<string> stringList)
            {
                value = Utilities.ToString(stringList);
            }
            else if (value is List<int> intList)
            {
                value = Utilities.ToString(intList);
            }

            return value;
        }

        public static object GetValue(object obj, string propertyName)
        {
            var propertyInfo = ReflectionUtils.GetTypeProperty(obj.GetType(), propertyName);
            if (propertyInfo != null && propertyInfo.CanRead)
            {
                var propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

                var val = propertyInfo.GetValue(obj, null);

                if (propertyType.IsEnum)
                {
                    try
                    {
                        return Enum.Parse(propertyType, val.ToString(), true);
                    }
                    catch
                    {
                        // ignored
                    }
                }

                return val;
            }

            return null;
        }

        public static void SetValue(object obj, string propertyName, object value)
        {
            var propertyInfo = ReflectionUtils.GetTypeProperty(obj.GetType(), propertyName);

            if (propertyInfo != null && propertyInfo.CanWrite)
            {
                var propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

                if (value == null)
                {
                    propertyInfo.SetValue(obj, null, null);
                }
                else if (propertyType.IsEnum)
                {
                    try
                    {
                        //var enumType = (Enum)value;
                        //var enumValue = enumType.GetValue();
                        //propertyInfo.SetValue(obj, enumValue, null);
                        propertyInfo.SetValue(obj, Enum.Parse(propertyType, value.ToString(), true), null);
                    }
                    catch
                    {
                        propertyInfo.SetValue(obj, ChangeType(value, propertyType), null);
                    }
                }
                else if (typeof(List<string>).IsAssignableFrom(propertyType))
                {
                    if (value is List<string> stringList)
                    {
                        propertyInfo.SetValue(obj, stringList, null);
                    }
                    else if (value is JArray jArray)
                    {
                        propertyInfo.SetValue(obj, Utilities.GetStringList(jArray), null);
                    }
                    else
                    {
                        propertyInfo.SetValue(obj, Utilities.GetStringList(value.ToString()), null);
                    }
                }
                else if (typeof(List<int>).IsAssignableFrom(propertyType))
                {
                    if (value is List<int> intList)
                    {
                        propertyInfo.SetValue(obj, intList, null);
                    }
                    else if (value is JArray jArray)
                    {
                        propertyInfo.SetValue(obj, Utilities.GetIntList(jArray), null);
                    }
                    else
                    {
                        propertyInfo.SetValue(obj, Utilities.GetIntList(value.ToString()), null);
                    }
                }
                else
                {
                    propertyInfo.SetValue(obj, ChangeType(value, propertyType), null);
                }
            }
        }

        private static object ChangeType(object value, Type conversionType)
        {
            try
            {
                var type = Nullable.GetUnderlyingType(conversionType) ?? conversionType;
                return Convert.ChangeType(value, type);
            }
            catch
            {
                return GetDefault(conversionType);
            }
        }

        private static object GetDefault(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        public static TableColumn GetTableColumn(PropertyInfo propertyInfo)
        {
            var attribute = propertyInfo.GetCustomAttribute<DataColumnAttribute>(true);

            if (attribute == null) return null;

            var dataType = DataType.VarChar;
            var dataLength = 0;

            var propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

            if (propertyType == typeof(string) ||
                propertyType == typeof(char) ||
                propertyType == typeof(Enum) ||
                propertyType == typeof(List<int>) ||
                propertyType == typeof(List<string>))
            {
                if (attribute.Text)
                {
                    dataType = DataType.Text;
                }
                else
                {
                    dataType = DataType.VarChar;
                    dataLength = attribute.Length;
                }
            }
            else if (propertyType == typeof(int))
            {
                dataType = DataType.Integer;
            }
            else if (propertyType == typeof(bool))
            {
                dataType = DataType.Boolean;
            }
            else if (propertyType == typeof(DateTimeOffset) || propertyType == typeof(DateTime))
            {
                dataType = DataType.DateTime;
            }
            else if (propertyType == typeof(double) || propertyType == typeof(decimal))
            {
                dataType = DataType.Decimal;
            }

            var isPrimaryKey = Utilities.EqualsIgnoreCase(propertyInfo.Name, nameof(Entity.Id));

            if (dataType == DataType.VarChar && dataLength <= 0)
            {
                dataLength = DbUtils.VarCharDefaultLength;
            }

            return new TableColumn
            {
                AttributeName = propertyInfo.Name,
                DataType = dataType,
                DataLength = dataLength,
                IsIdentity = isPrimaryKey,
                IsPrimaryKey = isPrimaryKey,
            };
        }
    }
}