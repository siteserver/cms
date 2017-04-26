using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Top.Api;

namespace Aliyun.Api.Parser
{
    /// <summary>
    /// TOP XML响应通用解释器。
    /// </summary>
    public class AliyunXmlParser : IAliyunParser
    {
        private static readonly Regex regex = new Regex("<(\\w+?)[ >]", RegexOptions.Compiled);
        private static readonly object writeLock = new object();
        private static readonly Dictionary<string, XmlSerializer> parsers = new Dictionary<string, XmlSerializer>();

        #region ITopParser Members

        public T Parse<T>(string body) where T : AliyunResponse
        {
            Type type = typeof(T);
            string rootTagName = GetRootElement(body);

            string key = type.FullName;
            if (Constants.ERROR_RESPONSE.Equals(rootTagName))
            {
                key += ("_" + Constants.ERROR_RESPONSE);
            }

            XmlSerializer serializer = null;
            bool incl = parsers.TryGetValue(key, out serializer);
            if (!incl || serializer == null)
            {
                XmlAttributes rootAttrs = new XmlAttributes();
                rootAttrs.XmlRoot = new XmlRootAttribute(rootTagName);

                XmlAttributeOverrides attrOvrs = new XmlAttributeOverrides();
                attrOvrs.Add(type, rootAttrs);

                serializer = new XmlSerializer(type, attrOvrs);
                lock (writeLock)
                {
                    parsers[key] = serializer;
                }
            }

            object obj = null;
            using (System.IO.Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(body)))
            {
                obj = serializer.Deserialize(stream);
            }

            T rsp = (T)obj;
            if (rsp != null)
            {
                rsp.Body = body;
            }
            return rsp;
        }

        #endregion

        /// <summary>
        /// 获取XML响应的根节点名称
        /// </summary>
        private string GetRootElement(string body)
        {
            Match match = regex.Match(body);
            if (match.Success)
            {
                return match.Groups[1].ToString();
            }
            else
            {
                throw new TopException("Invalid XML response format!");
            }
        }
    }
}
