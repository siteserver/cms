using System;
using System.Collections.Generic;

namespace SiteServer.CMS.StlParser.Model
{
    public class AttrEnum
    {
        public AttrEnum(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; }
        public string Description { get; }
    }
}
