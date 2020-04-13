using SSCMS.Enums;

namespace SSCMS.Models
{
    public class TableStyleRule
    {
        public ValidateType Type { get; set; }
        public string Value { get; set; }
        public string Message { get; set; }
    }
}