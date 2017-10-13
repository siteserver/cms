using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;
using Jayrock.Json.Conversion;
using Aop.Api.Request;
using Aop.Api.Util;

namespace Aop.Api.Parser
{
    /// <summary>
    /// AOP JSON响应通用解释器。
    /// </summary>
    public class AopJsonParser<T> : IAopParser<T> where T : AopResponse
    {
        private static readonly Dictionary<string, Dictionary<string, AopAttribute>> attrs = new Dictionary<string, Dictionary<string, AopAttribute>>();

        #region IAopParser<T> Members
        public T Parse(string body,string charset)
        {
            T rsp = null;

            IDictionary json = JsonConvert.Import(body) as IDictionary;
            if (json != null)
            {
                IDictionary data = null;

                // 忽略根节点的名称
                foreach (object key in json.Keys)
                {
                    data = json[key] as IDictionary;
                    if (data != null && data.Count > 0)
                    {
                        break;
                    }
                }

                if (data != null)
                {
                    IAopReader reader = new AopJsonReader(data);
                    rsp = (T)AopJsonConvert(reader, typeof(T));
                }
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


        public SignItem GetSignItem(IAopRequest<T> request, string responseBody)
        {
            if (string.IsNullOrEmpty(responseBody))
            {
                return null;
            }

            SignItem signItem = new SignItem();
            string sign = GetSign(responseBody);
            signItem.Sign = sign;

            string signSourceData = GetSignSourceData(request, responseBody);
            signItem.SignSourceDate = signSourceData;

            return signItem;
        }

        #endregion

        private static Dictionary<string, AopAttribute> GetAopAttributes(Type type)
        {
            Dictionary<string, AopAttribute> tas = null;
            bool inc = attrs.TryGetValue(type.FullName, out tas);

            if (inc && tas != null) // 从缓存中获取类属性元数据
            {
                return tas;
            }
            else // 创建新的类属性元数据缓存
            {
                tas = new Dictionary<string, AopAttribute>();
            }

            PropertyInfo[] pis = type.GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                AopAttribute ta = new AopAttribute();
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

                tas.Add(pi.Name + ta.ItemType + ta.ListType, ta);
            }

            attrs[type.FullName] = tas;
            return tas;
        }

        protected static readonly DAopConvert AopJsonConvert = delegate(IAopReader reader, Type type)
        {
            object rsp = null;
            Dictionary<string, AopAttribute> pas = GetAopAttributes(type);

            Dictionary<string, AopAttribute>.Enumerator em = pas.GetEnumerator();
            while (em.MoveNext())
            {
                KeyValuePair<string, AopAttribute> kvp = em.Current;
                AopAttribute ta = kvp.Value;
                string itemName = ta.ItemName;
                string listName = ta.ListName;

                if (!reader.HasReturnField(itemName) && (string.IsNullOrEmpty(listName) || !reader.HasReturnField(listName)))
                {
                    continue;
                }

                object value = null;
                if (ta.ListType != null)
                {
                    value = reader.GetListObjects(listName, itemName, ta.ListType, AopJsonConvert);
                }
                else
                {
                    if (typeof(string) == ta.ItemType)
                    {
                        object tmp = reader.GetPrimitiveObject(itemName);
                        if (tmp != null)
                        {
                            value = tmp.ToString();
                        }
                    }
                    else if (typeof(long) == ta.ItemType)
                    {
                        object tmp = reader.GetPrimitiveObject(itemName);
                        if (tmp != null)
                        {
                            value = ((IConvertible)tmp).ToInt64(null);
                        }
                    }
                    else if (typeof(bool) == ta.ItemType)
                    {
                        value = reader.GetPrimitiveObject(itemName);
                    }
                    else
                    {
                        value = reader.GetReferenceObject(itemName, ta.ItemType, AopJsonConvert);
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
        };

        private static string GetSign(string body)
        {
            IDictionary json = JsonConvert.Import(body) as IDictionary;
            Console.WriteLine(json);
            return (string)json["sign"];
        }

        private static string GetSignSourceData(IAopRequest<T> request, string body)
        {
            string rootNode = request.GetApiName().Replace(".", "_") + AlipayConstants.RESPONSE_SUFFIX;
            string errorRootNode = AlipayConstants.ERROR_RESPONSE;

            int indexOfRootNode = body.IndexOf(rootNode);
            int indexOfErrorRoot = body.IndexOf(errorRootNode);

            string result = null;
            if (indexOfRootNode > 0)
            {
                result = ParseSignSourceData(body, rootNode, indexOfRootNode);
            }
            else if (indexOfErrorRoot > 0)
            {
                result = ParseSignSourceData(body, errorRootNode, indexOfErrorRoot);
            }

            return result;
        }

        private static string ParseSignSourceData(string body, string rootNode, int indexOfRootNode)
        {
            int signDataStartIndex = indexOfRootNode + rootNode.Length + 2;
            int indexOfSign = body.IndexOf("\"" + AlipayConstants.SIGN + "\"");
            if (indexOfSign < 0)
            {
                return null;
            }

            int signDataEndIndex = indexOfSign - 1;
            int length = signDataEndIndex - signDataStartIndex;

            return body.Substring(signDataStartIndex, length);
        }


        public string EncryptSourceData(IAopRequest<T> request, string body, string encryptType, string encryptKey, string charset)
        {

            if (!"AES".Equals(encryptType))
            {
                throw new AopException("API only support AES!");
            }

            EncryptParseItem item = parseEncryptData(request, body);

            string bodyIndexContent = body.Substring(0, item.startIndex);
            string bodyEndexContent = body.Substring(item.endIndex);

            //TODO 解密逻辑
            string bizContent = AlipayEncrypt.AesDencrypt(encryptKey, item.encryptContent, charset);

            return bodyIndexContent + bizContent + bodyEndexContent;
        }



        /// <summary>
        /// 解析加密节点
        /// </summary>
        /// <param name="request"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        private static EncryptParseItem parseEncryptData(IAopRequest<T> request, string body)
        {
            string rootNode = request.GetApiName().Replace(".", "_") + AlipayConstants.RESPONSE_SUFFIX;
            string errorRootNode = AlipayConstants.ERROR_RESPONSE;

            int indexOfRootNode = body.IndexOf(rootNode);
            int indexOfErrorRoot = body.IndexOf(errorRootNode);

            EncryptParseItem result = null;
            if (indexOfRootNode > 0)
            {
                result = ParseEncryptItem(body, rootNode, indexOfRootNode);
            }
            else if (indexOfErrorRoot > 0)
            {
                result = ParseEncryptItem(body, errorRootNode, indexOfErrorRoot);
            }

            return result;
        }

        private static EncryptParseItem ParseEncryptItem(string body, string rootNode, int indexOfRootNode)
        {
            int signDataStartIndex = indexOfRootNode + rootNode.Length + 2;
            int indexOfSign = body.IndexOf("\"" + AlipayConstants.SIGN + "\"");

            int signDataEndIndex = indexOfSign - 1;

            if (signDataEndIndex < 0)
            {
                signDataEndIndex = body.Length - 1;
            }

            int length = signDataEndIndex - signDataStartIndex;

            string encyptContent = body.Substring(signDataStartIndex + 1, length - 2);

            EncryptParseItem item = new EncryptParseItem();
            item.encryptContent = encyptContent;
            item.startIndex = signDataStartIndex;
            item.endIndex = signDataEndIndex;


            return item;
        }
    }
}
