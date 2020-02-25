using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SS.CMS.Web.Controllers.Stl
{
    public partial class ActionsIfController
    {
        public class SubmitRequest
        {
            public string Value { get; set; }
            public int Page { get; set; }
        }

        public class SubmitResult
        {
            public bool Value { get; set; }
            public string Html { get; set; }
        }
    }
}
