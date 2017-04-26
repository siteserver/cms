using System;
using System.Xml.Serialization;

namespace BaiRong.Core.Tabs
{
	/// <summary>
	/// Tab is a container object which represents a singe tab
	/// </summary>
	[Serializable]
	public class Tab
	{
        private string _id;
        private string _text;
        private string _href;
        private string _name;
        private string _permissions;
        private bool _enable = true;
        private bool _isOwner;
        private bool _isPlatform;
        private Tab[] _children;
        private bool _keepQueryString;
        private bool _selected;
        private string _target;
        private string _iconUrl;
        private string _ban;
        private string _addtionalString;

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

        [XmlAttribute("isowner")]
        public bool IsOwner
        {
            get { return _isOwner; }
            set { _isOwner = value; }
        }

        [XmlAttribute("isplatform")]
        public bool IsPlatform
        {
            get { return _isPlatform; }
            set { _isPlatform = value; }
        }

        /// <summary>
        /// Property KeepQueryString (bool)
        /// </summary>
        [XmlAttribute("keepQueryString")]
        public bool KeepQueryString
        {
            get { return _keepQueryString; }
            set { _keepQueryString = value; }
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
        /// Property IconUrl (string)
        /// </summary>
        [XmlAttribute("iconUrl")]
        public string IconUrl
        {
            get { return _iconUrl; }
            set { _iconUrl = value; }
        }

        /// <summary>
        /// Property AddtionalString (string)
        /// </summary>
        [XmlAttribute("ban")]
        public string Ban
        {
            get { return _ban; }
            set { _ban = value; }
        }

        /// <summary>
        /// Property AddtionalString (string)
        /// </summary>
        [XmlAttribute("addtionalString")]
        public string AddtionalString
        {
            get { return _addtionalString; }
            set { _addtionalString = value; }
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
