using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;

namespace SiteServer.Plugin
{
    /// <summary>
    /// 可扩展属性的对象接口。
    /// 系统以键/值对的方式存储数据，键不区分大小写。
    /// </summary>
    public interface IAttributes
    {
        /// <summary>
        /// 将 <see cref="T:System.Data.IDataReader" /> 中的数据导入到实例中。
        /// 如果现有数据和导入数据名称冲突，系统将使用导入的数据覆盖现有的数据。
        /// </summary>
        /// <param name="reader">需要导入的数据。</param>
        void Load(IDataReader reader);

        /// <summary>
        /// 将 <see cref="T:System.Data.IDataRecord" /> 中的数据导入到实例中。
        /// 如果现有数据和导入数据名称冲突，系统将使用导入的数据覆盖现有的数据。
        /// </summary>
        /// <param name="record">需要导入的数据。</param>
        void Load(IDataRecord record);

        /// <summary>
        /// 将 <see cref="T:System.Data.DataRowView" /> 中的数据导入到实例中。
        /// 如果现有数据和导入数据名称冲突，系统将使用导入的数据覆盖现有的数据。
        /// </summary>
        /// <param name="rowView">需要导入的数据。</param>
        void Load(DataRowView rowView);

        /// <summary>
        /// 将 <see cref="T:System.Data.DataRow" /> 中的数据导入到实例中。
        /// 如果现有数据和导入数据名称冲突，系统将使用导入的数据覆盖现有的数据。
        /// </summary>
        /// <param name="row">需要导入的数据。</param>
        void Load(DataRow row);

        /// <summary>
        /// 将 <see cref="T:System.Collections.Specialized.NameValueCollection" /> 中的数据导入到实例中。
        /// 如果现有数据和导入数据名称冲突，系统将使用导入的数据覆盖现有的数据。
        /// </summary>
        /// <param name="attributes">需要导入的数据。</param>
        void Load(NameValueCollection attributes);

        /// <summary>
        /// 将 <see cref="T:System.Collections.Generic.Dictionary" /> 中的数据导入到实例中。
        /// 如果现有数据和导入数据名称冲突，系统将使用导入的数据覆盖现有的数据。
        /// </summary>
        /// <param name="dict">需要导入的数据。</param>
        void Load(Dictionary<string, object> dict);

        /// <summary>
        /// 将JSON字符串转换为键值对后导入到实例中。
        /// 如果现有数据和导入数据名称冲突，系统将使用导入的数据覆盖现有的数据。
        /// </summary>
        /// <param name="json">需要导入的数据。</param>
        void Load(string json);

        /// <summary>
        /// 将匿名对象中的数据导入到实例中。
        /// 如果现有数据和导入数据名称冲突，系统将使用导入的数据覆盖现有的数据。
        /// </summary>
        /// <param name="anonymous">匿名对象。</param>
        void Load(object anonymous);

        /// <summary>
        /// 获取与指定键关联的值。
        /// </summary>
        /// <param name="key">要获取的值的键。</param>
        /// <returns>
        /// 如果找到指定键，则包含与该键相关的值；否则返回 null。
        /// </returns>
        object Get(string key);

        /// <summary>
        /// 获取与指定键关联的值。
        /// </summary>
        /// <param name="key">要获取的值的键。</param>
        /// <param name="defaultValue">如果未包含与该键相关的值，此方法将返回的默认值。</param>
        /// <returns>
        /// 如果找到指定键，则包含与该键相关的值；否则返回 defaultValue。
        /// </returns>
        string GetString(string key, string defaultValue = "");

        /// <summary>
        /// 获取与指定键关联的值。
        /// </summary>
        /// <param name="key">要获取的值的键。</param>
        /// <param name="defaultValue">如果未包含与该键相关的值，此方法将返回的默认值。</param>
        /// <returns>
        /// 如果找到指定键，则包含与该键相关的值；否则返回 defaultValue。
        /// </returns>
        bool GetBool(string key, bool defaultValue = false);

        /// <summary>
        /// 获取与指定键关联的值。
        /// </summary>
        /// <param name="key">要获取的值的键。</param>
        /// <param name="defaultValue">如果未包含与该键相关的值，此方法将返回的默认值。</param>
        /// <returns>
        /// 如果找到指定键，则包含与该键相关的值；否则返回 defaultValue。
        /// </returns>
        int GetInt(string key, int defaultValue = 0);

        /// <summary>
        /// 获取与指定键关联的值。
        /// </summary>
        /// <param name="key">要获取的值的键。</param>
        /// <param name="defaultValue">如果未包含与该键相关的值，此方法将返回的默认值。</param>
        /// <returns>
        /// 如果找到指定键，则包含与该键相关的值；否则返回 defaultValue。
        /// </returns>
        decimal GetDecimal(string key, decimal defaultValue = 0);

        /// <summary>
        /// 获取与指定键关联的值。
        /// </summary>
        /// <param name="key">要获取的值的键。</param>
        /// <param name="defaultValue">如果未包含与该键相关的值，此方法将返回的默认值。</param>
        /// <returns>
        /// 如果找到指定键，则包含与该键相关的值；否则返回 defaultValue。
        /// </returns>
        DateTime GetDateTime(string key, DateTime defaultValue);

        /// <summary>
        /// 将带有指定键的值从实例中移除。
        /// </summary>
        /// <param name="key">要移除的元素的键。</param>
        void Remove(string key);

        /// <summary>
        /// 将指定的键和值添加到实例中。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="value">值。</param>
        void Set(string key, object value);

        /// <summary>
        /// 确定是否实例包含指定键。
        /// </summary>
        /// <param name="key">要在实例中定位的键。</param>
        /// <returns>如果包含具有指定键的元素，则为true；否则为false。</returns>
        bool ContainsKey(string key);

        /// <summary>
        /// 将实例序列化为JSON字符串并返回
        /// </summary>
        /// <returns>JSON字符串</returns>
        string ToString();

        /// <summary>
        /// 将实例序列化为JSON字符串并返回
        /// </summary>
        /// <param name="excludeKeys">需要排除的键列表</param>
        /// <returns></returns>
        string ToString(List<string> excludeKeys);

        /// <summary>
        /// 返回表示键和值的集合 <see cref="T:System.Collections.Generic.Dictionary" />
        /// </summary>
        /// <returns>键和值的集合</returns>
        Dictionary<string, object> ToDictionary();
    }
}
