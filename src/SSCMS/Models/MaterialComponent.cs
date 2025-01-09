using System.Collections.Generic;
using Datory;
using Datory.Annotations;

namespace SSCMS.Models
{
    [DataTable("siteserver_MaterialComponent")]
    public class MaterialComponent : Entity
    {
        [DataColumn]
        public int GroupId { get; set; }

        [DataColumn]
        public string Title { get; set; }
        
        [DataColumn]
        public string Description { get; set; }

        [DataColumn]
        public string ImageUrl { get; set; }

        [DataColumn]
        public string Parameters { get; set; }

        [DataColumn(Text = true)]
        public string Content { get; set; }
    }
}