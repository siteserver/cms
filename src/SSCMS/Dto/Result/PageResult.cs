using System.Collections.Generic;

namespace SSCMS.Dto.Result
{
    public class PageResult<T> where T : class
    {
        public IEnumerable<T> Items { get; set; }

        public int Count { get; set; }
    }
}