using System;
using System.Collections.Generic;
using System.Text;
using SSCMS.Enums;

namespace SSCMS.Models
{
    public class CensorResult
    {
        public CensorSuggestion Suggestion { get; set; }
        public float Rate { get; set; }
        public List<CensorItem> Items { get; set; }
    }
}
