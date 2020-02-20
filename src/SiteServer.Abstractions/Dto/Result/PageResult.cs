using System.Collections.Generic;

namespace SiteServer.Abstractions.Dto.Result
{
    public class PageResult<T> where T : class
    {
        public IEnumerable<T> Items { get; set; }

        public int Count { get; set; }
    }
}