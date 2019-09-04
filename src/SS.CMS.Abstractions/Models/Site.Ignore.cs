using System.Collections.Generic;
using Newtonsoft.Json;
using SS.CMS.Data;
using SS.CMS.Repositories;

namespace SS.CMS.Models
{
    public partial class Site
    {
        public object Clone()
        {
            return (Site)MemberwiseClone();
        }

        [DataIgnore]
        public IList<Site> Children { get; set; }
    }
}