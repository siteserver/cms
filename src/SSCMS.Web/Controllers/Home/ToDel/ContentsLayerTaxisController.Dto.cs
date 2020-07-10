using System.Collections.Generic;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Home.ToDel
{
    public partial class ContentsLayerTaxisController
    {
        public class SubmitRequest : ChannelRequest
        {
            public List<int> ContentIds { get; set; }
            public bool IsUp { get; set; }
            public int Taxis { get; set; }
        }
    }
}
