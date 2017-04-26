using System;
using System.Collections.Generic;
using System.IO;
using System.Collections.Specialized;
using System.Globalization;
using System.Security;
using System.Security.Permissions;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace BaiRong.Core
{
	public class Serializer
	{
		//Do not allow this class to be instantiated
		private Serializer()
		{
		
		}

		/// <summary>
		/// Static Constructor is used to set the CanBinarySerialize value only once for the given security policy
		/// </summary>
		static Serializer()
		{
			var sp =  new SecurityPermission(SecurityPermissionFlag.SerializationFormatter);
			try
			{
				sp.Demand();
				CanBinarySerialize = true;
			}
			catch(SecurityException)
			{
				CanBinarySerialize = false;
			}
		}

		/// <summary>
		/// Readonly value indicating if Binary Serialization (using BinaryFormatter) is allowed
		/// </summary>
		public static readonly bool CanBinarySerialize;

		/// <summary>
		/// Converts a .NET object to a byte array. Before the conversion happens, a check with 
		/// Serializer.CanBinarySerialize will be made
		/// </summary>
		/// <param name="objectToConvert">Object to convert</param>
		/// <returns>A byte arry representing the object paramter. Null will be return if CanBinarySerialize is false</returns>
		public static byte[] ConvertToBytes(object objectToConvert)
		{
			byte[] byteArray = null;

			if(CanBinarySerialize)
			{
				var	binaryFormatter	= new BinaryFormatter();
				using(var ms	= new MemoryStream())
				{

					binaryFormatter.Serialize(ms, objectToConvert);
            

					// Set the position	of the MemoryStream	back to	0
					//
					ms.Position	= 0;
			
					// Read	in the byte	array
					//
					byteArray =	new	Byte[ms.Length];
					ms.Read(byteArray, 0, byteArray.Length);
					ms.Close();
				}
			}
			return byteArray;
		}

		/// <summary>
		/// Saves an object to disk as a binary file. 
		/// </summary>
		/// <param name="objectToSave">Object to Save</param>
		/// <param name="path">Location of the file</param>
		/// <returns>true if the save was succesful.</returns>
		public static bool SaveAsBinary(object objectToSave, string path)
		{
			if(objectToSave != null && CanBinarySerialize)
			{
				var ba = ConvertToBytes(objectToSave);
				if(ba != null)
				{
					using(var fs = new FileStream(path,FileMode.OpenOrCreate,FileAccess.Write))
					{
						using(var bw = new BinaryWriter(fs))
						{
							bw.Write(ba);
							return true;
						}
					}
				}
			}
			return false;
		}


		/// <summary>
		/// Converts a .NET object to a string of XML. The object must be marked as Serializable or an exception
		/// will be thrown.
		/// </summary>
		/// <param name="objectToConvert">Object to convert</param>
		/// <returns>A xml string represting the object parameter. The return value will be null of the object is null</returns>
		public static string ConvertToString(object objectToConvert)
		{
			string xml = null;
           
			if(objectToConvert != null)
			{
				//we need the type to serialize
				var t = objectToConvert.GetType();

				var ser = new XmlSerializer(t);
				//will hold the xml
				using(var writer = new StringWriter(CultureInfo.InvariantCulture))
				{
					ser.Serialize(writer, objectToConvert);
					xml =  writer.ToString();
					writer.Close();
				}
			}
            
			return xml;
		}

		public static void SaveAsXML(object objectToConvert, string path)
		{
			if(objectToConvert != null)
			{
				//we need the type to serialize
				var t = objectToConvert.GetType();

				var ser = new XmlSerializer(t);
				//will hold the xml
				using(var writer = new StreamWriter(path))
				{
					ser.Serialize(writer, objectToConvert);
					writer.Close();
				}
			}
		}


		/// <summary>
		/// Converts a byte array to a .NET object. You will need to cast this object back to its expected type. 
		/// If the array is null or empty, it will return null.
		/// </summary>
		/// <param name="byteArray">An array of bytes represeting a .NET object</param>
		/// <returns>The byte array converted to an object or null if the value of byteArray is null or empty</returns>
		public static object ConvertToObject(byte[] byteArray)
		{
			object convertedObject = null;
			if(CanBinarySerialize && byteArray != null && byteArray.Length > 0)
			{
				var	binaryFormatter	= new BinaryFormatter();
				using(var ms	= new MemoryStream())
				{
					ms.Write(byteArray, 0, byteArray.Length);

					// Set the memory stream position to the beginning of the stream
					//
					ms.Position	= 0;

					if( byteArray.Length > 4 )
						convertedObject = binaryFormatter.Deserialize(ms);

					ms.Close();
				}
			}
			return convertedObject;
		}

		public static object ConvertFileToObject(string path, Type objectType)
		{
			object convertedObject = null;

			if(path != null && path.Length > 0)
			{
				using(var fs = new FileStream(path,FileMode.Open,FileAccess.Read))
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

			if(!string.IsNullOrEmpty(xml))
			{
				using(var reader  = new StringReader(xml))
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

			if(node != null)
			{
				using(var reader  = new StringReader(node.OuterXml))
				{
                    
					var ser = new XmlSerializer(objectType);
                   
					convertedObject = ser.Deserialize(reader);
                    
					reader.Close();
				}
			}
			return convertedObject;
		}


		public static object LoadBinaryFile(string path)
		{
			if(!File.Exists(path))
				return null;

			using(var fs = new FileStream(path,FileMode.Open,FileAccess.Read))
			{
				var br =new BinaryReader(fs);
				var ba = new byte[fs.Length];
				br.Read(ba,0,(int)fs.Length);
				return ConvertToObject(ba);
			}
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
			var nvc  = new NameValueCollection();

			if(keys != null && values != null && keys.Length > 0 && values.Length > 0)
			{
				var splitter = new char[1] { ':' } ;
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
			if(nvc == null || nvc.Count == 0)
				return;

			var sbKey = new StringBuilder();
			var sbValue = new StringBuilder();
		
			var index = 0;
			foreach(var key in nvc.AllKeys)
			{
				if(key.IndexOf(':') != -1)
					throw new ArgumentException("ExtendedAttributes Key can not contain the character \":\"");

				var v = nvc[key];
				if(!string.IsNullOrEmpty(v))
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
