using System.Collections.Generic;

namespace SSCMS.Dto
{
    public class CensorResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsBadWords { get; set; }
        public List<BadWord> BadWords { get; set; }
    }
}
