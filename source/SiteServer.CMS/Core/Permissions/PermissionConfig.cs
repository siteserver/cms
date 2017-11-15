using System.Xml;

namespace SiteServer.CMS.Core.Permissions
{
	public class PermissionConfig
	{
	    public PermissionConfig(XmlAttributeCollection attributes) 
		{
            Name = attributes["name"].Value;
            Text = attributes["text"].Value;
		}

        public PermissionConfig(string name, string text)
        {
            Name = name;
            Text = text;
        }

		public string Name { get; set; }

	    public string Text { get; set; }
	}
}
