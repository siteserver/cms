using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SiteServer.CMS.Core
{
    public class Serializer
    {


        /// <summary>
        /// Converts a .NET object to a string of XML. The object must be marked as Serializable or an exception
        /// will be thrown.
        /// </summary>
        /// <param name="objectToConvert">Object to convert</param>
        /// <returns>A xml string represting the object parameter. The return value will be null of the object is null</returns>
        public static string ConvertToString(object objectToConvert)
        {
            string xml = null;

            if (objectToConvert != null)
            {
                //we need the type to serialize
                var t = objectToConvert.GetType();

                var ser = new XmlSerializer(t);
                //will hold the xml
                using (var writer = new StringWriter(CultureInfo.InvariantCulture))
                {
                    ser.Serialize(writer, objectToConvert);
                    xml = writer.ToString();
                    writer.Close();
                }
            }

            return xml;
        }

        public static void SaveAsXML(object objectToConvert, string path)
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


        public static object ConvertFileToObject(string path, Type objectType)
        {
            object convertedObject = null;

            if (path != null && path.Length > 0)
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    var ser = new XmlSerializer(objectType);
                    convertedObject = ser.Deserialize(fs);
                    fs.Close();
                }
            }
            return convertedObject;
        }

        /// <summary>
        /// Converts a string of xml to the supplied object type. 
        /// </summary>
        /// <param name="xml">Xml representing a .NET object</param>
        /// <param name="objectType">The type of object which the xml represents</param>
        /// <returns>A instance of object or null if the value of xml is null or empty</returns>
        public static object ConvertToObject(string xml, Type objectType)
        {
            object convertedObject = null;

            if (!string.IsNullOrEmpty(xml))
            {
                using (var reader = new StringReader(xml))
                {
                    var ser = new XmlSerializer(objectType);
                    convertedObject = ser.Deserialize(reader);
                    reader.Close();
                }
            }
            return convertedObject;
        }

        /// <summary>
        /// Converts a string of xml to the supplied object type. 
        /// </summary>
        /// <param name="xml">Xml representing a .NET object</param>
        /// <param name="objectType">The type of object which the xml represents</param>
        /// <returns>A instance of object or null if the value of xml is null or empty</returns>
        public static object ConvertToObject(XmlNode node, Type objectType)
        {
            object convertedObject = null;

            if (node != null)
            {
                using (var reader = new StringReader(node.OuterXml))
                {

                    var ser = new XmlSerializer(objectType);

                    convertedObject = ser.Deserialize(reader);

                    reader.Close();
                }
            }
            return convertedObject;
        }

        /// <summary>
        /// Creates a NameValueCollection from two string. The first contains the key pattern and the second contains the values
        /// spaced according to the kys
        /// </summary>
        /// <param name="keys">Keys for the namevalue collection</param>
        /// <param name="values">Values for the namevalue collection</param>
        /// <returns>A NVC populated based on the keys and vaules</returns>
        /// <example>
        /// string keys = "key1:S:0:3:key2:S:3:2:";
        /// string values = "12345";
        /// This would result in a NameValueCollection with two keys (Key1 and Key2) with the values 123 and 45
        /// </example>
        public static NameValueCollection ConvertToNameValueCollection(string keys, string values)
        {
            var nvc = new NameValueCollection();

            if (keys != null && values != null && keys.Length > 0 && values.Length > 0)
            {
                var splitter = new char[1] { ':' };
                var keyNames = keys.Split(splitter);

                for (var i = 0; i < (keyNames.Length / 4); i++)
                {
                    var start = int.Parse(keyNames[(i * 4) + 2], CultureInfo.InvariantCulture);
                    var len = int.Parse(keyNames[(i * 4) + 3], CultureInfo.InvariantCulture);
                    var key = keyNames[i * 4];

                    //Future version will support more complex types	
                    if (((keyNames[(i * 4) + 1] == "S") && (start >= 0)) && (len > 0) && (values.Length >= (start + len)))
                    {
                        nvc[key] = values.Substring(start, len);
                    }
                }
            }

            return nvc;
        }

        public static Dictionary<string, string> ConvertToDictionary(string keys, string values)
        {
            var nvc = new Dictionary<string, string>();

            if (keys != null && values != null && keys.Length > 0 && values.Length > 0)
            {
                var splitter = new char[1] { ':' };
                var keyNames = keys.Split(splitter);

                for (var i = 0; i < (keyNames.Length / 4); i++)
                {
                    var start = int.Parse(keyNames[(i * 4) + 2], CultureInfo.InvariantCulture);
                    var len = int.Parse(keyNames[(i * 4) + 3], CultureInfo.InvariantCulture);
                    var key = keyNames[i * 4];

                    //Future version will support more complex types	
                    if (((keyNames[(i * 4) + 1] == "S") && (start >= 0)) && (len > 0) && (values.Length >= (start + len)))
                    {
                        nvc[key] = values.Substring(start, len);
                    }
                }
            }

            return nvc;
        }

        /// <summary>
        /// Creates a the keys and values strings for the simple serialization based on a NameValueCollection
        /// </summary>
        /// <param name="nvc">NameValueCollection to convert</param>
        /// <param name="keys">the ref string will contain the keys based on the key format</param>
        /// <param name="values">the ref string will contain all the values of the namevaluecollection</param>
        public static void ConvertFromNameValueCollection(NameValueCollection nvc, ref string keys, ref string values)
        {
            if (nvc == null || nvc.Count == 0)
                return;

            var sbKey = new StringBuilder();
            var sbValue = new StringBuilder();

            var index = 0;
            foreach (var key in nvc.AllKeys)
            {
                if (key.IndexOf(':') != -1)
                    throw new ArgumentException("ExtendedAttributes Key can not contain the character \":\"");

                var v = nvc[key];
                if (!string.IsNullOrEmpty(v))
                {
                    sbKey.Append($"{key}:S:{index}:{v.Length}:");
                    sbValue.Append(v);
                    index += v.Length;
                }
            }
            keys = sbKey.ToString();
            values = sbValue.ToString();
        }

        public static void ConvertFromDictionary(Dictionary<string, string> nvc, ref string keys, ref string values)
        {
            if (nvc == null || nvc.Count == 0)
                return;

            var sbKey = new StringBuilder();
            var sbValue = new StringBuilder();

            var index = 0;
            foreach (var key in nvc.Keys)
            {
                if (key.IndexOf(':') != -1)
                    throw new ArgumentException("ExtendedAttributes Key can not contain the character \":\"");

                var v = nvc[key];
                if (!string.IsNullOrEmpty(v))
                {
                    sbKey.Append($"{key}:S:{index}:{v.Length}:");
                    sbValue.Append(v);
                    index += v.Length;
                }
            }
            keys = sbKey.ToString();
            values = sbValue.ToString();
        }
    }
}
