using System;
using System.Collections.Specialized;
using Atom.AdditionalElements;
using Atom.AdditionalElements.DublinCore;
using Atom.Core;
using BaiRong.Core;
using BaiRong.Core.Cryptography;

namespace SiteServer.CMS.ImportExport
{
    /// <summary>
    /// Atom 0.3
    /// </summary>
    internal class AtomUtility
    {
        public const string Prefix = "SiteServer_";

        public static void AddDcElement(ScopedElementCollection collection, string name, string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                collection.Add(new DcElement(Prefix + name, StringUtils.ToXmlContent(content)));
            }
        }

        public static string GetDcElementContent(ScopedElementCollection additionalElements, string name)
        {
            return GetDcElementContent(additionalElements, name, "");
        }

        public static string GetDcElementContent(ScopedElementCollection additionalElements, string name, string defaultContent)
        {
            var content = defaultContent;
            var localName = Prefix + name;
            var element = additionalElements.FindScopedElementByLocalName(localName);
            if (element != null)
            {
                content = element.Content;
            }
            return content;
        }

        public static NameValueCollection GetDcElementNameValueCollection(ScopedElementCollection additionalElements)
        {
            return additionalElements.GetNameValueCollection(Prefix);
        }

        public static AtomFeed GetEmptyFeed()
        {
            var feed = new AtomFeed
            {
                Title = new AtomContentConstruct("title", "siteserver channel"),
                Author = new AtomPersonConstruct("author",
                    "siteserver", new Uri("http://www.siteserver.cn")),
                Modified = new AtomDateConstruct("modified", DateTime.Now,
                    TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now))
            };

            return feed;
        }

        public static AtomEntry GetEmptyEntry()
        {
            var entry = new AtomEntry
            {
                Id = new Uri("http://www.siteserver.cn/"),
                Title = new AtomContentConstruct("title", "title"),
                Modified = new AtomDateConstruct("modified", DateTime.Now,
                    TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now)),
                Issued = new AtomDateConstruct("issued", DateTime.Now,
                    TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now))
            };

            return entry;
        }

        public static string Encrypt(string inputString)
        {
            if (string.IsNullOrEmpty(inputString)) return string.Empty;

            var encryptor = new DESEncryptor
            {
                InputString = inputString,
                EncryptKey = "TgQQk42O"
            };
            encryptor.DesEncrypt();
            return encryptor.OutString;
        }


        public static string Decrypt(string inputString)
        {
            if (string.IsNullOrEmpty(inputString)) return string.Empty;

            var encryptor = new DESEncryptor
            {
                InputString = inputString,
                DecryptKey = "TgQQk42O"
            };
            encryptor.DesDecrypt();
            return encryptor.OutString;
        }
    }
}
