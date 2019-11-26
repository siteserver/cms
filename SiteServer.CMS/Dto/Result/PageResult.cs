using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteServer.API.Result
{
    public class PageResult<T> where T : class
    {
        public IEnumerable<T> Items { get; set; }

        public int Count { get; set; }
    }
}