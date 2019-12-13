using System;
using System.Collections.Generic;
using Datory;
using SiteServer.Abstractions;

namespace SiteServer.CMS.Dto
{
    [Serializable]
    public class Cascade<T>
    {
        public Cascade()
        {

        }

        public Cascade(T value, string label, List<Cascade<T>> children)
        {
            Value = value;
            Label = label;
            Children = children;
        }

        public T Value { get; set; }
        public string Label { get; set; }
        public List<Cascade<T>> Children { get; set; }
    }
}
