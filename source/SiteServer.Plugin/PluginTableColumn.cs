using Newtonsoft.Json;

namespace SiteServer.Plugin
{
    [JsonObject(MemberSerialization.OptOut)]
    public class PluginTableColumn
    {
        public string AttributeName { get; set; }

        public string DisplayName { get; set; }

        public DataType DataType { get; set; }

        public int DataLength { get; set; }

        public InputType InputType { get; set; }

        public string DefaultValue { get; set; }

        public bool IsRequired { get; set; }

        public int MinNum { get; set; }

        public int MaxNum { get; set; }

        public string RegExp { get; set; }

        public string Width { get; set; }
    }
}
