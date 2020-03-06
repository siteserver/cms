using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SS.CMS.Abstractions
{
    public static class YamlUtils
    {
        public static void ObjectToFile<T>(T objectToConvert, string path) where T : class
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            using var writer = new StreamWriter(path);
            serializer.Serialize(writer, objectToConvert);
            writer.Close();
        }

        public static T FileToObject<T>(string path) where T : class
        {
            var yaml = File.ReadAllText(path);
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            return deserializer.Deserialize<T>(yaml);
        }
    }
}