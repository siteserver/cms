using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Serialization;

namespace Top.Api.Parser
{
    /// <summary>
    /// TOP XML响应通用解释器。
    /// </summary>
    public class TopXmlParser<T> : ITopParser<T> where T : TopResponse
    {
        private static readonly Regex regex = new Regex("<(\\w+?)[ >]", RegexOptions.Compiled);
        private static readonly ReaderWriterLock rwLock = new ReaderWriterLock();
        private static readonly Dictionary<string, XmlSerializer> parsers = new Dictionary<string, XmlSerializer>();

        #region ITopParser Members

        public T Parse(string body, Type type)
        {
            return DoParse(body, type);
        }

        public T Parse(string body)
        {
            Type type = typeof(T);
            return DoParse(body, type);
        }

        #endregion

        private T DoParse(string body, Type type)
        {
            string rootTagName = GetRootElement(body);

            string key = type.FullName;
            if (Constants.ERROR_RESPONSE.Equals(rootTagName))
            {
                key += ("_" + Constants.ERROR_RESPONSE);
            }

            XmlSerializer serializer = null;
            bool incl = false;

            rwLock.AcquireReaderLock(50);
            try
            {
                if (rwLock.IsReaderLockHeld)
                {
                    incl = parsers.TryGetValue(key, out serializer);
                }
            }
            finally
            {
                if (rwLock.IsReaderLockHeld)
                {
                    rwLock.ReleaseReaderLock();
                }
            }

            if (!incl || serializer == null)
            {
                XmlAttributes rootAttrs = new XmlAttributes();
                rootAttrs.XmlRoot = new XmlRootAttribute(rootTagName);

                XmlAttributeOverrides attrOvrs = new XmlAttributeOverrides();
                attrOvrs.Add(type, rootAttrs);

                serializer = new XmlSerializer(type, attrOvrs);

                rwLock.AcquireWriterLock(50);
                try
                {
                    if (rwLock.IsWriterLockHeld)
                    {
                        parsers[key] = serializer;
                    }
                }
                finally
                {
                    if (rwLock.IsWriterLockHeld)
                    {
                        rwLock.ReleaseWriterLock();
                    }
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
