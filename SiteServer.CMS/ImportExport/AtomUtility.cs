using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Atom.AdditionalElements;
using Atom.AdditionalElements.DublinCore;
using Atom.Core;
using SiteServer.Utils;
using SiteServer.Utils.Auth;

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

        public static void AddDcElement(ScopedElementCollection collection, List<string> nameList, string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                foreach (var name in nameList)
                {
                    collection.Add(new DcElement(Prefix + name, StringUtils.ToXmlContent(content)));
                }
            }
        }

        public static string GetDcElementContent(ScopedElementCollection additionalElements, string name)
        {
            return GetDcElementContent(additionalElements, name, "");
        }

        public static string GetDcElementContent(ScopedElementCollection additionalElements, List<string> nameList)
        {
            return GetDcElementContent(additionalElements, nameList, "");
        }

        public static string GetDcElementContent(ScopedElementCollection additionalElements, string name, string defaultContent)
        {
            var localName = Prefix + name;
            var element = additionalElements.FindScopedElementByLocalName(localName);
            return element != null ? element.Content : defaultContent;
        }

        public static string GetDcElementContent(ScopedElementCollection additionalElements, List<string> nameList, string defaultContent)
        {
            foreach (var name in nameList)
            {
                var localName = Prefix + name;
                var element = additionalElements.FindScopedElementByLocalName(localName);
                if (element == null) continue;

                return element.Content;
            }
            return defaultContent;
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

            var encryptor = new DesEncryptor
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

            var encryptor = new DesEncryptor
            {
                InputString = inputString,
                DecryptKey = "TgQQk42O"
            };
            encryptor.DesDecrypt();
            return encryptor.OutString;
        }
    }
}
