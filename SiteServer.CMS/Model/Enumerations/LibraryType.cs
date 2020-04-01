using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SiteServer.CMS.Model.Enumerations
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LibraryType
    {
        Text,
        Image
    }
}
