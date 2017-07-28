using System.Xml;

namespace BaiRong.Core
{
	public class XmlUtils
	{

		public static XmlDocument GetXmlDocument(string xmlContent)
		{
			var xmlDocument = new XmlDocument();
			try
			{
				xmlDocument.LoadXml(xmlContent);
			}
			catch{}

			return xmlDocument;
		}


		public static string GetXmlNodeInnerText(XmlDocument xmlDocument, string xpath)
		{
			var innerText = string.Empty;
			try
			{
				var node = xmlDocument.SelectSingleNode(xpath);
				if (node != null)
				{
					innerText = node.InnerText;
				}
			}
			catch {}
			return innerText;
		}

		public static XmlNode GetXmlNode(XmlDocument xmlDocument, string xpath)
		{
			XmlNode node = null;
			try
			{
				node = xmlDocument.SelectSingleNode(xpath);
			}
			catch {}
			return node;
		}

		public static string GetXmlNodeAttribute(XmlDocument xmlDocument, string xpath, string attributeName)
		{
			return GetXmlNodeAttribute(GetXmlNode(xmlDocument, xpath), attributeName);
		}

		public static string GetXmlNodeAttribute(XmlNode node, string attributeName)
		{
			var retval = string.Empty;
			try
			{
				var ie = node.Attributes.GetEnumerator();
				while (ie.MoveNext())
				{
					var attr = (XmlAttribute)ie.Current;
					if (attr.Name.ToLower().Equals(attributeName.ToLower()))
					{
						retval = attr.Value;
						break;
					}
				}
			}
			catch{}
			return retval;
		}

//        public static string HtmlConvertXml(string html)
//        {
//            if (string.IsNullOrEmpty(html.Trim()))
//            {
//                return string.Empty;
//            }
//            using (SgmlReader reader = new SgmlReader())
//            {
//                reader.DocType = "HTML";
//                reader.InputStream = new StringReader(html);
//                using (StringWriter stringWriter = new StringWriter())
//                {
//                    using (XmlTextWriter writer = new XmlTextWriter(stringWriter))
//                    {
//                        reader.WhitespaceHandling = WhitespaceHandling.None;
//                        writer.Formatting = Formatting.Indented;
//                        XmlDocument doc = new XmlDocument();
//                        doc.Load(reader);
//                        if (doc.DocumentElement == null)
//                        {
//                            return string.Empty;
//                        }
//                        else
//                        {
//                            doc.DocumentElement.WriteContentTo(writer);
//                        }
//                        writer.Close();
//                        string xhtml = stringWriter.ToString();
//                        return xhtml;
//                    }
//                }
//            }
//        }
	}
}
