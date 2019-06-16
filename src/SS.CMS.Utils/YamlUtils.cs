using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SS.CMS.Utils
{
    public static class YamlUtils
    {
        public static void ObjectToFile<T>(T objectToConvert, string path) where T : class
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .Build();
            using (var writer = new StreamWriter(path))
            {
                serializer.Serialize(writer, objectToConvert);
                writer.Close();
            }
        }

        public static T FileToObject<T>(string path) where T : class
        {
            var yaml = File.ReadAllText(path);
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .Build();
            return deserializer.Deserialize<T>(yaml);
        }
    }
}
