using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace SSCMS.Core.Utils
{
	public static class XmlUtils
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

        public static void SaveAsXml(object objectToConvert, string path)
        {
            if (objectToConvert != null)
            {
                //we need the type to serialize
                var t = objectToConvert.GetType();

                var ser = new XmlSerializer(t);
                //will hold the xml
                using (var writer = new StreamWriter(path))
                {
                    ser.Serialize(writer, objectToConvert);
                    writer.Close();
                }
            }
        }

        public static T ConvertFileToObject<T>(string path) where T : class
        {
            if (string.IsNullOrEmpty(path)) return null;

            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            var ser = new XmlSerializer(typeof(T));
            var convertedObject = ser.Deserialize(fs);
            fs.Close();

            return convertedObject as T;
        }
	}
}
