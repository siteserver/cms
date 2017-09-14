using Newtonsoft.Json;

namespace SiteServer.Plugin.Models
{
    [JsonObject(MemberSerialization.OptOut)]
    public class PluginTableColumn
    {
        public string AttributeName { get; set; }

        public string DisplayName { get; set; }

        public DataType DataType { get; set; } = DataType.NVarChar;

        public int DataLength { get; set; } = 50;

        public InputType InputType { get; set; } = InputType.Text;

        public string DefaultValue { get; set; }

        public bool IsRequired { get; set; }

        public ValidateType ValidateType { get; set; } = ValidateType.None;

        public int MinNum { get; set; }

        public int MaxNum { get; set; }

        public string RegExp { get; set; }

        public string Width { get; set; }

        public bool IsVisibleInEdit { get; set; } = true;

        public bool IsVisibleInList { get; set; } = false;
    }
}
