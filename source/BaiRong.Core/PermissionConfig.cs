using System.Xml;

namespace BaiRong.Core.Configuration
{
	public class PermissionConfig
	{
		string name;
		string text;

        public PermissionConfig(XmlAttributeCollection attributes) 
		{
            name = attributes["name"].Value;
            text = attributes["text"].Value;
		}

        public PermissionConfig(string name, string text)
        {
            this.name = name;
            this.text = text;
        }

		public string Name 
		{
			get 
			{
				return name;
			}
            set
            {
                name = value;
            }
		}

		public string Text 
		{
			get 
			{
				return text;
			}
            set
            {
                text = value;
            }
		}
	}
}
