using System;
using System.Collections.Generic;
using Datory;
using SiteServer.Abstractions;

namespace SiteServer.CMS.Dto
{
    [Serializable]
    public class Cascade<T>
    {
        public T Value { get; set; }
        public string Label { get; set; }
        public Dictionary<string, object> Dict { get; set; }
        public List<Cascade<T>> Children { get; set; }
    }
}
