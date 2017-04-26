using System;
using System.Collections;
using System.Collections.Generic;
#if !SILVERLIGHT
using System.Data;
#endif
using System.Globalization;
using System.IO;
using System.Text;
using System.Collections.Specialized;
using Top.Api;
using Top.Api.Util;

namespace FastJSON
{
    internal sealed class JSONSerializer
    {
        private StringBuilder _output = new StringBuilder();
        //private StringBuilder _before = new StringBuilder();
        private int _before;
        private int _MAX_DEPTH = 20;
        int _current_depth = 0;
        private Dictionary<string, int> _globalTypes = new Dictionary<string, int>();
        private Dictionary<object, int> _cirobj = new Dictionary<object, int>();
        private JSONParameters _params;
        private bool _useEscapedUnicode = false;

        internal JSONSerializer(JSONParameters param)
        {
            _params = param;
            _useEscapedUnicode = _params.UseEscapedUnicode;
            _MAX_DEPTH = _params.SerializerMaxDepth;
        }

        internal string ConvertToJSON(object obj)
        {
            WriteValue(obj);

            if (_params.UsingGlobalTypes && _globalTypes != null && _globalTypes.Count > 0)
            {
                var sb = new StringBuilder();
                sb.Append("\"$types\":{");
                var pendingSeparator = false;
                foreach (var kv in _globalTypes)
                {
                    if (pendingSeparator) sb.Append(',');
                    pendingSeparator = true;
                    sb.Append('\"');
                    sb.Append(kv.Key);
                    sb.Append("\":\"");
                    sb.Append(kv.Value);
                    sb.Append('\"');
                }
                sb.Append("},");
                _output.Insert(_before, sb.ToString());
            }
            return _output.ToString();
        }

        private void WriteValue(object obj)
        {
            if (obj == null || obj is DBNull)
                _output.Append("null");

            else if (obj is string || obj is char)
                WriteString(obj.ToString());

            else if (obj is Guid)
                WriteGuid((Guid)obj);

            else if (obj is bool)
                _output.Append(((bool)obj) ? "true" : "false"); // conform to standard

            else if (
                obj is int || obj is long || obj is double ||
                obj is decimal || obj is float ||
                obj is byte || obj is short ||
                obj is sbyte || obj is ushort ||
                obj is uint || obj is ulong
            )
                _output.Append(((IConvertible)obj).ToString(NumberFormatInfo.InvariantInfo));

            else if (obj is DateTime)
                WriteDateTime((DateTime)obj);

            else if (_params.KVStyleStringDictionary == false && obj is IDictionary &&
                obj.GetType().IsGenericType && obj.GetType().GetGenericArguments()[0] == typeof(string))

                WriteStringDictionary((IDictionary)obj);
#if net4
            else if (_params.KVStyleStringDictionary == false && obj is System.Dynamic.ExpandoObject)
                WriteStringDictionary((IDictionary<string, object>)obj);
#endif
            else if (obj is IDictionary)
                WriteDictionary((IDictionary)obj);
#if !SILVERLIGHT
            else if (obj is DataSet)
                WriteDataset((DataSet)obj);

            else if (obj is DataTable)
                this.WriteDataTable((DataTable)obj);
#endif
            else if (obj is byte[])
                WriteBytes((byte[])obj);

            else if (obj is StringDictionary)
                WriteSD((StringDictionary)obj);

            else if (obj is NameValueCollection)
                WriteNV((NameValueCollection)obj);

            else if (obj is IEnumerable)
                WriteArray((IEnumerable)obj);

            else if (obj is Enum)
                WriteEnum((Enum)obj);

            else if (Reflection.Instance.IsTypeRegistered(obj.GetType()))
                WriteCustom(obj);

            else
                WriteObject(obj);
        }

        private void WriteNV(NameValueCollection nameValueCollection)
        {
            _output.Append('{');

            bool pendingSeparator = false;

            foreach (string key in nameValueCollection)
            {
                if (_params.SerializeNullValues == false && (nameValueCollection[key] == null))
                {
                }
                else
                {
                    if (pendingSeparator) _output.Append(',');
                    if (_params.SerializeToLowerCaseNames)
                        WritePair(key.ToLower(), nameValueCollection[key]);
                    else
                        WritePair(key, nameValueCollection[key]);
                    pendingSeparator = true;
                }
            }
            _output.Append('}');
        }

        private void WriteSD(StringDictionary stringDictionary)
        {
            _output.Append('{');

            bool pendingSeparator = false;

            foreach (DictionaryEntry entry in stringDictionary)
            {
                if (_params.SerializeNullValues == false && (entry.Value == null))
                {
                }
                else
                {
                    if (pendingSeparator) _output.Append(',');

                    string k = (string)entry.Key;
                    if (_params.SerializeToLowerCaseNames)
                        WritePair(k.ToLower(), entry.Value);
                    else
                        WritePair(k, entry.Value);
                    pendingSeparator = true;
                }
            }
            _output.Append('}');
        }

        private void WriteCustom(object obj)
        {
            Serialize s;
            Reflection.Instance._customSerializer.TryGetValue(obj.GetType(), out s);
            WriteStringFast(s(obj));
        }

        private void WriteEnum(Enum e)
        {
            // TODO : optimize enum write
            if (_params.UseValuesOfEnums)
                WriteValue(Convert.ToInt32(e));
            else
                WriteStringFast(e.ToString());
        }

        private void WriteGuid(Guid g)
        {
            if (_params.UseFastGuid == false)
                WriteStringFast(g.ToString());
            else
                WriteBytes(g.ToByteArray());
        }

        private void WriteBytes(byte[] bytes)
        {
#if !SILVERLIGHT
            WriteStringFast(Convert.ToBase64String(bytes, 0, bytes.Length, Base64FormattingOptions.None));
#else
            WriteStringFast(Convert.ToBase64String(bytes, 0, bytes.Length));
#endif
        }

        private void WriteDateTime(DateTime dateTime)
        {
            if (_params.UseApiNamingStyle)
            {
                WriteString(dateTime.ToString(Constants.DATE_TIME_FORMAT));
                return;
            }
            // datetime format standard : yyyy-MM-dd HH:mm:ss
            DateTime dt = dateTime;
            if (_params.UseUTCDateTime)
                dt = dateTime.ToUniversalTime();

            _output.Append('\"');
            _output.Append(dt.Year.ToString("0000", NumberFormatInfo.InvariantInfo));
            _output.Append('-');
            _output.Append(dt.Month.ToString("00", NumberFormatInfo.InvariantInfo));
            _output.Append('-');
            _output.Append(dt.Day.ToString("00", NumberFormatInfo.InvariantInfo));
            _output.Append('T'); // strict ISO date compliance 
            _output.Append(dt.Hour.ToString("00", NumberFormatInfo.InvariantInfo));
            _output.Append(':');
            _output.Append(dt.Minute.ToString("00", NumberFormatInfo.InvariantInfo));
            _output.Append(':');
            _output.Append(dt.Second.ToString("00", NumberFormatInfo.InvariantInfo));
            if (_params.DateTimeMilliseconds)
            {
                _output.Append('.');
                _output.Append(dt.Millisecond.ToString("000", NumberFormatInfo.InvariantInfo));
            }
            if (_params.UseUTCDateTime)
                _output.Append('Z');

            _output.Append('\"');
        }

#if !SILVERLIGHT
        private DatasetSchema GetSchema(DataTable ds)
        {
            if (ds == null) return null;

            DatasetSchema m = new DatasetSchema();
            m.Info = new List<string>();
            m.Name = ds.TableName;

            foreach (DataColumn c in ds.Columns)
            {
                m.Info.Add(ds.TableName);
                m.Info.Add(c.ColumnName);
                m.Info.Add(c.DataType.ToString());
            }
            // FEATURE : serialize relations and constraints here

            return m;
        }

        private DatasetSchema GetSchema(DataSet ds)
        {
            if (ds == null) return null;

            DatasetSchema m = new DatasetSchema();
            m.Info = new List<string>();
            m.Name = ds.DataSetName;

            foreach (DataTable t in ds.Tables)
            {
                foreach (DataColumn c in t.Columns)
                {
                    m.Info.Add(t.TableName);
                    m.Info.Add(c.ColumnName);
                    m.Info.Add(c.DataType.ToString());
                }
            }
            // FEATURE : serialize relations and constraints here

            return m;
        }

        private string GetXmlSchema(DataTable dt)
        {
            using (var writer = new StringWriter())
            {
                dt.WriteXmlSchema(writer);
                return dt.ToString();
            }
        }

        private void WriteDataset(DataSet ds)
        {
            _output.Append('{');
            if (_params.UseExtensions)
            {
                WritePair("$schema", _params.UseOptimizedDatasetSchema ? (object)GetSchema(ds) : ds.GetXmlSchema());
                _output.Append(',');
            }
            bool tablesep = false;
            foreach (DataTable table in ds.Tables)
            {
                if (tablesep) _output.Append(',');
                tablesep = true;
                WriteDataTableData(table);
            }
            // end dataset
            _output.Append('}');
        }

        private void WriteDataTableData(DataTable table)
        {
            _output.Append('\"');
            _output.Append(table.TableName);
            _output.Append("\":[");
            DataColumnCollection cols = table.Columns;
            bool rowseparator = false;
            foreach (DataRow row in table.Rows)
            {
                if (rowseparator) _output.Append(',');
                rowseparator = true;
                _output.Append('[');

                bool pendingSeperator = false;
                foreach (DataColumn column in cols)
                {
                    if (pendingSeperator) _output.Append(',');
                    WriteValue(row[column]);
                    pendingSeperator = true;
                }
                _output.Append(']');
            }

            _output.Append(']');
        }

        void WriteDataTable(DataTable dt)
        {
            this._output.Append('{');
            if (_params.UseExtensions)
            {
                this.WritePair("$schema", _params.UseOptimizedDatasetSchema ? (object)this.GetSchema(dt) : this.GetXmlSchema(dt));
                this._output.Append(',');
            }

            WriteDataTableData(dt);

            // end datatable
            this._output.Append('}');
        }
#endif

        bool _TypesWritten = false;
        private void WriteObject(object obj)
        {
            int i = 0;
            if (_cirobj.TryGetValue(obj, out i) == false)
                _cirobj.Add(obj, _cirobj.Count + 1);
            else
            {
                if (_current_depth > 0 && _params.InlineCircularReferences == false)
                {
                    //_circular = true;
                    _output.Append("{\"$i\":");
                    _output.Append(i.ToString());
                    _output.Append("}");
                    return;
                }
            }
            if (_params.UsingGlobalTypes == false)
                _output.Append('{');
            else
            {
                if (_TypesWritten == false)
                {
                    _output.Append('{');
                    _before = _output.Length;
                    //_output = new StringBuilder();
                }
                else
                    _output.Append('{');
            }
            _TypesWritten = true;
            _current_depth++;
            if (_current_depth > _MAX_DEPTH)
                throw new Exception("Serializer encountered maximum depth of " + _MAX_DEPTH);


            Dictionary<string, string> map = new Dictionary<string, string>();
            Type t = obj.GetType();
            bool append = false;
            if (_params.UseExtensions)
            {
                if (_params.UsingGlobalTypes == false)
                    WritePairFast("$type", Reflection.Instance.GetTypeAssemblyName(t));
                else
                {
                    int dt = 0;
                    string ct = Reflection.Instance.GetTypeAssemblyName(t);
                    if (_globalTypes.TryGetValue(ct, out dt) == false)
                    {
                        dt = _globalTypes.Count + 1;
                        _globalTypes.Add(ct, dt);
                    }
                    WritePairFast("$type", dt.ToString());
                }
                append = true;
            }

            Getters[] g = Reflection.Instance.GetGetters(t, _params.ShowReadOnlyProperties, _params.IgnoreAttributes);
            int c = g.Length;
            for (int ii = 0; ii < c; ii++)
            {
                var p = g[ii];
                object o = p.Getter(obj);
                if (_params.SerializeNullValues == false && (o == null || o is DBNull))
                {
                    //append = false;
                }
                else
                {
                    if (append)
                        _output.Append(',');
                    if (_params.SerializeToLowerCaseNames)
                        WritePair(p.lcName, o);
                    else
                        WritePair(p.Name, o);
                    if (o != null && _params.UseExtensions)
                    {
                        Type tt = o.GetType();
                        if (tt == typeof(System.Object))
                            map.Add(p.Name, tt.ToString());
                    }
                    append = true;
                }
            }
            if (map.Count > 0 && _params.UseExtensions)
            {
                _output.Append(",\"$map\":");
                WriteStringDictionary(map);
            }
            _output.Append('}');
            _current_depth--;
        }

        private void WritePairFast(string name, string value)
        {
            if (_params.UseApiNamingStyle)
            {
                string newName = StringUtil.ToUnderlineStyle(name.TrimEnd('_'));
                WriteStringFast(newName);
            }
            else
            {
                WriteStringFast(name);
            }

            _output.Append(':');

            WriteStringFast(value);
        }

        private void WritePair(string name, object value)
        {
            if (_params.UseApiNamingStyle)
            {
                string newName = StringUtil.ToUnderlineStyle(name.TrimEnd('_'));
                WriteStringFast(newName);
            }
            else
            {
                WriteStringFast(name);
            }

            _output.Append(':');

            WriteValue(value);
        }

        private void WriteArray(IEnumerable array)
        {
            _output.Append('[');

            bool pendingSeperator = false;

            foreach (object obj in array)
            {
                if (pendingSeperator) _output.Append(',');

                WriteValue(obj);

                pendingSeperator = true;
            }
            _output.Append(']');
        }

        private void WriteStringDictionary(IDictionary dic)
        {
            _output.Append('{');

            bool pendingSeparator = false;

            foreach (DictionaryEntry entry in dic)
            {
                if (_params.SerializeNullValues == false && (entry.Value == null))
                {
                }
                else
                {
                    if (pendingSeparator) _output.Append(',');

                    string k = (string)entry.Key;
                    if (_params.SerializeToLowerCaseNames)
                        WritePair(k.ToLower(), entry.Value);
                    else
                        WritePair(k, entry.Value);
                    pendingSeparator = true;
                }
            }
            _output.Append('}');
        }

        private void WriteStringDictionary(IDictionary<string, object> dic)
        {
            _output.Append('{');
            bool pendingSeparator = false;
            foreach (KeyValuePair<string, object> entry in dic)
            {
                if (_params.SerializeNullValues == false && (entry.Value == null))
                {
                }
                else
                {
                    if (pendingSeparator) _output.Append(',');
                    string k = entry.Key;

                    if (_params.SerializeToLowerCaseNames)
                        WritePair(k.ToLower(), entry.Value);
                    else
                        WritePair(k, entry.Value);
                    pendingSeparator = true;
                }
            }
            _output.Append('}');
        }

        private void WriteDictionary(IDictionary dic)
        {
            _output.Append('[');

            bool pendingSeparator = false;

            foreach (DictionaryEntry entry in dic)
            {
                if (pendingSeparator) _output.Append(',');
                _output.Append('{');
                WritePair("k", entry.Key);
                _output.Append(",");
                WritePair("v", entry.Value);
                _output.Append('}');

                pendingSeparator = true;
            }
            _output.Append(']');
        }

        private void WriteStringFast(string s)
        {
            _output.Append('\"');
            _output.Append(s);
            _output.Append('\"');
        }

        private void WriteString(string s)
        {
            _output.Append('\"');

            int runIndex = -1;
            int l = s.Length;
            for (var index = 0; index < l; ++index)
            {
                var c = s[index];

                if (_useEscapedUnicode)
                {
                    if (c >= ' ' && c < 128 && c != '\"' && c != '\\')
                    {
                        if (runIndex == -1)
                            runIndex = index;

                        continue;
                    }
                }
                else
                {
                    if (c != '\t' && c != '\n' && c != '\r' && c != '\"' && c != '\\')// && c != ':' && c!=',')
                    {
                        if (runIndex == -1)
                            runIndex = index;

                        continue;
                    }
                }

                if (runIndex != -1)
                {
                    _output.Append(s, runIndex, index - runIndex);
                    runIndex = -1;
                }

                switch (c)
                {
                    case '\t': _output.Append("\\t"); break;
                    case '\r': _output.Append("\\r"); break;
                    case '\n': _output.Append("\\n"); break;
                    case '"':
                    case '\\': _output.Append('\\'); _output.Append(c); break;
                    default:
                        if (_useEscapedUnicode)
                        {
                            _output.Append("\\u");
                            _output.Append(((int)c).ToString("X4", NumberFormatInfo.InvariantInfo));
                        }
                        else
                            _output.Append(c);

                        break;
                }
            }

            if (runIndex != -1)
                _output.Append(s, runIndex, s.Length - runIndex);

            _output.Append('\"');
        }
    }
}
