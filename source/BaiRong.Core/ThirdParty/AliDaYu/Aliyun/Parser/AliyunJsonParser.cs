using FastJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;
using Top.Api.Parser;

namespace Aliyun.Api.Parser
{
    /// <summary>
    /// TOP JSON响应通用解释器。
    /// </summary>
    public class AliyunJsonParser : IAliyunParser
    {
        private static readonly object writeLock = new object();
        private static readonly Dictionary<string, Dictionary<string, TopAttribute>> attrs = new Dictionary<string, Dictionary<string, TopAttribute>>();

        #region ITopParser Members

        public T Parse<T>(string body) where T : AliyunResponse
        {
            T rsp = null;

            IDictionary json = JSON.Parse(body) as IDictionary;
            if (json != null)
            {
                ITopReader reader = new TopJsonReader(json);
                rsp = (T)FromJson(reader, typeof(T));
            }

            if (rsp == null)
            {
                rsp = Activator.CreateInstance<T>();
            }

            if (rsp != null)
            {
                rsp.Body = body;
            }

            return rsp;
        }

        #endregion

        private Dictionary<string, TopAttribute> GetTopAttributes(Type type)
        {
            Dictionary<string, TopAttribute> tas = null;
            bool inc = attrs.TryGetValue(type.FullName, out tas);

            if (inc && tas != null) // 从缓存中获取类属性元数据
            {
                return tas;
            }
            else // 创建新的类属性元数据缓存
            {
                tas = new Dictionary<string, TopAttribute>();
            }

            PropertyInfo[] pis = type.GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                TopAttribute ta = new TopAttribute();
                ta.Method = pi.GetSetMethod();

                // 获取对象属性名称
                XmlElementAttribute[] xeas = pi.GetCustomAttributes(typeof(XmlElementAttribute), true) as XmlElementAttribute[];
                if (xeas != null && xeas.Length > 0)
                {
                    ta.ItemName = xeas[0].ElementName;
                }

                // 获取列表属性名称
                if (ta.ItemName == null)
                {
                    XmlArrayItemAttribute[] xaias = pi.GetCustomAttributes(typeof(XmlArrayItemAttribute), true) as XmlArrayItemAttribute[];
                    if (xaias != null && xaias.Length > 0)
                    {
                        ta.ItemName = xaias[0].ElementName;
                    }
                    XmlArrayAttribute[] xaas = pi.GetCustomAttributes(typeof(XmlArrayAttribute), true) as XmlArrayAttribute[];
                    if (xaas != null && xaas.Length > 0)
                    {
                        ta.ListName = xaas[0].ElementName;
                    }
                    if (ta.ListName == null)
                    {
                        continue;
                    }
                }

                // 获取属性类型
                if (pi.PropertyType.IsGenericType)
                {
                    Type[] types = pi.PropertyType.GetGenericArguments();
                    ta.ListType = types[0];
                }
                else
                {
                    ta.ItemType = pi.PropertyType;
                }

                tas.Add(pi.Name, ta);
            }

            lock (writeLock)
            {
                attrs[type.FullName] = tas;
            }
            return tas;
        }

        public object FromJson(ITopReader reader, Type type)
        {
            object rsp = null;
            Dictionary<string, TopAttribute> pas = GetTopAttributes(type);

            Dictionary<string, TopAttribute>.Enumerator em = pas.GetEnumerator();
            while (em.MoveNext())
            {
                KeyValuePair<string, TopAttribute> kvp = em.Current;
                TopAttribute ta = kvp.Value;
                string itemName = ta.ItemName;
                string listName = ta.ListName;

                if (!reader.HasReturnField(itemName) && (string.IsNullOrEmpty(listName) || !reader.HasReturnField(listName)))
                {
                    continue;
                }

                object value = null;
                if (ta.ListType != null)
                {
                    value = reader.GetListObjects(listName, itemName, ta.ListType, FromJson);
                }
                else
                {
                    if (typeof(string) == ta.ItemType)
                    {
                        object tmp = reader.GetPrimitiveObject(itemName);
                        if (typeof(string) == tmp.GetType())
                        {
                            value = tmp;
                        }
                        else
                        {
                            if (tmp != null)
                            {
                                value = tmp.ToString();
                            }
                        }
                    }
                    else if (typeof(long) == ta.ItemType)
                    {
                        object tmp = reader.GetPrimitiveObject(itemName);
                        if (typeof(long) == tmp.GetType())
                        {
                            value = tmp;
                        }
                        else
                        {
                            if (tmp != null)
                            {
                                value = long.Parse(tmp.ToString());
                            }
                        }
                    }
                    else if (typeof(bool) == ta.ItemType)
                    {
                        object tmp = reader.GetPrimitiveObject(itemName);
                        if (typeof(bool) == tmp.GetType())
                        {
                            value = tmp;
                        }
                        else
                        {
                            if (tmp != null)
                            {
                                value = bool.Parse(tmp.ToString());
                            }
                        }
                    }
                    else
                    {
                        value = reader.GetReferenceObject(itemName, ta.ItemType, FromJson);
                    }
                }

                if (value != null)
                {
                    if (rsp == null)
                    {
                        rsp = Activator.CreateInstance(type);
                    }
                    ta.Method.Invoke(rsp, new object[] { value });
                }
            }

            return rsp;
        }
    }
}
