using System.Collections.Generic;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Home.Write
{
    public partial class ContentsLayerViewController
    {
        public class GetRequest : ChannelRequest
        {
            public int ContentId { set; get; }
        }

        public class GetResult
        {
            public Content Content { set; get; }
            public string ChannelName { set; get; }
            public List<ContentColumn> Attributes { set; get; }
        }
    }
}
