using System.Collections.Generic;
using SS.CMS.Data;

namespace SS.CMS.Models
{
    public partial class Channel
    {
        public object Clone()
        {
            return (Channel)MemberwiseClone();
        }

        [DataIgnore]
        public IList<Channel> Children { get; set; }
    }
}