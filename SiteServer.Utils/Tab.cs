using System;
using System.Xml.Serialization;

namespace SiteServer.Utils
{
	/// <summary>
	/// Tab is a container object which represents a singe tab
	/// </summary>
	[Serializable]
	public class Tab
	{
        private string _id;
        private string _parentId;
        private string _text;
        private string _href;
        private string _name;
        private string _permissions;
        private bool _enable = true;
        private Tab[] _children;
        private bool _selected;
        private string _target;
        private string _iconClass;

        /// <summary>
        /// Property Text (string)
        /// </summary>
        [XmlAttribute("id")]
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// Property Text (string)
        /// </summary>
        [XmlAttribute("parentId")]
        public string ParentId
        {
            get { return _parentId; }
            set { _parentId = value; }
        }

        /// <summary>
        /// Property Text (string)
        /// </summary>
        [XmlAttribute("text")]
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }


        /// <summary>
        /// Property Href (string)
        /// </summary>
        [XmlAttribute("href")]
        public string Href
        {
            get { return _href; }
            set { _href = value; }
        }

        /// <summary>
        /// Property Name (string)
        /// </summary>
        [XmlAttribute("name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Property Permissions (string)
        /// </summary>
        [XmlAttribute("permissions")]
        public string Permissions
        {
            get { return _permissions; }
            set { _permissions = value; }
        }

        /// <summary>
        /// Property Enable (bool)
        /// </summary>
        [XmlAttribute("enabled")]
        public bool Enabled
        {
            get { return _enable; }
            set { _enable = value; }
        }

        /// <summary>
        /// Property Selected (bool)
        /// </summary>
        [XmlAttribute("selected")]
        public bool Selected
        {
            get { return _selected; }
            set { _selected = value; }
        }

        /// <summary>
        /// Property Target (string)
        /// </summary>
        [XmlAttribute("target")]
        public string Target
        {
            get { return _target; }
            set { _target = value; }
        }

        /// <summary>
        /// Property IconClass (string)
        /// </summary>
        [XmlAttribute("iconClass")]
        public string IconClass
        {
            get { return _iconClass; }
            set { _iconClass = value; }
        }


        /// <summary>
        /// Property Children (Tab[])
        /// </summary>
        [XmlArray("SubTabs")]
        public Tab[] Children
        {
            get { return _children; }
            set { _children = value; }
        }

        public bool HasChildren => Children != null && Children.Length > 0;

        public bool HasPermissions => !string.IsNullOrEmpty(Permissions);

        public bool HasHref => !string.IsNullOrEmpty(Href);

        public Tab Clone()
        {
            return MemberwiseClone() as Tab;
        }

	}
}
