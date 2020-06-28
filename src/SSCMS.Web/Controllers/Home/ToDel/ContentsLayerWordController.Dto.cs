using System.Collections.Generic;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Home.ToDel
{
    public partial class ContentsLayerWordController
    {
        public class GetResult
        {
            public List<KeyValuePair<int, string>> CheckedLevels { set; get; }
            public int CheckedLevel { set; get; }
        }

        public class NameTitle
        {
            public string FileName { get; set; }
            public string Title { get; set; }
        }

        public class SubmitRequest : ChannelRequest
        {
            public bool IsFirstLineTitle { get; set; }
            public bool IsClearFormat { get; set; }
            public bool IsFirstLineIndent { get; set; }
            public bool IsClearFontSize { get; set; }
            public bool IsClearFontFamily { get; set; }
            public bool IsClearImages { get; set; }
            public int CheckedLevel { get; set; }
            public List<NameTitle> Files { get; set; }
        }
    }
}
