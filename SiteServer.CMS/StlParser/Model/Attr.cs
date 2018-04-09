using System;
using System.Collections.Generic;

namespace SiteServer.CMS.StlParser.Model
{
    public class Attr
    {
        public Attr(string name, string description)
        {
            Name = name;
            Type = AttrType.String;
            Description = description;
        }

        public Attr(string name, string description, AttrType type)
        {
            Name = name;
            Type = type;
            Description = description;
        }

        public Attr(string name, string description, AttrType type, List<AttrEnum> enums)
        {
            Name = name;
            Type = type;
            Description = description;
            _enums = enums;
        }

        public string Name { get; }
        public AttrType Type { get; }
        public string Description { get; }

        private readonly List<AttrEnum> _enums;

        public List<AttrEnum> GetEnums(Type elementType, int siteId)
        {
            return _enums ?? AttrUtils.GetEnums(elementType, Name, siteId);
        }
    }
}
