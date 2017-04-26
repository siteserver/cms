using FastJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Xml.Serialization;

namespace Top.Api.Parser
{
    /// <summary>
    /// TOP JSON响应通用解释器。
    /// </summary>
    public class TopJsonParser<T> : ITopParser<T> where T : TopResponse
    {
        private static readonly ReaderWriterLock rwLock = new ReaderWriterLock();
        private static readonly Dictionary<string, Dictionary<string, TopAttribute>> attrs = new Dictionary<string, Dictionary<string, TopAttribute>>();

        #region ITopParser Members

        public virtual T Parse(string body)
        {
            return Parse(body, typeof(T));
        }

        public T Parse(string body, Type type)
        {
            T rsp = null;

            IDictionary json = JSON.Parse(body) as IDictionary;
            if (json != null)
            {
                IDictionary data = null;

                // 忽略根节点的名称
                foreach (object key in json.Keys)
                {
                    data = json[key] as IDictionary;
                    break;
                }

                if (data != null)
                {
                    ITopReader reader = new TopJsonReader(data);
                    rsp = (T)FromJson(reader, type);
                }
            }

            if (rsp == null)
            {
                rsp = Activator.CreateInstance(type) as T;
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
            bool inc = false;

            rwLock.AcquireReaderLock(50);
            try
            {
                if (rwLock.IsReaderLockHeld)
                {
                    inc = attrs.TryGetValue(type.FullName, out tas);
                }
            }
            finally
            {
                if (rwLock.IsReaderLockHeld)
                {
                    rwLock.ReleaseReaderLock();
                }
            }

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

            rwLock.AcquireWriterLock(50);
            try
            {
                if (rwLock.IsWriterLockHeld)
                {
                    attrs[type.FullName] = tas;
                }
            }
            finally
            {
                if (rwLock.IsWriterLockHeld)
                {
                    rwLock.ReleaseWriterLock();
                }
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
                                value = JSON.ToJSON(tmp);
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
