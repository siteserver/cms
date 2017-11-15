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
		    catch
		    {
		        // ignored
		    }

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
		    catch
		    {
		        // ignored
		    }
		    return innerText;
		}

		public static XmlNode GetXmlNode(XmlDocument xmlDocument, string xpath)
		{
			XmlNode node = null;
			try
			{
				node = xmlDocument.SelectSingleNode(xpath);
			}
		    catch
		    {
		        // ignored
		    }
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
			    if (node.Attributes != null)
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
			}
		    catch
		    {
		        // ignored
		    }
		    return retval;
		}
	}
}
