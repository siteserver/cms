using Qimen.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace Top.Api.Parser
{
    /// <summary>
    /// 奇门XML响应通用解释器。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QimenXmlParser<T> : ITopParser<T> where T : QimenResponse
    {
        private static readonly ReaderWriterLock rwLock = new ReaderWriterLock();
        private static readonly Dictionary<string, XmlSerializer> parsers = new Dictionary<string, XmlSerializer>();

        public T Parse(string body)
        {
            Type type = typeof(T);
            return Parse(body, type);
        }

        public T Parse(string body, Type type)
        {
            string key = type.FullName;

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
                rootAttrs.XmlRoot = new XmlRootAttribute(Constants.QM_ROOT_TAG_RSP);

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
    }
}
