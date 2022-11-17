using System.Collections.Generic;

namespace SSCMS.Dto
{
    public class CensorResult : CloudResult
    {
        public bool IsBadWords { get; set; }
        public List<BadWord> BadWords { get; set; }
    }
}
