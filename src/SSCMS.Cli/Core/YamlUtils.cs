using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SSCMS.Cli.Core
{
    public static class YamlUtils
    {
        public static T Deserialize<T>(string yaml)
        {
            try
            {
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();

                return deserializer.Deserialize<T>(yaml);
            }
            catch
            {
                // ignored
            }

            return default;
        }

        public static string Serialize<T>(T objectToSerialize)
        {
            try
            {
                var serializer = new SerializerBuilder().Build();
                return serializer.Serialize(objectToSerialize);
            }
            catch
            {
                // ignored
            }

            return string.Empty;
        }
    }
}
