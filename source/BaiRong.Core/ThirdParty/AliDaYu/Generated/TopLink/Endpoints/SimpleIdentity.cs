using System.Collections.Generic;

namespace Taobao.Top.Link.Endpoints
{
    /// <summary>simple id with Name
    /// </summary>
    public class SimpleIdentity : Identity
    {
        public string Name { get; private set; }

        public SimpleIdentity(string name)
        {
            this.Name = name;
        }

        public Identity Parse(object data)
        {
            return new SimpleIdentity((data as IDictionary<string, string>)["name"]);
        }

        public void Render(object to)
        {
            (to as IDictionary<string, object>).Add("name", this.Name);
        }

        public bool Equals(Identity id)
        {
            return this.Name.Equals((id as SimpleIdentity).Name);
        }
    }
}