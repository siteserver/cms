using System;
using System.Collections.Generic;

namespace SiteServer.CMS.Dto
{
    [Serializable]
    public class Option<T>
    {
        public T Value { get; set; }
        public string Label { get; set; }
        public List<Option<T>> Children { get; set; }
    }
}
