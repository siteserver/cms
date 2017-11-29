using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;

namespace SiteServer.Plugin.Models
{
    public interface IAttributes
    {
        void Load(object dataItem);

        void Load(IDataReader rdr);

        void Load(NameValueCollection attributes);

        void Load(string str);

        string GetString(string name, string defaultValue);

        string GetString(string name);

        bool GetBool(string name, bool defaultValue = false);

        int GetInt(string name, int defaultValue = 0);

        decimal GetDecimal(string name, decimal defaultValue = 0);

        DateTime GetDateTime(string name, DateTime defaultValue);

        void Remove(string name);

        void Set(string name, string value);

        bool ContainsKey(string name);

        string ToString();

        string ToString(List<string> lowerCaseExcludeAttributeNames);

        NameValueCollection ToNameValueCollection();
    }
}
