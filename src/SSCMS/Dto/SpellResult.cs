using System.Collections.Generic;

namespace SSCMS.Dto
{
    public class SpellResult : CloudResult
    {
        public bool IsErrorWords { get; set; }
        public List<ErrorWord> ErrorWords { get; set; }
    }
}
