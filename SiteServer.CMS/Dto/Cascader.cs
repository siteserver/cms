using System;
using System.Collections.Generic;
using Datory;
using Datory.Utils;
using SiteServer.Abstractions;

namespace SiteServer.CMS.Dto
{
    public class Cascade<T> : Dictionary<string, object>
    {
        public T Value
        {
            get => TranslateUtils.Get(this, nameof(Value), default(T));
            set => this[nameof(Value)] = value;
        }

        public string Label {
            get => TranslateUtils.Get<string>(this, nameof(Label));
            set => this[nameof(Label)] = value;
        }

        public List<Cascade<T>> Children
        {
            get => TranslateUtils.Get<List<Cascade<T>>>(this, nameof(Children), null);
            set => this[nameof(Children)] = value;
        }
    }
}
