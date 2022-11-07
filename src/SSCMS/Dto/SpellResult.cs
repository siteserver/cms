using System.Collections.Generic;

namespace SSCMS.Dto
{
    public class SpellResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsErrorWords { get; set; }
        public List<ErrorWord> ErrorWords { get; set; }
    }
}
