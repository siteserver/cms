using System;
using Newtonsoft.Json;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Model
{
    public class TableColumnInfo
    {
        public TableColumnInfo()
        {
            ColumnName = string.Empty;
            DataType = DataType.VarChar;
            Length = 0;
            IsPrimaryKey = false;
            IsIdentity = false;
        }

        public TableColumnInfo(string columnName, DataType dataType, int length, bool isPrimaryKey, bool isIdentity)
        {
            ColumnName = columnName;
            DataType = dataType;
            Length = length;
            IsPrimaryKey = isPrimaryKey;
            IsIdentity = isIdentity;
        }

        public string ColumnName { get; set; }

        [JsonConverter(typeof(DataTypeConverter))]
        public DataType DataType { get; set; }

        public int Length { get; set; }

        public bool IsPrimaryKey { get; set; }

        public bool IsIdentity { get; set; }
    }

    public class DataTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DataType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var dataType = value as DataType;
            serializer.Serialize(writer, dataType != null ? dataType.Value : null);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return DataTypeUtils.GetEnumType((string)reader.Value);
        }
    }
}
