using System.Collections.Generic;
using SSCMS.Enums;

namespace SSCMS.Dto
{
    public class BadWord
    {
        public BadWordsType Type { get; set; }
        public string Message { get; set; }
        public List<string> Words { get; set; }
    }
}
